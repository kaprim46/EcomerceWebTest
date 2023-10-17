using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Ecommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ecommerce.DataAccess.Repository;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Http;

namespace EcommerceWebTest.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> products = _unitOfWork.product.GetAll(includeProperties:"Category,ProductImages");
        return View(products);
    }

    public IActionResult Details(int productId)
    {
        ShoppingCart cart = new()
        {
            Product = _unitOfWork.product.Get(q => q.ProductId == productId, includeProperties: "Category,ProductImages"),
            Count = 1,
            ProductId = productId
        };
        return View(cart);
    }


    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart model)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        model.ApplicationUserId = userId;
        var cart = _unitOfWork.shoppingCart.Get(q => q.ProductId == model.ProductId && q.ApplicationUser.Id == userId);
        if(cart != null)
        {
            cart.Count += model.Count;
            _unitOfWork.shoppingCart.Update(cart); //Entity framework tracking that and updated but i put tracked false in the repository
            _unitOfWork.Save();
        }
        else
        {
            _unitOfWork.shoppingCart.Add(model);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart,
                      _unitOfWork.shoppingCart.GetAll(q => q.ApplicationUserId == userId).Count());
        }

        TempData["success"] = "Cart updated successfully";
       
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

