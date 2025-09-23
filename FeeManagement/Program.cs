using FeeManagement.Repository.Error;
using FeeManagement.Services.FeeHead;
using FeeManagement.Services.FeeStructure;
using Microsoft.Extensions.FileProviders;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IFeeHeadService, FeeHeadService>();
builder.Services.AddScoped<IFeeStructureService, FeeStructureService>();

// ? Register ErrorBLL itself so it can be resolved
builder.Services.AddScoped<ErrorBLL>();

var app = builder.Build();
app.UseStaticFiles();

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
