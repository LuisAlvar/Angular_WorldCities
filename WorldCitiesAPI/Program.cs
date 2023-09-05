using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;
using Microsoft.AspNetCore.Cors;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

using WorldCitiesAPI.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Writers;
using Microsoft.IdentityModel.Tokens;
using WorldCitiesAPI.Data.GraphQL;

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

// First ASP.NET Core service that we create
builder.Services.AddScoped<JwtHandler>();

//GraphQL 
builder.Services.AddGraphQLServer()
  .AddAuthorization()
  .AddQueryType<Query>()
  .AddMutationType<Mutation>()
  .AddFiltering()
  .AddSorting();

// Add Authentication services & middleware 
builder.Services.AddAuthentication(opt => {
  opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt => {

  opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
  {
    RequireExpirationTime = true,
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
    ValidAudience = builder.Configuration["JwtSettings:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecurityKey"]))
  };
});


var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
  app.UseExceptionHandler("/Error");
  app.MapGet("/Error", () => Results.Problem());
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AngularPolicy");

app.MapControllers();

app.MapGraphQL("/api/graphql");

app.MapMethods("api/heartbeat", new[] { "HEAD" }, () => Results.Ok());

app.Run();
