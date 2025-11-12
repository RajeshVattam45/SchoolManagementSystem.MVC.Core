using FastEndpoints;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.Events
{
    public class GetEventByIdRequest
    {
        [BindFrom ( "id" )]
        public int Id { get; set; }
    }

    public class GetEventByIdEndpoint : Endpoint<GetEventByIdRequest>
    {
        private readonly IEventService _eventService;

        public GetEventByIdEndpoint ( IEventService eventService )
        {
            _eventService = eventService;
        }

        public override void Configure ( )
        {
            Get ( "api/school/events/{id}" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( GetEventByIdRequest req, CancellationToken ct )
        {
            var ev = await _eventService.GetEventByIdAsync ( req.Id );
            if (ev == null)
                await SendNotFoundAsync ();
            else
                await SendOkAsync ( ev );
        }
    }
}
