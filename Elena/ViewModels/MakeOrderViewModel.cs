using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Elena.Models;

namespace Elena.ViewModels
{
    public class MakeOrderViewModel
    {
        public MakeOrderViewModel(IEnumerable<Product> products)
        {
            Products = products;
        }

        [Required, DisplayName("Voornaam")]
        public string FirstName { get; set; }

        [Required, DisplayName("Achternaam")]
        public string LastName { get; set; }

        [Required, DisplayName("E-mail adres"), EmailAddress]
        public string Email { get; set; }

        [Required, DisplayName("Straat")]
        public string Streetname { get; set; }

        [Required, DisplayName("Huisnummer")]
        public string Number { get; set; }

        [Required, DisplayName("Postcode")]
        public string PostalCode { get; set; }

        [Required, DisplayName("Woonplaats")]
        public string City { get; set; }

        public string Country { get; set; } = "Nederland";

        public IEnumerable<Product> Products { get; set; }
    }
}
