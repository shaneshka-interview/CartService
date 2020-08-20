using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CartRepository;
using CartRepository.Models;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExpireWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBaseRepository _baseRepository;
        private readonly ICartsRepository _cartsRepository;

        public Worker(ILogger<Worker> logger, IBaseRepository baseRepository, ICartsRepository cartsRepository)
        {
            _logger = logger;
            _baseRepository = baseRepository;
            _cartsRepository = cartsRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using var db = _baseRepository.GetConnection();
            var sql = "SELECT * FROM Cart where IsDeleted=false and LastUpdated < @LastUpdated";
            var cartTasks = (await db.QueryAsync<Cart>(sql,
                    new {LastUpdated = DateTime.UtcNow.AddDays(-Settings.Settings.ExpireDays)}))
                .Select(cart => Task.Run(async () =>
                {
                    _logger.LogInformation($"expire cartId:[{cart.Id}]");
                    try
                    {
                        await _cartsRepository.DeleteAsync(cart.Id);
                        _logger.LogInformation($"delete cartId:[{cart.Id}]");
                        await StartHooksAsync(cart.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(null, e);
                    }
                }, stoppingToken));
            await Task.WhenAll(cartTasks);
        }

        private async Task StartHooksAsync(int cartId)
        {
            using var db = _baseRepository.GetConnection();
            var sql = "SELECT * FROM CartHook where CartId=@CartId";
            var urls = (await db.QueryAsync<CartHook>(sql, new {CartId = cartId})).ToArray();
            var httpClient = new HttpClient();
            foreach (var cartHook in urls)
            {
                try
                {
                    await httpClient.GetAsync(cartHook.Url);
                    _logger.LogInformation($"send {cartHook.Url}");
                }
                catch (Exception e)
                {
                    _logger.LogError($"error hookId:[{cartHook.Id}] to url:[{cartHook.Url}]", e);
                }
            }
        }
    }
}