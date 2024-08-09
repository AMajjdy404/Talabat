using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.Order_Aggreagate;

namespace Talabat.API.DTOs
{
    public class OrderDto
    {
        [Required]
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public AddressDto shipToAddress { get; set; }
    }
}
