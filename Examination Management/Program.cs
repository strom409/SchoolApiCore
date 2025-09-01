using Examination_Management.Repository.Error;
using Examination_Management.Services;
using Examination_Management.Services.ExamGrades;
using Examination_Management.Services.Marks;
using Examination_Management.Services.MarksSheetSetting;
using Examination_Management.Services.MaxMarks;
using Examination_Management.Services.OptionalMarks;
using Examination_Management.Services.OptionalMaxMarks;
using Examination_Management.Services.Result;
using Examination_Management.Services.Result.Gazet;
using Examination_Management.Services.Units;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitsService, UnitsService>();
builder.Services.AddScoped<IMaxMarksService, MaxMarksService>();
builder.Services.AddScoped<IMarksService, MarksService>();
builder.Services.AddScoped<IOptionalMarksService, OptionalMarksService>();
builder.Services.AddScoped<IOptionalMaxMarksService, OptionalMaxMarksService>();
builder.Services.AddScoped<IMarksSheetSettingService, MarksSheetSettingService>();
builder.Services.AddScoped<IExamGradesService, ExamGradesService>();
builder.Services.AddScoped<IStudentResultsService, StudentResultsService>();
builder.Services.AddScoped<IGazetService, GazetService>();

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
