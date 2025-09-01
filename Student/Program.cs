using Student.Services.Students;
using Student.Repository.Error;
using Student.Services.ClassMaster;
using Student.Services.District;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICrescentStudentServics, CrescentStudentServics>();
builder.Services.AddScoped<IClassMasterService, ClassMasterService>();
builder.Services.AddScoped<IDistrictService, DistrictService>();


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
