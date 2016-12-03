using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Required, Range(0, 999.99)]
        public decimal Price { get; set; }

        public Customer Buyer { get; set; }

        [NotMapped]
        public bool IsAvailable => !(Type == ProductType.Painting && Buyer != null);
    }
}
