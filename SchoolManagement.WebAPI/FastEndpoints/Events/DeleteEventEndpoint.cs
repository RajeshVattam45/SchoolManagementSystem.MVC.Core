using FastEndpoints;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.Events
{
    public class DeleteEventEndpoint : Endpoint<DeleteEventRequest>
    {
        private readonly IEventService _eventService;

        public DeleteEventEndpoint ( IEventService eventService )
        {
            _eventService = eventService;
        }

        public override void Configure ( )
        {
            Delete ( "api/school/events/{id}" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( DeleteEventRequest req, CancellationToken ct )
        {
            await _eventService.DeleteEventAsync ( req.Id );
            await SendNoContentAsync ( ct );
        }
    }

    public class DeleteEventRequest
    {
        public int Id { get; set; }
    }
}
