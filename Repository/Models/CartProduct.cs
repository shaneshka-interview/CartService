namespace CartRepository.Models
{
    public class CartProduct
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Cost { get; set; }
        public bool IsBonusPoints { get; set; }
    }
}