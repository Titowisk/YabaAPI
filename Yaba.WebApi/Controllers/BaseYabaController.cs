using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Yaba.Tools.Validations;

namespace Yaba.WebApi.Controllers
{
    [ApiController]
    public class BaseYabaController : ControllerBase
    {
        protected int GetLoggedUserId()
        {
            // TODO : find a better way to do this? 
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            Validate.IsTrue(!string.IsNullOrWhiteSpace(userId), "Acesso negado");

            return int.Parse(userId);
        }
    }
}
