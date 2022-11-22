using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Trainabl.Server;
using Trainabl.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
var connectionString = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContext<ApplicationContext>(opt =>
	                                                  opt.UseSqlServer(connectionString));
builder.Services.AddScoped<AccessControlService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
       {
	       c.Authority = $"{builder.Configuration["Auth0:Domain"]}";
	       c.TokenValidationParameters = new TokenValidationParameters
	       {
		       ValidAudience = builder.Configuration["Auth0:Audience"],
		       ValidIssuer   = builder.Configuration["Auth0:Domain"]
	       };
       });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();