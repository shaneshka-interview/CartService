using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using CartRepository.Models;
using Contracts;
using Cart = CartRepository.Models.Cart;
using CartProduct = CartRepository.Models.CartProduct;

namespace CartRepository
{
    public interface ICartRepository
    {
        Task AddAsync(CartProduct cartProduct);
        Task UpdateAsync(CartProduct cartProduct);
        Task DeleteAsync(ProductDeleteDto cartProduct);
        Task DeleteAsync(int cartId);
        Task<IEnumerable<CartProduct>> GetProductsAsync(int cartId);

        Task AddHookAsync(CartHook cartHook);
    }

    public class CartRepository : BaseRepository, ICartRepository
    {
        public CartRepository(string conn) : base(conn)
        {
        }

        public async Task AddAsync(CartProduct cartProduct)
        {
            await ActivateCartAsync(cartProduct.CartId);
            using var db = GetConnection();
            var sqlQuery = "INSERT INTO CartProduct (CartId, ProductId) VALUES(@CartId, @ProductId)";
            await db.ExecuteAsync(sqlQuery, cartProduct);
        }

        public async Task UpdateAsync(CartProduct cartProduct)
        {
            await ActivateCartAsync(cartProduct.CartId);
            using var db = GetConnection();
            var sqlQuery = "UPDATE CartProduct SET Count = @Count, IsBonusPoints = @IsBonusPoints WHERE Id = @Id";
            await db.ExecuteAsync(sqlQuery, cartProduct);
        }

        public async Task DeleteAsync(ProductDeleteDto cartProduct)
        {
            using var db = GetConnection();
            var sqlQuery = "DELETE FROM CartProduct WHERE ProductId = @ProductId and CartId=@CartId";
            await db.ExecuteAsync(sqlQuery, cartProduct);
        }

        public async Task DeleteAsync(int cartId)
        {
            using var db = GetConnection();
            var sqlQuery = "DELETE FROM CartProduct WHERE CartId=@CartId";
            await db.ExecuteAsync(sqlQuery, new {CartId = cartId});

            var sqlQuery2 = "Update Cart set IsDeleted=true WHERE Id=@CartId";
            await db.ExecuteAsync(sqlQuery2, new {CartId = cartId});
        }

        public async Task<IEnumerable<CartProduct>> GetProductsAsync(int cartId)
        {
            using var db = GetConnection();
            return (await db.QueryAsync<CartProduct>(
                "SELECT * FROM CartProduct cp WHERE cp.Id = @CartId",
                new {CartId = cartId})).ToArray();
        }

        public async Task AddHookAsync(CartHook cartHook)
        {
            using var db = GetConnection();
            var insert = "INSERT INTO CartHook (CartId, Url) VALUES(@CartId, @Url)";
            await db.ExecuteAsync(insert, cartHook);
        }

        private async Task ActivateCartAsync(int cartId)
        {
            using var db = GetConnection();

            var sqlQuery = "Select * from Cart WHERE Id=@CartId";
            var cart = (await db.QueryAsync<Cart>(sqlQuery, new {CartId = cartId})).FirstOrDefault();
            if (cart == null)
            {
                var insert = "INSERT INTO Cart (IsDeleted, LastUpdated) VALUES(false, @LastUpdate)";
                await db.ExecuteAsync(insert, new {LastUpdate = DateTime.UtcNow});
            }
            else
            {
                var update = "Update Cart set IsDeleted=false,LastUpdated=@LastUpdate WHERE Id=@CartId";
                await db.ExecuteAsync(update, new {CartId = cartId, LastUpdate = DateTime.UtcNow});
            }
        }

        #region MyRegion

        public async Task<Cart> GetCartAsync(int id)
        {
            using var db = GetConnection();
            return (await db.QueryAsync<Cart>("SELECT * FROM Cart where Id=@Id", new {Id = id})).FirstOrDefault();
        }

        public async Task<IEnumerable<Cart>> GetCartsAsync()
        {
            using var db = GetConnection();
            return (await db.QueryAsync<Cart>("SELECT * FROM Cart")).ToArray();
        }

        #endregion
    }
}