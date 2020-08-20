using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CartRepository;
using CartRepository.Models;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReportWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IReportWriter _reportWriter;
        private readonly IBaseRepository _baseRepository;

        public Worker(ILogger<Worker> logger, IReportWriter reportWriter, IBaseRepository baseRepository)
        {
            _logger = logger;
            _reportWriter = reportWriter;
            _baseRepository = baseRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            try
            {
                var carts = new Dictionary<Cart, List<CartProduct>>();
                using var db = _baseRepository.GetConnection();
                var sql = "SELECT c.*, cp.* FROM Cart c join CartProduct cp on cp.CartId=c.Id  where IsDeleted=false";
                (await db.QueryAsync<Cart, CartProduct, Cart>(
                    sql,
                    (c, cp) =>
                    {
                        if (!carts.ContainsKey(c))
                            carts.Add(c, new List<CartProduct>());
                        carts[c].Add(cp);
                        return c;
                    })).AsEnumerable();

                var sb = new StringBuilder();
                sb.AppendLine($"на {DateTime.UtcNow}");
                sb.AppendLine($"всего корзин {carts.Keys.Count}");
                if (carts.Keys.Count > 0)
                {
                    sb.AppendLine(
                        $"из них содержат продукты за баллы {carts.Count(x => x.Value.Any(y => y.IsBonusPoints))}");
                    sb.AppendLine(
                        $"{carts.Count(x => x.Key.LastUpdated <= DateTime.Now.AddDays(-20))} корзин истечет в течение 10 дней");
                    sb.AppendLine(
                        $"{carts.Count(x => x.Key.LastUpdated <= DateTime.Now.AddDays(-10))} корзин истечет в течение 20 дней");
                    sb.AppendLine(
                        $"{carts.Count(x => x.Key.LastUpdated <= DateTime.Now)} корзин истечет в течение 30 дней");
                    sb.AppendLine(
                        $"средний чек корзины {carts.SelectMany(x => x.Value).Sum(x => x.Count * x.Cost) / carts.Keys.Count}");
                }

                _reportWriter.Save(sb.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(null, e);
                //todo notify
            }
        }
    }
}