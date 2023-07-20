
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Movies.Core.Interfaces;
using Movies.Core.Models.Auth;
using Movies.Core.Models.Responses;
using Movies.EF;
using Movies.EF.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<ApplicationDbContext>(
	options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
	b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
	));

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

// Add our AuthService 
builder.Services.AddScoped<IAuthService, AuthService>();

// Mapping JWT values from appsettings.json to object
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

// Configure our Authentication Shared Schema with JWT Mapping 
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
				.AddJwtBearer(o =>
				{
					o.RequireHttpsMetadata = false;
					o.SaveToken = false;
					o.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidIssuer = builder.Configuration["JWT:Issuer"],
						ValidAudience = builder.Configuration["JWT:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
						ClockSkew = TimeSpan.Zero
					};
				});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
/// preferable to add CORS before Autherization
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

// Unauthorized (401) & Forbidden (403) MiddleWares
app.Use(async (context, next) =>
{
	await next();

	if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
	{
		context.Response.ContentType = "application/json";
		var unAuthorizedResponse = new UnAuthorizedFailureResponse();
		await context.Response.WriteAsync(unAuthorizedResponse.ToString());
	}
	else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden) // 403
	{
		context.Response.ContentType = "application/json";
		var forbiddenResponse = new ForbiddenFailureResponse();
		await context.Response.WriteAsync(forbiddenResponse.ToString());
	}
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
