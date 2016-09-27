using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using DotNet5CloudWeb.Models;
using DotNet5CloudWeb.Models.DocumentDbModels;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNet5CloudWeb.Controllers {
    public class AlbumController : Controller {
        public const string NoFilterWord = "-All-";

        private const string DbName = "Albums";
        private const string AlbumNamesKey = "~AlbumNames~";

        private string DocumentDbEndPoint => ConfigurationManager.AppSettings["DDbEndPoint"];

        private string DocumentDbAuthKey => ConfigurationManager.AppSettings["DDbMasterKey"];

        #region Properties
        private List<string> CachedAlbumNames {
            get {
                if (Session[AlbumNamesKey] != null) {
                    return Session[AlbumNamesKey] as List<string>;
                }

                var albumNames = PerformQuery("SELECT VALUE Albums.Name FROM RockAlbums D JOIN Albums in D.Albums");

                var names = new List<string>(albumNames.Count);
                names.AddRange(from object albumName in albumNames select albumName as JValue into val select val.Value as string);

                names.Sort();

                Session[AlbumNamesKey] = names;
                return names;
            }
        }
        #endregion

        #region Actions
        public ActionResult QueryAlbums() {
            var model = new QueryAlbumsModel { AlbumNames = CachedAlbumNames };

            model.AlbumDetails = SearchDiscographies(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QueryAlbums(QueryAlbumsModel model) {
            if (!ModelState.IsValid) {
                throw new Exception("Model Not Valid!!!");
            }

            model.AlbumNames = CachedAlbumNames;
            model.AlbumDetails = SearchDiscographies(model);

            return View(model);
        }

        public ActionResult Credits() {
            return View();
        }
        #endregion

        #region Helper methods
        private IList PerformQuery(string query) {
            var ddbClient = new DocumentClient(new Uri(DocumentDbEndPoint), DocumentDbAuthKey);
            var db = ddbClient.CreateDatabaseQuery()
                .Where(d => d.Id == DbName).AsEnumerable().FirstOrDefault();

            var collectionQueryUri = $"dbs/{db.Id}/colls/RockAlbums";
            return ddbClient.CreateDocumentQuery(collectionQueryUri, query).ToList();
        }

        private List<AlbumCollection> SearchDiscographies(QueryAlbumsModel model) {
            var query = string.Empty;

            if (model.HasFilter) {
                query = "SELECT C.ArtistName, "
                    + "{ " +
                            "\"Name\": A.Name, " +
                            "\"Year\": A.Year, " +
                            "\"Label\": A.Label, " +
                            "\"CopiesSold\": A.CopiesSold " +
                        "} AS Album, "
                        +
                        "{ " +
                            "\"First\": M.FirstName, " +
                            "\"Last\": M.LastName, " +
                            "\"Instrument\": M.Instrument " +
                        "} AS Member " +
                        "FROM C " +
                        "JOIN A IN C.Albums " +
                        "JOIN M IN A.Members"
                        + $" WHERE A.Name = '{model.AlbumNameFilter}'";
            } else {
                query = "SELECT  C.ArtistName, " +
                        "{ " +
                            "\"Name\": A.Name, " +
                            "\"Year\": A.Year, " +
                            "\"Label\": A.Label, " +
                            "\"CopiesSold\": A.CopiesSold " +
                        "} AS Album "
                        + "FROM C " +
                        "JOIN A IN C.Albums";
            }

            var filteredDiscographies = PerformQuery(query);

            var discographies = new List<AlbumCollection>();

            var counter = 0;
            foreach (var discJSON in filteredDiscographies) {
                //JToken token = JObject.Parse(discJSON.ToString());
                //var artistName = (string)token.SelectToken("ArtistName");
                //var albumLabel = (string) token.SelectToken("Album.Label");

                var deser = JsonConvert.DeserializeObject<AlbumCollection>(discJSON.ToString());

                if (model.HasFilter) {
                    if (counter > 0) {
                        deser.Album = null; // don't show album info
                        deser.ArtistName = null; // don't show album name for "member" rows
                    }
                }
                discographies.Add(deser);
                counter++;
            }

            return discographies;
        }
        #endregion
    }
}