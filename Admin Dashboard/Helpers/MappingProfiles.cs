using Admin_Dashboard.Models;
using AutoMapper;
using Talabat.Core.Entities;

namespace Admin_Dashboard.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product,ProductViewModel>().ReverseMap();
        }
    }
}
