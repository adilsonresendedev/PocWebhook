using Microsoft.AspNetCore.Mvc;
using PocWebhook.Shared;

namespace PocWebhook.ApiCRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrmController : ControllerBase
    {
        [HttpPost(nameof(AddUserToEmailList))]
        public async Task<IActionResult> AddUserToEmailList([FromBody] UserDTO userDTO)
        {
            Console.WriteLine($"User {userDTO.FirstName} {userDTO.LastName}, Email: {userDTO.Email} added to email list.");
            return Ok(userDTO);
        }
    }
}
