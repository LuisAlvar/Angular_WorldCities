using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;
using Microsoft.AspNetCore.Cors;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

using WorldCitiesAPI.Data.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Adds Serilog Support
builder.Host.UseSerilog((ctx, lc) =>
  lc.ReadFrom.Configuration(ctx.Configuration)
  .WriteTo.MSSqlServer(connectionString: ctx.Configuration.GetConnectionString("DefaultConnection"),
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents", AutoCreateSqlTable = true })
  .WriteTo.Console()
);


builder.Services
  .AddControllers()
  .AddJsonOptions(options => {
    //options.JsonSerializerOptions.WriteIndented = true;
    //options.JsonSerializerOptions.PropertyNamingPolicy = null;
  });


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
  options.SignIn.RequireConfirmedAccount = true;
  options.Password.RequireDigit = true;
  options.Password.RequireLowercase = true;
  options.Password.RequireUppercase = true;
  options.Password.RequireNonAlphanumeric = true;
  options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<ApplicationDbContext>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// we will relax the CORS policy for any endpoint, for testing puporses only.
// REMOVE: When deploying this app to production
builder.Services.AddCors(opts => {
  opts.AddPolicy(name: "AngularPolicy", cfg => {
    cfg.AllowAnyHeader();
    cfg.AllowAnyMethod();
    cfg.WithOrigins(builder.Configuration["AllowedCORS"]);
  });
});

var strConnection = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("This is the connection string from .json " + strConnection);

builder.Services
    .AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(strConnection));

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AngularPolicy");

app.MapControllers();

app.Run();
