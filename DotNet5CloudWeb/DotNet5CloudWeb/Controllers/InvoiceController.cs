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
        private const string DbName = "North";

        private string DocumentDbEndPoint => ConfigurationManager.AppSettings["DDbEndPoint"];

        private string DocumentDbAuthKey => ConfigurationManager.AppSettings["DDbMasterKey"];

        #region Actions
        public ActionResult Index() {
            var model = new InvoicesModel();
            model.Invoices = SearchInvoices(model);

            return View(model);
        }

        public ActionResult FilterInvoices(string orderId) {
            var model = new InvoicesModel();
            model.OrderIdFilter = int.Parse(orderId);
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
                    .Where(d => d.Id == DbName).AsEnumerable().FirstOrDefault();

                var collectionQueryUri = $"dbs/{db.Id}/colls/Invoices";
                var results = ddbClient.CreateDocumentQuery<T>(collectionQueryUri, query);
                return results.ToList();
            }
        }

        private List<Invoice> SearchInvoices(InvoicesModel model) {
            var query = string.Empty;

            if (model.OrderIdFilter.HasValue) {
                query = $"SELECT * FROM c WHERE c.OrderId = {model.OrderIdFilter.Value}";
            } else {
                query = "SELECT * FROM c";
            }

            var filteredInvoices = PerformQuery<Invoice>(query);
            var invoiceList = filteredInvoices.ToList();
            return invoiceList;
        }
        #endregion
    }
}