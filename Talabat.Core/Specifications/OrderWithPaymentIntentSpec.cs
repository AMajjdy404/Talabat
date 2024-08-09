using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggreagate;

namespace Talabat.Core.Specifications
{
    public class OrderWithPaymentIntentSpec : BaseSpecification<Order>
    {
        public OrderWithPaymentIntentSpec(string paymentIntentId):base(o => o.PaymentIntentId == paymentIntentId)
        {
            
        }
    }
}
