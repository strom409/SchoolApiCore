
using HR.Repository.Error;
using HR.Services;
using HR.Services.Deparments;
using HR.Services.Designation;
using HR.Services.EmployeeAttendance;
using HR.Services.EmpStatus;
using HR.Services.GrossPay;
using HR.Services.PayRoll;
using HR.Services.Qualifications;
using HR.Services.Salary;
using HR.Services.SalaryPayment;
using HR.Services.Subjects;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ISalaryService, SalaryService>();
builder.Services.AddScoped<ISalaryPaymentService, SalaryPaymentService>();
builder.Services.AddScoped<IDesignationServices, DesignationServices>();
builder.Services.AddScoped<IDepartmentsService, DepartmentsService>();
builder.Services.AddScoped<IPayRollService, PayRollService>();
builder.Services.AddScoped<IGrossPayService, GrossPayService>();
builder.Services.AddScoped<IEmpStatusService, EmpStatusService>();
builder.Services.AddScoped<IQualificationsService, QualificationsService>();
builder.Services.AddScoped<IEmployeeSubjectsService, EmployeeSubjectsService>();
builder.Services.AddScoped<IEmployeeAttendanceService, EmployeeAttendanceService>();
// ? Register ErrorBLL itself so it can be resolved
builder.Services.AddScoped<ErrorBLL>();

var app = builder.Build();

// ? Configure ErrorBLL with DI container
ErrorBLL.Configure(app.Services);

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
        c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "NIT Services Student Api");
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
