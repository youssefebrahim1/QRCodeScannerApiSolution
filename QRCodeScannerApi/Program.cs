var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger to the services collection
builder.Services.AddEndpointsApiExplorer();  // Enable API endpoint discovery
builder.Services.AddSwaggerGen();  // Enable Swagger generation

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve Swagger-generated API documentation
    app.UseSwagger();

    // Enable middleware to serve Swagger UI (HTML, JS, CSS, etc.)
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Enable API routing
app.MapControllers();

app.Run();
