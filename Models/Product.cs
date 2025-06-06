﻿namespace WebApplication_AuthenticationSystem_.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Brand { get; set; } = "";

        public string Category { get; set; } = "";

        public decimal Price { get; set; }

        public string Description { get; set; } = "";
        public string? ImageFileName { get; set; }

        public IFormFile? ImageFile { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
