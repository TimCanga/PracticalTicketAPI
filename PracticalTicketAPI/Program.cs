using Microsoft.EntityFrameworkCore;
using PracticalTicketAPI.Data;
using PracticalTicketAPI.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure EF Core with SQLite using the connection string from appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tickets.db"));

// Register the ticket repository.
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

// Enable Swagger for API documentation (optional).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure the database is created.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();

    // Seed the database only if there are no tickets.
    if (!dbContext.Tickets.Any())
    {
        // Build the file path to TicketData.json.
        // AppContext.BaseDirectory typically points to the build output directory.
        var filePath = Path.Combine(AppContext.BaseDirectory, "TicketData.json");

        if (File.Exists(filePath))
        {
            var jsonString = File.ReadAllText(filePath);
            var tickets = JsonSerializer.Deserialize<List<Ticket>>(jsonString);

            if (tickets != null && tickets.Count > 0)
            {
                dbContext.Tickets.AddRange(tickets);
                dbContext.SaveChanges();
            }
        }
        else
        {
            Console.WriteLine("TicketData.json file not found: " + filePath);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
