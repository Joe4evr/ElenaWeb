using System.ComponentModel.DataAnnotations;

namespace Elena.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required]
        public string Streetname { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; }
    }
}
