using DGame.Web.Models;
using DGame.Web.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DGame.Web.Controllers
{
    public class GameController : Controller
    {
        private readonly IStorageService storageService;
        private readonly IGameService gameService;

        public GameController(IStorageService storageService, IGameService gameService)
        {
            this.storageService = storageService;
            this.gameService = gameService;
        }

        [Authorize]
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(CreateGameViewModel viewModel)
        {
            this.storageService.TempStoragePath = Server.MapPath("~/Temp");

            try
            {
                if (viewModel.File.ContentLength > 0)
                {
                    string filename = Path.GetFileName(viewModel.File.FileName);
                    string path = Path.Combine(Server.MapPath("~/Temp"), filename);
                    viewModel.File.SaveAs(path);
                    this.storageService.SaveFile(filename, path);
                }

                this.gameService.Create(viewModel.Name, viewModel.Description, User.Identity.Name);

                ViewBag.Message = "File Uploaded Successfully!!";
                
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Play(Guid id)
        {
            var game = this.gameService.Get(id);
            string gameFolder = $"{Server.MapPath("~/Temp/Extract")}/{game.Name}";

            if (!game.Views.Any(v => v.UserName == User.Identity.Name))
            {
                this.gameService.AddView(game.Id, User.Identity.Name);
            }

            DirectoryInfo dir = new DirectoryInfo(gameFolder);
            List<string> scripts = dir.GetFiles("*.js").Select(f => f.Name).ToList();

            var viewModel = new GameRenderViewModel()
            {
                Name = game.Name,
                Scripts = scripts
            };

            return View(viewModel);
        }

        public FileResult GetFile(string gameName, string filename)
        {
            var gameFolder = Path.Combine(Server.MapPath("~/Temp/Extract"), gameName);

            if (!Directory.Exists(gameFolder))
            {
                var zipGameFile = Path.Combine(Server.MapPath("~/Temp"), $"{gameName}.zip");

                if (System.IO.File.Exists(zipGameFile))
                {
                    string extractPath = Server.MapPath("~/Temp/Extract");
                    ZipFile.ExtractToDirectory(zipGameFile, extractPath);
                }
                else
                {
                    Thread thread = new Thread(storageService.StartFileListener);
                    thread.Start();

                    this.storageService.GetFile($"{gameName}.zip");

                    thread.Abort();

                    string extractPath = Server.MapPath("~/Temp/Extract");
                    ZipFile.ExtractToDirectory(zipGameFile, extractPath);
                }
            }

            string filepath = $"{gameFolder}/{filename}";

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
    }
}