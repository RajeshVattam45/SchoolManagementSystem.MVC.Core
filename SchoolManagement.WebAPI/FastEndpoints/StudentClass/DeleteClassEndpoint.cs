using FastEndpoints;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.StudentClass
{
    public class DeleteClassEndpoint : Endpoint<DeleteClassRequest>
    {
        private readonly IClassService _classService;

        public DeleteClassEndpoint ( IClassService classService )
        {
            _classService = classService;
        }

        public override void Configure ( )
        {
            Delete ( "api/classes/{id:int}" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( DeleteClassRequest req, CancellationToken ct )
        {
            var existing = _classService.GetClassById ( req.Id );
            if (existing == null)
            {
                await SendNotFoundAsync ( );
                return;
            }

            _classService.DeleteClass ( req.Id );
            await SendNoContentAsync ();
        }
    }

    public class DeleteClassRequest
    {
        public int Id { get; set; }
    }

}
