using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartRepository.Models;
using Contracts;
using Dapper;

namespace CartRepository
{
    public interface ICartsRepository
    {
        Task AddAsync(CartProduct cartProduct);
        Task UpdateAsync(CartProduct cartProduct);
        Task DeleteAsync(ProductDeleteDto cartProduct);
        Task DeleteAsync(int cartId);
        Task<IEnumerable<CartProduct>> GetProductsAsync(int cartId);
        Task AddHookAsync(CartHook cartHook);

        Task<IEnumerable<Cart>> GetCartsAsync();
    }

    public class CartsRepository : BaseRepository, ICartsRepository
    {
        public CartsRepository(string conn) : base(conn)
        {
        }

        public async Task AddAsync(CartProduct cartProduct)
        {
            await ActivateCartAsync(cartProduct.CartId);
            using var db = GetConnection();
            var sql =
                "Select * from CartProduct where CartId=@CartId and ProductId=@ProductId and IsBonusPoints=@IsBonusPoints";
            var product = (await db.QueryAsync<CartProduct>(sql, cartProduct)).FirstOrDefault();
            if (product == null)
            {
                var sqlQuery =
                    "INSERT INTO CartProduct (CartId, ProductId, Cost, Count, IsBonusPoints) VALUES(@CartId, @ProductId, @Cost, @Count, @IsBonusPoints)";
                await db.ExecuteAsync(sqlQuery, cartProduct);
            }
            else
            {
                await Update(cartProduct);
            }
        }

        public async Task UpdateAsync(CartProduct cartProduct)
        {
            await ActivateCartAsync(cartProduct.CartId);
            await Update(cartProduct);
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
            return await db.QueryAsync<CartProduct>(
                "SELECT * FROM CartProduct cp WHERE cp.CartId = @CartId",
                new {CartId = cartId});
        }

        public async Task AddHookAsync(CartHook cartHook)
        {
            using var db = GetConnection();
            var hook = (await db.QueryAsync<CartHook>("select * from CartHook where CartId=@CartId and Url=@Url", cartHook))
                .FirstOrDefault();
            if (hook == null)
            {
                var insert = "INSERT INTO CartHook (CartId, Url) VALUES(@CartId, @Url)";
                await db.ExecuteAsync(insert, cartHook);
            }
        }

        private async Task Update(CartProduct cartProduct)
        {
            using var db = GetConnection();
            var sqlQuery =
                "UPDATE CartProduct SET Count=@Count, Cost=@Cost WHERE CartId = @CartId and ProductId=@ProductId and IsBonusPoints = @IsBonusPoints";
            await db.ExecuteAsync(sqlQuery, cartProduct);
        }

        private async Task ActivateCartAsync(int cartId)
        {
            using var db = GetConnection();

            var sqlQuery = "Select * from Cart WHERE Id=@CartId";
            var cart = (await db.QueryAsync<Cart>(sqlQuery, new {CartId = cartId})).FirstOrDefault();
            if (cart == null)
            {
                var insert = "INSERT INTO Cart (IsDeleted, LastUpdated) VALUES (false, @LastUpdate)";
                await db.ExecuteAsync(insert, new {LastUpdate = DateTime.Now});
            }
            else
            {
                var update = "Update Cart set IsDeleted=false, LastUpdated=@LastUpdate WHERE Id=@CartId";
                await db.ExecuteAsync(update, new {CartId = cartId, LastUpdate = DateTime.Now});
            }
        }

        #region MyRegion

        public async Task<IEnumerable<Cart>> GetCartsAsync()
        {
            using var db = GetConnection();
            return await db.QueryAsync<Cart>("SELECT * FROM Cart where IsDeleted=false");
        }

        public async Task<Cart> GetCartAsync(int id)
        {
            using var db = GetConnection();
            return (await db.QueryAsync<Cart>("SELECT * FROM Cart where Id=@Id", new {Id = id})).FirstOrDefault();
        }

        #endregion
    }
}