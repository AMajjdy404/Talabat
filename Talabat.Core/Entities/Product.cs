using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    public class Product: ProductBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        public int ProductBrandId { get; set; } // FK

        public int ProductTypeId { get; set; } // FK
        public ProductBrand ProductBrand { get; set; }

        public ProductType ProductType { get; set; }

    }
}
