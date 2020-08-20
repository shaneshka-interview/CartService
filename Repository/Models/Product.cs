namespace CartRepository.Models
{
    public class Product
    {
        public int Id { get; set; } //todo guid?
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public bool ForBonusPoints { get; set; }
    }
}