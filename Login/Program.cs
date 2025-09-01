
using Login.Services.HT;
using Login.Services.Login;
using Login.Services.Users;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ILoginService, LoginServices>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHTService, HTService>();

// ? Register ErrorBLL itself so it can be resolved


var app = builder.Build();

// ? Configure ErrorBLL with DI container
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
