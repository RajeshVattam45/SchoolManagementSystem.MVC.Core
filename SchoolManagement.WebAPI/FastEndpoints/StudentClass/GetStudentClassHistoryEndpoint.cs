using FastEndpoints;
using Newtonsoft.Json;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.StudentClass
{
    public class GetStudentClassHistoryEndpoint : Endpoint<GetStudentClassHistoryRequest>
    {
        private readonly IStudentClassHistoryService _studentClassHistoryService;

        public GetStudentClassHistoryEndpoint ( IStudentClassHistoryService service )
        {
            _studentClassHistoryService = service;
        }

        public override void Configure ( )
        {
            Get ( "api/classes/student/{studentId:int}/class-history" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( GetStudentClassHistoryRequest req, CancellationToken ct )
        {
            var history = await _studentClassHistoryService.GetClassHistoryAsync ( req.StudentId );

            if (history == null || !history.Any ())
            {
                await SendNotFoundAsync ( );
                return;
            }

            var json = JsonConvert.SerializeObject ( history, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            } );

            await SendAsync ( json, 200, cancellation: ct );
        }
    }

    public class GetStudentClassHistoryRequest
    {
        public int StudentId { get; set; }
    }
}
