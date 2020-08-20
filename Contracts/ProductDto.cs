namespace Contracts
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Cost { get; set; }
        public bool IsBonusPoints { get; set; }
    }

    public class ProductDeleteDto
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
    }
}