using System.Collections.Generic;
using DotNet5CloudWeb.Models.DocumentDbModels;

namespace DotNet5CloudWeb.Models {
    public class InvoicesModel {
        public int? OrderIdFilter { get; set; }
        public List<Invoice> Invoices { get; set; }
    }
}