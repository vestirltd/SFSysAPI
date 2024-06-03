using Microsoft.AspNetCore.Mvc;
using SFSysAPI.Interfaces;
using SFSysAPI.Models;

namespace SFSysAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController(ILogger<ContactsController> logger, IContactsService contactsService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetContacts(){
            logger.LogInformation("Request received to get contact details");
            List<SendContact> sendContacts = await contactsService.GetContacts();
            return Ok(sendContacts);
        }
    }
}