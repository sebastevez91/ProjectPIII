using System;
using System.Collections.Generic;

namespace AutoPartesRazor.Models
{
    public class Order
    {
        public int id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navegación
        public List<OrderItem> Items { get; set; } = new();
    }
}