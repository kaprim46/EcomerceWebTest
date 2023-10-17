using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModel;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceWebTest.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.product.GetAll(includeProperties:"Category").ToList();
            return View(products);
        }

        public IActionResult UpSert(int? id)
        {
            var productVM = new ProductVM
            {
                CategoryList = _unitOfWork.category.GetAll().Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.CategoryId.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.product.Get(q => q.ProductId == id, includeProperties: "ProductImages");
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult UpSert(ProductVM model, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (model.Product.ProductId == 0)
                {
                    _unitOfWork.product.Add(model.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.product.Update(model.Product);
                    TempData["success"] = "Product updated successfully";
                }

                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images/products/product-" + model.Product.ProductId;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"/" + productPath + @"/" + fileName,
                            ProductId = model.Product.ProductId
                        };
                        if (model.Product.ProductImages == null)
                        {
                            model.Product.ProductImages = new List<ProductImage>();
                        }
                        model.Product.ProductImages.Add(productImage);
                        
                    }

                    _unitOfWork.product.Update(model.Product);
                    _unitOfWork.Save();

                    
                }
                TempData["success"] = "Product created/updated successfuly";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.CategoryList = _unitOfWork.category
                    .GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.CategoryId.ToString()
                    });
                return View(model);
            }

        }

        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.productImage.Get(q => q.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImage))
                    {
                        System.IO.File.Delete(oldImage);
                    }
                }
                _unitOfWork.productImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();
                TempData["success"] = "Deleted successfully";
            }
            return RedirectToAction(nameof(UpSert), new {id = productId});
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product product = _unitOfWork.product.Get(q => q.ProductId == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            
            string productPath = @"images/products/product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (var filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }
                

            _unitOfWork.product.Remove(product);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
        #endregion
    
}