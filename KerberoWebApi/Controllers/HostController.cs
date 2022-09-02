using KerberoWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace KerberoWebApi.Controllers
{
    // TODO: implement application authentication
    [ApiController]
    [Route("host")]
    public class HostController : Controller
    {
        private readonly HostAuthenticationContext _context;
        public HostController(HostAuthenticationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Register a generic user to db.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<bool> SignUp(Models.Host host)
        {       
            _context.HostList.Add(host);
            await _context.SaveChangesAsync();

            return true;
        }
        
        /// <summary>
        /// It returns the list of vendor accounts the host own.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        [HttpGet("vendor")]
        public async Task<bool> GetVendor(Models.Host host)
        {       
            return true;
        }

        [HttpPost("vendor/add")]
        public async Task<bool> AddVendor(Models.Host host)
        {       
            return true;
        }
    }
}