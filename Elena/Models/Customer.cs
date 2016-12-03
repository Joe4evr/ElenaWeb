using System.ComponentModel.DataAnnotations;

namespace Elena.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public Address Address { get; set; }

        public string VatNumber { get; set; }
    }
}
