using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route ( "api/[controller]" )]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController ( IEventService eventService )
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var events = await _eventService.GetAllEventsAsync ();
            return Ok ( events );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var ev = await _eventService.GetEventByIdAsync ( id );
            return ev == null ? NotFound () : Ok ( ev );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( Events ev )
        {
            await _eventService.AddEventAsync ( ev );
            return CreatedAtAction ( nameof ( GetById ), new { id = ev.Id }, ev );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, Events ev )
        {
            if (id != ev.Id)
                return BadRequest ();

            await _eventService.UpdateEventAsync ( ev );
            return NoContent ();
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            await _eventService.DeleteEventAsync ( id );
            return NoContent ();
        }
    }
}
