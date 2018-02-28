using DGame.Web.Services.Contracts;
using System.Linq;
using System.Web.Mvc;

namespace DGame.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGameService gameService;

        public HomeController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        public ActionResult Index()
        {
            var topGames = this.gameService.GetTop(3).ToList();

            return View(topGames);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}