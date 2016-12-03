using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Elena.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Customer Customer { get; set; }

        [Required]
        public IEnumerable<Product> Products { get; set; }

        [Required]
        public OrderStatus Status { get; set; }
    }
}
