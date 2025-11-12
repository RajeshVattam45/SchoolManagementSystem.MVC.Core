using FastEndpoints;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.StudentClass
{
    public class AddClassEndpoint : Endpoint<Class>
    {
        private readonly IClassService _classService;

        public AddClassEndpoint ( IClassService classService )
        {
            _classService = classService;
        }

        public override void Configure ( )
        {
            Post ( "api/classes" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( Class cls, CancellationToken ct )
        {
            if (cls == null)
            {
                await SendNotFoundAsync (  );
                return;
            }

            _classService.AddClass ( cls );
            await SendCreatedAtAsync<GetClassByIdEndpoint> ( new { id = cls.Id }, cls );
        }
    }
}
