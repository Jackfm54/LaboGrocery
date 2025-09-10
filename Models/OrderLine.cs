namespace LaboGrocery.Models
{
    public class OrderLine
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public int ProductId { get; set; }      // FK al Product existente
        public int Quantity { get; set; }
        public decimal Price { get; set; }      // Precio al momento de compra
    }
}
