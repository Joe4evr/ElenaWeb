using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Elena.Models;

namespace Elena.ViewModels
{
    public class MakeOrderViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Streetname { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }


        public string Country { get; set; }

        public IEnumerable<Product> Products { get; set; }

    }
}
