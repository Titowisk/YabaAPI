using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Yaba.Application.UserServices;
using Yaba.Infrastructure.DTO;

namespace Yaba.WebApi.Controllers
{
    [Route("api/auth")]
    public class AuthController : BaseYabaController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;

        public AuthController(
            ILogger<AuthController> logger,
            IUserService userService)
        {
            this._logger = logger;
            this._userService = userService;
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpDTO signUpDto)
        {
            await this._userService.UserSignUp(signUpDto);

            return this.Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            var result = await this._userService.Login(loginDto);

            return this.Ok(result);
        }

        [HttpGet]
        [Route("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var loggedUserId = base.GetLoggedUserId();
            var result = await this._userService.GetCurrentUserById(loggedUserId);

            return this.Ok(result);
        }
    }

    // TODO: Add google oauth
}
