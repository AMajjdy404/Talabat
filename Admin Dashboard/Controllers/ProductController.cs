using Admin_Dashboard.Helpers;
using Admin_Dashboard.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Admin_Dashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var ProductSpecParams = new ProductSpecParams()
            {
                PageSize = 10,
            };
            var spec = new ProductWithBrandAndTypeSpecification(ProductSpecParams);
            var products = await unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var mappedProducts = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductViewModel>>(products);
            return View(mappedProducts);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image != null)
                    model.PictureUrl = DocumentSettings.UploadFile(model.Image, "Products");
                var mappedProduct = mapper.Map<ProductViewModel, Product>(model);
                await unitOfWork.Repository<Product>().AddAsync(mappedProduct);
                await unitOfWork.CompleteAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await unitOfWork.Repository<Product>().GeyByIdAsync(id);
            if (product.PictureUrl != null)
                DocumentSettings.DeleteFile(product.PictureUrl, "Products");

             unitOfWork.Repository<Product>().Delete(product);
            await unitOfWork.CompleteAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await unitOfWork.Repository<Product>().GeyByIdAsync(id);
            var mappedProduct = mapper.Map<Product,ProductViewModel>(product);
            return View(mappedProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image != null)
                {
                    if (model.PictureUrl != null)
                        DocumentSettings.DeleteFile(model.PictureUrl, "Products");
                    model.PictureUrl = DocumentSettings.UploadFile(model.Image, "Products");

                }

                var mappedProduct = mapper.Map<ProductViewModel, Product>(model);
                unitOfWork.Repository<Product>().Update(mappedProduct);
                await unitOfWork.CompleteAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
