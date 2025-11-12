using FastEndpoints;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.StudentClass
{
    public class GetAllClassesEndpoint : EndpointWithoutRequest
    {
        private readonly IClassService _classService;

        public GetAllClassesEndpoint ( IClassService classService )
        {
            _classService = classService;
        }

        public override void Configure ( )
        {
            Get ( "api/classes" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( CancellationToken ct )
        {
            var classes = _classService.GetAllClasses ();
            if (classes != null && classes.Any ())
            {
                await SendOkAsync ( classes );
            }
            else
            {
                await SendNotFoundAsync ( ct );
            }
        }
    }
}
