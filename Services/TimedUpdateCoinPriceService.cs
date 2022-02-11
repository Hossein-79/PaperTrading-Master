using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PaperTrading.Services
{
    public class TimedUpdateCoinPriceService : IHostedService, IDisposable
    {

        private Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly double _jobTimer = 30;
        readonly HttpClient _client;


        public TimedUpdateCoinPriceService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            _client = new HttpClient()
            {
                BaseAddress = new Uri("https://api.binance.com/"),
            };
        }

        private async void DoWork(object state)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var coinService = scope.ServiceProvider.GetRequiredService<ICoinService>();
            var tradeService = scope.ServiceProvider.GetRequiredService<ITradeService>();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            var coins = await coinService.GetCoins();
            var usdt = await coinService.GetUsdt();

            foreach (var coin in coins)
            {
                if (coin.Symbol == "USDT")
                    continue;

                try
                {
                    coin.Price = await GetCoinPrice(coin.Symbol + "USDT");
                    await coinService.Update(coin);

                    var solveTradeBuy = await tradeService.GetCoinTradesBuy(coin.CoinId, coin.Price);
                    var solveTradeSell = await tradeService.GetCoinTradesSell(coin.CoinId, coin.Price);

                    foreach (var trade in solveTradeBuy)
                    {
                        var usdtWallet = await walletService.GetUserWallet(trade.UserId, usdt.CoinId);
                        var coinWallet = await walletService.GetUserWallet(trade.UserId, coin.CoinId);

                        if (coinWallet == null)
                        {
                            coinWallet = new Models.Wallet()
                            {
                                Balance = trade.Amount,
                                CoinId = coin.CoinId,
                                UserId = trade.UserId,
                            };
                            await walletService.AddWallet(coinWallet);
                        }
                        else
                        {
                            coinWallet.Balance += trade.Amount;
                            await walletService.Update(coinWallet);
                        }

                        usdtWallet.Balance -= trade.Amount * trade.Price;
                        trade.Status = Models.TradeStatus.Success;

                        await walletService.Update(usdtWallet);
                        await tradeService.Update(trade);
                    }
                    foreach (var trade in solveTradeSell)
                    {
                        var usdtWallet = await walletService.GetUserWallet(trade.UserId, usdt.CoinId);
                        var coinWallet = await walletService.GetUserWallet(trade.UserId, coin.CoinId);

                        usdtWallet.Balance += trade.Amount * trade.Price;
                        coinWallet.Balance -= trade.Amount;
                        trade.Status = Models.TradeStatus.Success;

                        await walletService.Update(usdtWallet);
                        await walletService.Update(coinWallet);
                        await tradeService.Update(trade);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_jobTimer));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public async Task<decimal> GetCoinPrice(string symbol)
        {
            try
            {
                var response = await _client.GetAsync($"api/v3/ticker/price?symbol={symbol.ToUpper()}");
                var content = await response.Content.ReadAsStringAsync();

                var aa = JsonSerializer.Deserialize<CoinPrice>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return decimal.Parse(aa.Price);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private class CoinPrice
        {
            public string Symbol { get; set; }

            public string Price { get; set; }
        }
    }
}
