using Microsoft.EntityFrameworkCore;
using Serilog;
using TicketBooking.API.Middlewares;
using TicketBooking.Application.BackgroundServices;
using TicketBooking.Application.Services;
using TicketBooking.Core.Interfaces;
using TicketBooking.Infrastructure.Data;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TicketDB")));

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddSingleton<IBackgroundBookingQueue, BackgroundBookingQueue>();
builder.Services.AddHostedService<BackgroundBookingWorker>();

//Redis cache configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "TicketBooking:";
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Safe to use now
    db.Database.Migrate();
    db.SeedFromSqlFile();
}


app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
