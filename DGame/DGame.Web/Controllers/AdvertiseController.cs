using DGame.Data.Repository;
using DGame.Web.Models;
using DGame.Web.Services.Contracts;
using System.IO;
using System.Web.Mvc;

namespace DGame.Web.Controllers
{
    public class AdvertiseController : Controller
    {
        private readonly IAdvertService advertService;

        public AdvertiseController(IAdvertService advertService)
        {
            this.advertService = advertService;
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateAdvertiseViewModel viewModel)
        {
            if (viewModel.File.ContentLength > 0)
            {
                string filename = Path.GetFileName(viewModel.File.FileName);
                string path = Path.Combine(Server.MapPath("~/Temp/Adds"), filename);
                viewModel.File.SaveAs(path);
            }

            this.advertService.Create(viewModel.TransactionHash, User.Identity.Name);

            return RedirectToAction("Index", "Home");
        }
    }
}