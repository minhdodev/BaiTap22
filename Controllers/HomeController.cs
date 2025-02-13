using System.Diagnostics;
using Bai22.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Bai22.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        // Constructor chỉ nhận một lần tất cả dependency
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new CircleViewModel()); // Trả về View với model rỗng
        }

        [HttpPost]
        public async Task<IActionResult> Index(CircleViewModel model)
        {
            if (model == null || model.rr <= 0)
            {
                ModelState.AddModelError("rr", "Bán kính phải lớn hơn 0");
                return View(model);
            }

            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7131/api/huy/Circle/cv_dt?rr={model.rr}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(json);

                if (data != null)
                {
                    model.DienTich = data.dien_tich;
                    model.ChuVi = data.chu_vi;
                    model.DuongKinh = data.duong_kinh;
                }
            }
            else
            {
                ModelState.AddModelError("", "Lỗi khi gọi API");
            }

            return View(model);
        }
        [HttpGet("api/huy/Circle/cv_dt")]
        public IActionResult GetCircleInfo(double rr)
        {
            if (rr <= 0)
            {
                return BadRequest(new { error = "Bán kính phải lớn hơn 0" });
            }

            var result = new
            {
                dien_tich = Math.PI * rr * rr,
                chu_vi = 2 * Math.PI * rr,
                duong_kinh = 2 * rr
            };

            return Ok(result);
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
