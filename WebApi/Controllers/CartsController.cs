using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartRepository;
using CartRepository.Models;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly ILogger<CartsController> _logger;
        private readonly ICartRepository _cartRepository;

        public CartsController(ILogger<CartsController> logger, ICartRepository cartRepository)
        {
            _logger = logger;
            _cartRepository = cartRepository;
        }

        [HttpGet,Route("help")]
        public async Task<string> GetHelp()
        {
            return @"
GET api/carts

GET api/carts/{id}/products
POST api/carts/{id}/products
DELETE api/carts/{id}/products

POST api/carts/{id}/hook
";
        }

        [HttpGet]
        public async Task<IEnumerable<int>> GetCarts()
        {
            return null;
        }

        [HttpGet, Route("{id}/products")]
        public async Task<IEnumerable<ProductDto>> GetProducts(int id)
        {
            return (await _cartRepository.GetProductsAsync(id))?.Select(ToDto);
        }

        [HttpPost, Route("{id}/products")]
        public async Task<ActionResult> Add(int id, ProductDto dto)
        {
            await _cartRepository.AddAsync(ToModel(id, dto));
            return NoContent();
        }

        [HttpDelete, Route("{id}/products")]
        public async Task<ActionResult> DeleteProduct(int id, int productId)
        {
            await _cartRepository.DeleteAsync(new ProductDeleteDto
            {
                CartId = id,
                ProductId = productId
            });
            return NoContent();
        }

        [HttpPost, Route("{id}/hook")]
        public async Task<ActionResult> AddHook(int id, string url)
        {
            await _cartRepository.AddHookAsync(new CartHook
            {
                Url = url,
                CartId = id
            });
            return NoContent();
        }

        private static ProductDto ToDto(CartProduct product)
        {
            return new ProductDto
            {
                Count = product.Count,
                ProductId = product.ProductId,
                IsBonusPoints = product.IsBonusPoints
            };
        }

        private static CartProduct ToModel(int cartId, ProductDto product)
        {
            return new CartProduct
            {
                Count = product.Count,
                CartId = cartId,
                ProductId = product.ProductId,
                IsBonusPoints = product.IsBonusPoints
            };
        }
    }
}