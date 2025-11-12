using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ContactUsApi : Controller
    {
        public readonly IContactUsFormServices _services;

        public ContactUsApi ( IContactUsFormServices services )
        {
            _services = services;
        }

        [HttpPost ( "api/contact" )]
        public async Task<IActionResult> SubmitContact ( ContactRequestDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest ();

            await _services.SubmitContactAsync ( dto );
            return Ok ( new { message = "Contact request submitted successfully" } );
        }

        [HttpGet ( "api/get-contact-list" )]
        public async Task<IActionResult> GetContactUsData ( )
        {
            if (!ModelState.IsValid)
                return BadRequest ();

            var result = await _services.GetAllContactsAsync ();
            return Ok ( result);
        }
    }
}
