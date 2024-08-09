using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggreagate;

namespace Talabat.Repository.Data.Configurations
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)  
        {
           builder.Property(o => o.Status)
                .HasConversion(os => os.ToString(),os => (OrderStatus) Enum.Parse(typeof(OrderStatus),os));
                   // Stored in DB                              // Returned From DB

            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");

            builder.OwnsOne(o => o.ShippingAddress, x => x.WithOwner());

            builder.HasOne(o => o.DeliveryMethod)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
