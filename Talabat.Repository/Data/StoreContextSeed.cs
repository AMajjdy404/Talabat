using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggreagate;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context)
        {
            // Brands Seeding
            if (!context.ProductBrands.Any())
            {
                var BrandData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandData);
                if (Brands?.Count > 0)
                {
                    foreach (var brand in Brands)
                    {
                        await context.Set<ProductBrand>().AddAsync(brand);
                    }
                    await context.SaveChangesAsync();
                }
            }

            // Types Seeding
            if (!context.ProductTypes.Any())
            {
                var TypeData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/types.json");
                var Types = JsonSerializer.Deserialize<List<ProductType>>(TypeData);
                if (Types?.Count > 0)
                {
                    foreach (var type in Types)
                    {
                        await context.Set<ProductType>().AddAsync(type);
                    }
                    await context.SaveChangesAsync();
                }
            }

            // Products Seeding
            if (!context.Products.Any())
            {
                var ProductData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                var Products = JsonSerializer.Deserialize<List<Product>>(ProductData);
                if (Products?.Count > 0)
                {
                    foreach (var product in Products)
                    {
                        await context.Set<Product>().AddAsync(product);
                    }
                    await context.SaveChangesAsync();
                }
            }

            // DeliveryMethods Seeding
            if (!context.DeliveryMethods.Any())
            {
                var DeliveryMethodData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");
                var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodData);
                if (DeliveryMethods?.Count > 0)
                {
                    foreach (var DeliveryMethod in DeliveryMethods)
                    {
                        await context.Set<DeliveryMethod>().AddAsync(DeliveryMethod);
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
