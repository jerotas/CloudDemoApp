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
        private const string DbName = "Albums";
        private const string AlbumNamesKey = "~AlbumNames~";

        private string DocumentDbEndPoint => ConfigurationManager.AppSettings["DDbEndPoint"];

        private string DocumentDbAuthKey => ConfigurationManager.AppSettings["DDbMasterKey"];

        private List<string> CachedAlbumNames
        {
            get
            {
                if (Session[AlbumNamesKey] != null)
                {
                    return Session[AlbumNamesKey] as List<string>;
                }

                var albumNames = PerformQuery("SELECT Albums.Name FROM RockAlbums D JOIN Albums in D.Albums");

                var names = new List<string>(albumNames.Count);

                foreach (var album in albumNames) {
                    JToken token = JObject.Parse(album.ToString());
                    var albumName = (string)token.SelectToken("Name");
                    names.Add(albumName);
                }

                names.Sort();

                Session[AlbumNamesKey] = names;
                return names;
            }
        } 

        public ActionResult QueryAlbums() {
            var model = new QueryAlbumsModel {AlbumNames = CachedAlbumNames };

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

        private IList PerformQuery(string query) {
            var ddbClient = new DocumentClient(new Uri(DocumentDbEndPoint), DocumentDbAuthKey);
            var db = ddbClient.CreateDatabaseQuery()
                .Where(d => d.Id == DbName).AsEnumerable().FirstOrDefault();

            var collectionQueryUri = $"dbs/{db.Id}/colls/RockAlbums";
            return ddbClient.CreateDocumentQuery(collectionQueryUri, query).ToList();
        }

        private List<Album> SearchDiscographies(QueryAlbumsModel model) {
            var query = "SELECT D.ArtistName, Albums.Name, Albums.Year, Albums.Label, Albums.CopiesSold FROM RockAlbums D JOIN Albums in D.Albums";
            if (!string.IsNullOrWhiteSpace(model.AlbumNameFilter)) {
                query += $" WHERE Albums.Name = '{model.AlbumNameFilter}'";
            }

            var filteredDiscographies = PerformQuery(query);

            var discographies = new List<Album>();

            foreach (var discJSON in filteredDiscographies) {
                //    //JToken token = JObject.Parse(college.ToString());
                //    //var cName = (string)token.SelectToken("collegeName");
                //    //var branchId = (string) token.SelectToken("branches[0].branchId");

                Album deser = JsonConvert.DeserializeObject<Album>(discJSON.ToString());
                discographies.Add(deser);

                //Console.Write($"\t Result is college: {deser.ArtistName}");
            }

            return discographies;
        }
    }
}