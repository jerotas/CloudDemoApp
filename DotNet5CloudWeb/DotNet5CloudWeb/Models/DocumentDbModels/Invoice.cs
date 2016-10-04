using System;

namespace DotNet5CloudWeb.Models.DocumentDbModels {
    public class Invoice {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public Address Address { get; set; }
        public ShipInfo ShipInfo { get; set; }
        public Product Product { get; set; }
    }

    public class Address
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }

    public class ShipInfo
    {
        public DateTime ShippedDate { get;set; }
        public string ShipperName { get; set; }
        public decimal Freight { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public decimal Discount { get; set; }
        public decimal ItemSubtotal { get; set; }
    }
}
