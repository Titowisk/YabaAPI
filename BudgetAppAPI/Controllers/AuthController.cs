using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yaba.Application.UserServices;
using Yaba.Infrastructure.DTO;

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
        public async Task<IActionResult> SignIn(UserSignInDTO dto)
        {
            try
            {
                await _userService.UserSignIn(dto);

                return Ok();
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, "Message: {0}", aex.Message);
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message: {0}", ex.Message);
                return StatusCode(500);
            }
        }
    }
    // TODO: Add google auth
}
