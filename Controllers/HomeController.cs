using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaperTrading.Models;
using PaperTrading.Services;
using PaperTrading.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PaperTrading.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly ICoinService _coinService;
        private readonly IPNLService _pNLService;
        private readonly ITradeService _tradeService;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IWalletService walletService, ICoinService coinService, IPNLService pNLService, ITradeService tradeService)
        {
            _logger = logger;

            _userService = userService;
            _walletService = walletService;
            _coinService = coinService;
            _pNLService = pNLService;
            _tradeService = tradeService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Wallet));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string name, string password)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Wallet));
            }

            var user = await _userService.GetUser(name);

            if (name == null || password == null)
            {
                ViewBag.Errors = "Please enter username and password.";
                return View();
            }

            if (user == null)
            {
                user = new User()
                {
                    Name = name,
                    Password = password.Sha256(),
                    Wallets = new List<Wallet>()
                    {
                        new Wallet()
                        {
                            Balance = 1000,
                            CoinId = (await _coinService.GetUsdt()).CoinId,
                        }
                    }
                };
                await _userService.AddUser(user);
            }

            if (user.Password != password.Sha256())
            {
                ViewBag.Errors = "incorrect password";
                return View();
            }

            var identity = new ClaimsIdentity(new[]{
                    new Claim(ClaimTypes.Name, user.Name),
                }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddYears(1)
            });

            return RedirectToAction(nameof(Wallet));
        }

        [Authorize]
        public async Task<IActionResult> Wallet()
        {
            var user = await _userService.GetUser(User.Identity.Name);

            var coins = await _coinService.GetCoins();

            decimal total = 0;
            foreach (var coin in coins)
            {
                var wallet = await _walletService.GetUserWallet(user.UserId, coin.CoinId);
                if (wallet != null)
                {
                    coin.UserBalance = wallet.Balance;
                    total += coin.UserBalance * coin.Price;
                }
            }

            coins = coins.OrderByDescending(u => u.Price * u.UserBalance);

            ViewBag.TotalBalance = total;

            return View(coins);
        }


        [Authorize]
        public async Task<IActionResult> Trade(string id)
        {
            var user = await _userService.GetUser(User.Identity.Name);

            var model = new TradeViewModel();

            model.Usdt = await _coinService.GetUsdt();
            model.Token = await _coinService.GetCoin(id);
            model.Trade = null;

            if (model.Token == null || model.Usdt == null)
                return RedirectToAction(nameof(Wallet));

            var usdtWallet = await _walletService.GetUserWallet(user.UserId, model.Usdt.CoinId);
            var coinWallet = await _walletService.GetUserWallet(user.UserId, model.Token.CoinId);

            model.Usdt.UserBalance = usdtWallet != null ? usdtWallet.Balance : 0;
            model.Token.UserBalance = coinWallet != null ? coinWallet.Balance : 0;

            var UserTrades = await _tradeService.GetUserTrades(user.UserId);
            model.OpenTrades = UserTrades.Where(u => u.Status == TradeStatus.Open).ToList();
            model.SuccessTrade = UserTrades.Where(u => u.Status == TradeStatus.Success).ToList();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Trade(string id, TradeViewModel model)
        {
            var user = await _userService.GetUser(User.Identity.Name);

            model.Usdt = await _coinService.GetUsdt();
            model.Token = await _coinService.GetCoin(id);

            if (model.Token == null || model.Usdt == null)
                return RedirectToAction(nameof(Wallet));

            var usdtWallet = await _walletService.GetUserWallet(user.UserId, model.Usdt.CoinId);
            var coinWallet = await _walletService.GetUserWallet(user.UserId, model.Token.CoinId);

            model.Usdt.UserBalance = usdtWallet != null ? usdtWallet.Balance : 0;
            model.Token.UserBalance = coinWallet != null ? coinWallet.Balance : 0;

            var trade = new Trade()
            {
                CoinId = model.Token.CoinId,
                UserId = user.UserId,
                Price = model.Trade.Price,
                Amount = model.Trade.Amount,
                Side = model.Trade.Side,
                Status = TradeStatus.Open,
            };


            var UserTrades = await _tradeService.GetUserTrades(user.UserId);
            model.OpenTrades = UserTrades.Where(u => u.Status == TradeStatus.Open).ToList();
            model.SuccessTrade = UserTrades.Where(u => u.Status == TradeStatus.Success).ToList();

            if (model.Trade.Side == TradeSide.Buy)
            {
                if (model.Trade.Amount * model.Trade.Price > (model.Usdt.UserBalance - model.OpenTrades.Where(u => u.Side == TradeSide.Buy).Sum(u => u.Amount * u.Price)))
                {
                    ViewBag.Errors = "Not Enough Balance";

                    return View(model);
                }
                if (model.Trade.Price > model.Token.Price)
                {
                    trade.Price = model.Token.Price;
                    trade.Status = TradeStatus.Success;

                    if (coinWallet == null)
                    {
                        coinWallet = new Models.Wallet()
                        {
                            Balance = trade.Amount,
                            CoinId = model.Token.CoinId,
                            UserId = trade.UserId,
                        };
                        await _walletService.AddWallet(coinWallet);
                    }
                    else
                    {
                        coinWallet.Balance += trade.Amount;
                        await _walletService.Update(coinWallet);
                    }
                    model.Token.UserBalance += trade.Amount;

                    usdtWallet.Balance -= trade.Amount * trade.Price;
                    model.Usdt.UserBalance -= trade.Amount * trade.Price;

                    await _walletService.Update(usdtWallet);

                }
            }
            else
            {
                if (model.Trade.Amount > (model.Token.UserBalance - model.OpenTrades.Where(u => u.Side == TradeSide.Sell).Sum(u => u.Amount)))
                {
                    ViewBag.Errors = "Not Enough Balance";
                    return View(model);
                }
                if (model.Trade.Price < model.Token.Price)
                {
                    trade.Status = TradeStatus.Success;
                    trade.Price = model.Token.Price;

                    usdtWallet.Balance += trade.Amount * trade.Price;
                    model.Usdt.UserBalance += trade.Amount * trade.Price;
                    coinWallet.Balance -= trade.Amount;
                    model.Token.UserBalance -= trade.Amount;

                    await _walletService.Update(usdtWallet);
                    await _walletService.Update(coinWallet);
                }
            }

            await _tradeService.AddTrade(trade);

            ViewBag.Success = true;

            UserTrades = await _tradeService.GetUserTrades(user.UserId);
            model.OpenTrades = UserTrades.Where(u => u.Status == TradeStatus.Open).ToList();
            model.SuccessTrade = UserTrades.Where(u => u.Status == TradeStatus.Success).ToList();

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> CancelTrade(int id)
        {
            var user = await _userService.GetUser(User.Identity.Name);

            var trade = await _tradeService.GetTrade(id);
            if (trade == null || trade.UserId != user.UserId)
            {
                return Json(false);
            }

            trade.Status = TradeStatus.Cancel;
            await _tradeService.Update(trade);

            return Json(true);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

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
}
