using FastEndpoints;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.Employees
{
    public class CreateEmployeeEndpoint : Endpoint<CreateEmployeeRequest>
    {
        private readonly IEmployeeService _employeeService;

        public CreateEmployeeEndpoint ( IEmployeeService employeeService )
        {
            _employeeService = employeeService;
        }

        public override void Configure ( )
        {
            Post ( "api/employees/create" );
            AllowAnonymous ();
            AllowFileUploads ();
        }

        public override async Task HandleAsync ( CreateEmployeeRequest req, CancellationToken ct )
        {
            var employee = new Employee
            {
                EmployeeId = req.EmployeeId,
                FirstName = req.FirstName,
                LastName = req.LastName,
                PhoneNumber = req.PhoneNumber,
                Role = req.Role,
                PermanentAddress = req.PermanentAddress,
                CurrentAddress = req.CurrentAddress,
                Pincode = req.Pincode,
                EmployeeSalary = req.EmployeeSalary,
                Email = req.Email,
                PasswordHash = req.PasswordHash,
                Gender = req.Gender,
                DateOfJoining = req.DateOfJoining,
            };

            if (req.ProfileImage is not null)
            {
                using var ms = new MemoryStream ();
                await req.ProfileImage.CopyToAsync ( ms, ct );
                employee.ProfileImage = ms.ToArray ();
            }

            await _employeeService.RegisterEmployeeAsync ( employee );

            await SendCreatedAtAsync<GetEmployeeByIdEndpoint> (
                new { id = employee.Id },
                employee,
                cancellation: ct
            );
        }
    }
    public class CreateEmployeeRequest
    {
       public int EmployeeId { get; set; }
        public string FirstName { get; set; }
         public string LastName { get; set; }
      public string PhoneNumber { get; set; }
        public string Role { get; set; }
         public string PermanentAddress { get; set; }
        public string CurrentAddress { get; set; }
        public string Pincode { get; set; }
         public decimal EmployeeSalary { get; set; }
         public string Email { get; set; }
       public string PasswordHash { get; set; }
         public string Gender { get; set; }
        public DateTime DateOfJoining { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
