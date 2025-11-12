using FastEndpoints;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.StudentClass
{
    public class GetClassByIdEndpoint : Endpoint<GetClassByIdRequest>
    {
        private readonly IClassService _classService;

        public GetClassByIdEndpoint ( IClassService classService )
        {
            _classService = classService;
        }

        public override void Configure ( )
        {
            Get ( "api/classes/{id:int}" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( GetClassByIdRequest req, CancellationToken ct )
        {
            var cls = _classService.GetClassById ( req.Id );
            if (cls == null)
            {
                await SendNotFoundAsync ();
                return;
            }

            await SendOkAsync ( cls );
        }
    }

    public class GetClassByIdRequest
    {
        [BindFrom ( "id" )]
        public int Id { get; set; }
    }
}
