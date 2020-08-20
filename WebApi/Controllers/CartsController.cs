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
        private readonly ICartsRepository _cartsRepository;

        public CartsController(ILogger<CartsController> logger, ICartsRepository cartsRepository)
        {
            _logger = logger;
            _cartsRepository = cartsRepository;
        }

        [HttpGet("help")]
        public string GetHelp()
        {
            return @"
GET api/carts

GET api/carts/{id}/products

POST api/carts/{id}/products
BODY {""productId"":1,""count"":1,""cost"":1,""IsBonusPoints"":true}

PUT api/carts/{id}/products
BODY {""productId"":1,""count"":1,""cost"":1,""IsBonusPoints"":true}

DELETE api/carts/{id}/products/{productId}

POST api/carts/{id}/hookExpire
BODY ""http://""
";
        }

        [HttpGet]
        public async Task<IEnumerable<int>> GetCarts()
        {
            return (await _cartsRepository.GetCartsAsync())?.Select(x => x.Id);
        }

        [HttpGet("{id}/products")]
        public async Task<IEnumerable<ProductDto>> GetProducts(int id)
        {
            return (await _cartsRepository.GetProductsAsync(id))?.Select(ToDto);
        }

        [HttpPost("{id}/products")]
        public async Task<ActionResult> Add(int id, ProductDto dto)
        {
            await _cartsRepository.AddAsync(ToModel(id, dto));
            return NoContent();
        }

        [HttpPut("{id}/products")]
        public async Task<ActionResult> Update(int id, ProductDto dto)
        {
            await _cartsRepository.UpdateAsync(ToModel(id, dto));
            return NoContent();
        }

        [HttpDelete("{id}/products/{productId}")]
        public async Task<ActionResult> DeleteProduct(int id, int productId)
        {
            await _cartsRepository.DeleteAsync(new ProductDeleteDto
            {
                CartId = id,
                ProductId = productId
            });
            return NoContent();
        }

        [HttpPost("{id}/hookExpire")]
        public async Task<ActionResult> AddHook(int id, [FromBody] string url)
        {
            await _cartsRepository.AddHookAsync(new CartHook
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
                Cost = product.Cost,
                ProductId = product.ProductId,
                IsBonusPoints = product.IsBonusPoints
            };
        }

        private static CartProduct ToModel(int cartId, ProductDto product)
        {
            return new CartProduct
            {
                Count = product.Count,
                Cost = product.Cost,
                CartId = cartId,
                ProductId = product.ProductId,
                IsBonusPoints = product.IsBonusPoints
            };
        }
    }
}