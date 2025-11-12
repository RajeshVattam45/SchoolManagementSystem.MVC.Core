using FastEndpoints;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.StudentClass
{
    public class UpdateClassEndpoint : Endpoint<Class>
    {
        private readonly IClassService _classService;

        public UpdateClassEndpoint ( IClassService classService )
        {
            _classService = classService;
        }

        public override void Configure ( )
        {
            Put ( "api/classes/{id:int}" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( Class cls, CancellationToken ct )
        {
            var existing = _classService.GetClassById ( cls.Id );
            if (existing == null)
            {
                await SendNotFoundAsync ();
                return;
            }

            _classService.UpdateClass ( cls );
            await SendAsync ( new { message = "Class updated successfully." }, 200 );
        }
    }

    public class UpdateClassRequest
    {
        [BindFrom ( "id" )]
        public int Id { get; set; }
        public Class Class { get; set; }
    }
}
