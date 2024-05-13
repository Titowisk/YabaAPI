using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yaba.Application.UserServices;
using Yaba.Infrastructure.DTO;
using Yaba.Tools.Validations;

namespace Yaba.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;

        public AuthController(
            ILogger<AuthController> logger,
            IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> SignIn([FromBody] UserSignUpDTO dto)
        {
            await _userService.UserSignUp(dto);

            return Ok();
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            var result = await _userService.Login(dto);

            return Ok(result);
        }

        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // TODO : better way to do this? 
            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            Validate.IsTrue(!string.IsNullOrEmpty(user.Value), "Acesso negado");

            // TODO: safer way to do this? (if someone hijacks a valid token, this could be problematic)
            var result = await _userService.GetCurrentUserById(int.Parse(user.Value));

            return Ok(result);
        }
    }
    // TODO: Add google auth
}
