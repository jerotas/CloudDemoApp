using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using DotNet5CloudWeb.Models;
using DotNet5CloudWeb.Models.DocumentDbModels;
using Microsoft.Azure.Documents.Client;

namespace DotNet5CloudWeb.Controllers {
    public class InvoiceController : Controller {
        private string DocumentDbEndPoint => ConfigurationManager.AppSettings["DDbEndPoint"];

        private string DocumentDbAuthKey => ConfigurationManager.AppSettings["DDbMasterKey"];
        private string DocumentDbName => ConfigurationManager.AppSettings["DDbName"];

        #region Actions
        public ActionResult Index() {
            var model = new InvoicesModel();
            model.Invoices = SearchInvoices(model);

            return View(model);
        }

        public ActionResult FilterInvoicesByOrderId(string orderId) {
            var model = new InvoicesModel { OrderIdFilter = int.Parse(orderId) };
            model.Invoices = SearchInvoices(model);

            return View(model);
        }

        public ActionResult FilterInvoicesByProductId(string productId) {
            var model = new InvoicesModel {ProductIdFilter = int.Parse(productId)};
            model.Invoices = SearchInvoices(model);

            return View(model);
        }

        public ActionResult FilterInvoicesByCountry(string country) {
            var model = new InvoicesModel {CountryFilter = country};
            model.Invoices = SearchInvoices(model);

            return View(model);
        }

        public ActionResult Credits() {
            return View();
        }
        #endregion

        #region Helper methods
        private IList<T> PerformQuery<T>(string query) {
            DocumentClient ddbClient;
            using (ddbClient = new DocumentClient(new Uri(DocumentDbEndPoint), DocumentDbAuthKey)) {
                var db = ddbClient.CreateDatabaseQuery()
                    .Where(d => d.Id == DocumentDbName).AsEnumerable().FirstOrDefault();

                var collectionQueryUri = $"dbs/{db.Id}/colls/Invoices";
                var results = ddbClient.CreateDocumentQuery<T>(collectionQueryUri, query);
                return results.ToList();
            }
        }

        private List<Invoice> SearchInvoices(InvoicesModel model) {
            var query = "SELECT * FROM c";

            if (!string.IsNullOrWhiteSpace(model.CountryFilter)) {
                query = $"SELECT * FROM c WHERE c.Address.Country = '{model.CountryFilter}'";
            } else if (model.ProductIdFilter.HasValue) {
                query = $"SELECT * FROM c WHERE c.Product.Id = {model.ProductIdFilter.Value}";
            } else if (model.OrderIdFilter.HasValue) {
                query = $"SELECT * FROM c WHERE c.OrderId = {model.OrderIdFilter.Value}";
            } 

            var filteredInvoices = PerformQuery<Invoice>(query);

            // You can also do faster parsing if you need less from it, and don't want to parse the entire JSON all the tiem.
            //    JToken token = JObject.Parse(invoiceJSON.ToString());
            //    var cName = (string)token.SelectToken("collegeName");
            //    var branchId = (string) token.SelectToken("branches[0].branchId");
            // OR - parse the entire document optionally.
            //    Invoice deser = JsonConvert.DeserializeObject<Invoice>(invoiceJSON.ToString());

            return filteredInvoices.ToList();
        }
        #endregion
    }
}