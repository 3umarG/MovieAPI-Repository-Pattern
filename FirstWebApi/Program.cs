
using Movies.Core.Interfaces;
using Movies.EF;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(
	options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
	b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
	));

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

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
app.UseAuthorization();

app.MapControllers();

app.Run();
