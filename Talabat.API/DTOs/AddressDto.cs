using System.ComponentModel.DataAnnotations;

namespace Talabat.API.DTOs
{
    public class AddressDto
    {
        [Required]
        public int Id { get; set; }
        [Required]

        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]

        public string Street { get; set; }
        [Required]

        public string City { get; set; }
        [Required]

        public string Country { get; set; }
    }
}
