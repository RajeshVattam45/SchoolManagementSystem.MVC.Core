using FastEndpoints;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.Employees
{
    public class GetEmployeeByIdRequest
    {
        [BindFrom ( "id" )]
        public int Id { get; set; }
    }

    public class GetEmployeeByIdEndpoint : Endpoint<GetEmployeeByIdRequest, Employee>
    {
        private readonly IEmployeeService _employeeService;

        public GetEmployeeByIdEndpoint ( IEmployeeService employeeService )
        {
            _employeeService = employeeService;
        }

        public override void Configure ( )
        {
            Get ( "api/employees/{id}" );
            AllowAnonymous ();
        }

        public override async Task HandleAsync ( GetEmployeeByIdRequest req, CancellationToken ct )
        {
            var employee = await _employeeService.GetEmployeeByIdAsync ( req.Id );
            if (employee is null)
            {
                await SendNotFoundAsync ( ct );
                return;
            }

            await SendOkAsync ( employee, ct );
        }
    }
}
