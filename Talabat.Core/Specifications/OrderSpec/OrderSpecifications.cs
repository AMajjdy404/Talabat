using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggreagate;

namespace Talabat.Core.Specifications.OrderSpec
{
    public class OrderSpecifications : BaseSpecification<Order>
    {
        public OrderSpecifications(string email):base(o => o.BuyerEmail == email)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
            AddOrderByDesc(o => o.OrderDate);

        }

        public OrderSpecifications(string email,int orderId):base(o => o.BuyerEmail == email && o.Id == orderId) 
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
        }
    }
}
