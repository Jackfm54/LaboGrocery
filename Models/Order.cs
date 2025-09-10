using System;
using System.Collections.Generic;

namespace LaboGrocery.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Lo pedía tu controlador; si no la usas, no pasa nada que sea nullable.
        public string? UserId { get; set; }

        public decimal Total { get; set; }

        public ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
    }
}
