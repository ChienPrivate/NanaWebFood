﻿namespace NanaFoodWeb.Models
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }=true;
    }
}
