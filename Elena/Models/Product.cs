using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Elena.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        [Required]
        public ProductType Type { get; set; }
        [Required]
        public decimal Price { get; set; }
        public Customer Buyer { get; set; }
    }
}
