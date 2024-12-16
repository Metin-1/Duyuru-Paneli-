using Microsoft.AspNetCore.Mvc;
using Login2.Services;

namespace Login2.Controllers
{
    public class DuyuruController : Controller
    {
        private readonly DuyuruService _duyuruService;

        public DuyuruController(DuyuruService duyuruService)
        {
            _duyuruService = duyuruService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var duyurular = _duyuruService.ReadDuyurular();
            return View(duyurular);
        }

        [HttpPost]
        public IActionResult Guncelle()
        {
            var duyurular = _duyuruService.ReadDuyurular();
            foreach (var duyuru in duyurular["duyurular"])
            {
                var id = (int)duyuru["id"];
                var newText = Request.Form[$"newText_{id}"].ToString();
                if (!string.IsNullOrEmpty(newText))
                {
                    duyuru["text"] = newText;
                }
            }
            _duyuruService.UpdateDuyurular(duyurular);
            return RedirectToAction("Yayin", "Home");
        }
    }
}
