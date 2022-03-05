using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _logger = logger;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();

            var response = await _productService.GetAllProductsAsync<ResponseDto>("");

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());
            }

            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            ProductDto productDto = new();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id,"");
            if (response != null && response.IsSuccess)
                productDto = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());

            return View(productDto);
        }

        [HttpPost]
        [Authorize]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDto cartDto = new() {
                CartHeader = new() {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetailsDto = new() {
                Count = productDto.Count,
                ProductId = productDto.Id
            };

            //we could populate it from 'productDto' too

            var responseDto = await _productService.GetProductByIdAsync<ResponseDto>(productDto.Id, null);

            if (responseDto != null && responseDto.IsSuccess)
                cartDetailsDto.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Result));

            List<CartDetailsDto> cartDetailsDtos = new();
            cartDetailsDtos.Add(cartDetailsDto);
            cartDto.CartDetails = cartDetailsDtos;

            string token = await HttpContext.GetTokenAsync("access_token");
            var responseAddDetails = await _cartService.AddCartAsync<ResponseDto>(cartDto, token: token);

            if (responseAddDetails != null && responseAddDetails.IsSuccess)
                return RedirectToAction(nameof(Index));
            return View(productDto);

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

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var token = await HttpContext.GetTokenAsync("access_token");


            var isInrole = User.Claims.ToList();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
