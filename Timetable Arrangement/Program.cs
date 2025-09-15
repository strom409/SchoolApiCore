
using Timetable_Arrangement.Repository.Error;
using Timetable_Arrangement.Services.TimeTableArrangements;
using Timetable_Arrangement.Services.TimeTableHistory;
using Timetable_Arrangement.Services.TTAssignPeroids;
using Timetable_Arrangement.Services.TTDays;
using Timetable_Arrangement.Services.TTPeroids;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITimeTableArrangementsServices, TimeTableArrangementsServices>();
builder.Services.AddScoped<ITimeTableHistoryServices, TimeTableHistoryServices>();
builder.Services.AddScoped<ITTAssignPeroidsService, TTAssignPeroidsService>();
builder.Services.AddScoped<ITTDaysService, TTDaysService>();
builder.Services.AddScoped<ITTPeroidService, TTPeroidService>();
builder.Services.AddScoped<ITTPeroidsNoService, TTPeroidsNoService>();
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
