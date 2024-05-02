namespace WebBanGiay.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Size { get; set; }
        public string? ImageUrl { get; set; }
        public string Url { get; set; }
        public List<ProductImage>? Images { get; set; }
    }
}
