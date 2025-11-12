using FastEndpoints;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.Events
{
    public class UpdateEventEndpoint : Endpoint<SchoolManagement.Core.Entites.Models.Events>
    {
        private readonly IEventService _eventService;

        public UpdateEventEndpoint ( IEventService eventService )
        {
            _eventService = eventService;
        }

        public override void Configure ( )
        {
            Put ( "api/school/events/{id}" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( SchoolManagement.Core.Entites.Models.Events ev, CancellationToken ct )
        {
            if (Route<int> ( "id" ) != ev.Id)
            {
                await SendAsync ( "ID mismatch.", 400, ct );
                return;
            }

            await _eventService.UpdateEventAsync ( ev );
            await SendNoContentAsync ( ct );
        }
    }
}
