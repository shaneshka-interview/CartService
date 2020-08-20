using System;

namespace CartRepository.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}