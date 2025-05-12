using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Application.Services;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Services;
using SchoolManagementSystem.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder ( args );

var connectionString = builder.Configuration.GetConnectionString ( "DefaultConnection" );
builder.Services.AddDbContext<SchoolDbContext> ( options =>
    options.UseSqlServer ( connectionString, b => b.MigrationsAssembly ( "SchoolManagement.Infrastructure" ) ) );

// JWT Settings
var jwtSettings = builder.Configuration.GetSection ( "Jwt" );
var key = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

// Authentication
builder.Services.AddAuthentication ( "Bearer" )
    .AddJwtBearer ( "Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey ( Encoding.UTF8.GetBytes ( key ) ),
            RoleClaimType = ClaimTypes.Role
        };
    } );

builder.Services.AddAuthorization ();

// Add services to the container.
builder.Services.AddScoped<IStudentRepository, StudentRepository> ();
builder.Services.AddScoped<IStudentService, StudentService> ();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository> ();
builder.Services.AddScoped<IEmployeeService, EmployeeService> ();

builder.Services.AddScoped<IClassRepository, ClassRepository> ();
builder.Services.AddScoped<IClassService, ClassService> ();

builder.Services.AddScoped<ISubjectRepository, SubjectRepository> ();
builder.Services.AddScoped<ISubjectService, SubjectService> ();

builder.Services.AddScoped<IExamRepository, ExamRepository> ();
builder.Services.AddScoped<IExamService, ExamService> ();

builder.Services.AddScoped<IExamTypeRepository, ExamTypeRepository> ();
builder.Services.AddScoped<IExamTypeService, ExamTypeService> ();

builder.Services.AddScoped<IExamSubjectRepository, ExamSubjectRepository> ();
builder.Services.AddScoped<IExamSubjectService, ExamSubjectService> ();
// Register repository and service
builder.Services.AddScoped<IExamScheduleRepository, ExamScheduleRepository> ();
builder.Services.AddScoped<IExamScheduleService, ExamScheduleService> ();

builder.Services.AddScoped<IClassSubjectTeacherRepository, ClassSubjectTeacherRepository> ();
builder.Services.AddScoped<IClassSubjectTeachersService, ClassSubjectTeachersService> ();

builder.Services.AddScoped<IMarksRepository, MarksRepository> ();
builder.Services.AddScoped<IMarksService, MarksService> ();

builder.Services.AddScoped<IStudentAttendanceRepository, StudentAttendanceRepository> ();
builder.Services.AddScoped<IStudentAttendanceService, StudentAttendanceService> ();

builder.Services.AddScoped<IEmployeeAttendanceRepository, EmployeeAttendanceRepository> ();
builder.Services.AddScoped<IEmployeeAttendanceService, EmployeeAttendanceService> ();

builder.Services.AddScoped<IClassSubjectRepository, ClassSubjectRepository> ();
builder.Services.AddScoped<IClassSubjectService, ClassSubjectService> ();

builder.Services.AddScoped<IStudentClassHistoryRepository, StudentClassHistoryRepository> ();
builder.Services.AddScoped<IStudentClassHistoryService, StudentClassHistoryService> ();

builder.Services.AddScoped ( typeof ( IGenericRepository<> ), typeof ( GenericRepository<> ) );
builder.Services.AddScoped<IEventService, EventService> ();


builder.Services.AddControllers ();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen ( c =>
{
    c.SwaggerDoc ( "v1", new OpenApiInfo { Title = "School API", Version = "v1" } );

    // ?? Define JWT Security Scheme
    c.AddSecurityDefinition ( "Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJI...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    } );

    // ?? Apply the scheme globally to all operations
    c.AddSecurityRequirement ( new OpenApiSecurityRequirement ()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    } );
} );


var app = builder.Build ();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment ())
{
    app.UseSwagger ();
    app.UseSwaggerUI ();
}

app.UseHttpsRedirection ();
app.UseAuthentication ();
app.UseAuthorization ();

app.MapControllers ();

app.Run ();
