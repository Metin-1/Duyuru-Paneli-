using Login2.Models;
using Login2.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.AccessControl;

namespace Login2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DuyuruService _duyuruService;

        public HomeController(ILogger<HomeController> logger, DuyuruService duyuruService)
        {
            _logger = logger;
            _duyuruService = duyuruService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Giris(IFormCollection fc)
        {
            var kadi = fc["kadi"];
            var sifre = fc["sifre"];

            var pkadi = "meto";
            var psifre = "1234";

            if (kadi == pkadi && sifre == psifre)
            {
                HttpContext.Session.SetString("yetki", kadi);
                return RedirectToAction("Duyuru", "Home");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Duyuru()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("yetki")))
            {
                return RedirectToAction("Giris");
            }

            var duyurular = _duyuruService.ReadDuyurular();
            var model = new DuyuruModel
            {
                Textboxes = duyurular["duyurular"].Select(d => (string)d["text"]).ToList(),
                CheckBoxes = new List<bool>(new bool[duyurular["duyurular"].Count()]),
                Ids = duyurular["duyurular"].Select(d => (int)d["id"]).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Guncelle(DuyuruModel model)
        {
            if (model.Textboxes == null || model.CheckBoxes == null || model.Ids == null)
            {
                return BadRequest("Textboxes, CheckBoxes veya Ids null olamaz.");
            }

            if (model.Textboxes.Count != model.CheckBoxes.Count || model.Textboxes.Count != model.Ids.Count)
            {
                return BadRequest("Textboxes, CheckBoxes ve Ids listelerinin uzunluklarý eþleþmiyor.");
            }

            var duyurular = _duyuruService.ReadDuyurular();

            for (int i = 0; i < model.Ids.Count; i++)
            {
                var id = model.Ids[i];
                var duyuru = duyurular["duyurular"].FirstOrDefault(d => (int)d["id"] == id);
                if (duyuru != null)
                {
                    if (duyuru["text"].ToString() != model.Textboxes[i])
                    {
                        duyuru["updatedAt"] = DateTime.UtcNow.ToString("o"); // Güncelleme zamanýný ayarla
                    }
                    duyuru["text"] = model.Textboxes[i];
                    duyuru["isChecked"] = model.CheckBoxes[i];
                }
            }

            try
            {
                _duyuruService.UpdateDuyurular(duyurular);
                Console.WriteLine("JSON dosyasý baþarýyla güncellendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("JSON dosyasý güncellenirken hata oluþtu: " + ex.Message);
                return StatusCode(500, "Güncelleme sýrasýnda bir hata oluþtu.");
            }

            return Ok(new { message = "Veriler baþarýyla güncellendi." });
        }




        [HttpGet]
        public IActionResult Yayýn()
        {
            var duyurular = _duyuruService.ReadDuyurular();
            var duyuruList = duyurular["duyurular"]
                .Where(d => (bool)d["isChecked"])
                .OrderByDescending(d => {
                    // Tarih formatýný doðru parse etmek için
                    DateTime parsedDate;
                    var dateString = (string)d["updatedAt"];
                    if (!DateTime.TryParse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsedDate))
                    {
                        // Tarih parse edilemezse varsayýlan tarih olarak minimum tarih kullan
                        parsedDate = DateTime.MinValue;
                    }
                    return parsedDate;
                })
                .Select(d => new Duyuru
                {
                    Id = (int)d["id"],
                    Text = (string)d["text"],
                    UpdatedAt = DateTime.ParseExact((string)d["updatedAt"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    IsChecked = (bool)d["isChecked"]
                }).ToList();

            DateTime? lastDate = DateTime.MinValue;
            var lastNotification = duyuruList[0];
           
            foreach (var item in duyuruList)
            {
                //tarihleri kontrol edip aktarma iþlemi yapýlýyor
                if (item.UpdatedAt >= lastDate)
                {
                    //Son tarih listedeki elemandan küçükse son duyuruyu güncelliyoruz
                    lastDate = item.UpdatedAt;
                    lastNotification = item; //Son duyuru objesi

                }
            }
            if(duyuruList.Contains(lastNotification))
            {
                duyuruList.Remove(lastNotification);
                duyuruList.Insert(0, lastNotification);
            }
            var model = new DuyuruModel
            {
                Textboxes = duyuruList.Select(d => d.Text).ToList(),
                CheckBoxes = duyuruList.Select(d => d.IsChecked).ToList(),
                Ids = duyuruList.Select(d => d.Id).ToList(),
            };
  
            return View(model);
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
