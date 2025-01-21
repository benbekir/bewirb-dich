using CreepyApi.Database;
using CreepyApi.Middleware;
using CreepyApi.Services;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddDbContext<CreepyApiDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DbConnection"), new MySqlServerVersion("9.1.0")));

builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddExceptionHandler<CreepyApiExceptionHandler>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(options => { });

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
