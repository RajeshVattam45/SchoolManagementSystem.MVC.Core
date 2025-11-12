using FastEndpoints;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.Events
{
    public class CreateEventEndpoint : Endpoint<SchoolManagement.Core.Entites.Models.Events>
    {
        private readonly IEventService _eventService;

        public CreateEventEndpoint ( IEventService eventService )
        {
            _eventService = eventService;
        }

        public override void Configure ( )
        {
            Post ( "api/school/events" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( SchoolManagement.Core.Entites.Models.Events ev, CancellationToken ct )
        {
            await _eventService.AddEventAsync ( ev );
            await SendCreatedAtAsync<GetEventByIdEndpoint> ( new { id = ev.Id }, ev );
        }
    }
}
