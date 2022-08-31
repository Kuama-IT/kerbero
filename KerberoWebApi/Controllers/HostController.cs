using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace KerberoWebApi.Controllers
{
    // TODO: implement applcation authentication
    [ApiController]
    [Route("[controller]")]
    public class HostController : Controller
    {
        [HttpGet]
        public bool IsAuthenticated()
        {
            return true;
        }


        [HttpPost]
        [Route("auth")]
        public string Login()
        {
            return "This is the Welcome action method...";
        }
    }
}