using DGame.Data.Repository;
using DGame.Web.Models;
using DGame.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
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
            string filename = string.Empty;
            if (viewModel.File.ContentLength > 0)
            {
                filename = $"{Guid.NewGuid().ToString()}.{Path.GetExtension(viewModel.File.FileName)}";
                string path = Path.Combine(Server.MapPath("~/Temp/Adds"), filename);
                viewModel.File.SaveAs(path);
            }

            this.advertService.Create(viewModel.TransactionHash, User.Identity.Name, filename, viewModel.Link);

            return RedirectToAction("Index", "Home");
        }

        public FileResult GetAdvertImage(string filename)
        {
            var filepath = Path.Combine(Server.MapPath("~/Temp/Adds"), filename);

            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }

        public ActionResult Render()
        {
            var adverts = this.advertService.GetAll().ToList();
            List<Tuple<string, string>> filenames = new List<Tuple<string, string>>();

            foreach (var advert in adverts)
            {
                if (this.advertService.IsAdvertPayed(advert.TransactionHash))
                {
                    filenames.Add(new Tuple<string, string>(advert.FileName, advert.Link));
                }
            }

            return PartialView(filenames);
        }
    }
}