using ClientsAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=clientservice.db"));

// Configuration de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Activation de Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
});

// Création automatique de la base de données
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// CRUD Routes
app.MapGet("/clients", async (AppDbContext db) =>
    await db.Clients.ToListAsync());

app.MapGet("/clients/{id}", async (int id, AppDbContext db) =>
    await db.Clients.FindAsync(id) is Client client ? Results.Ok(client) : Results.NotFound());

app.MapPost("/clients", async (AppDbContext db, Client client) =>
{
    db.Clients.Add(client);
    await db.SaveChangesAsync();
    return Results.Created($"/clients/{client.Id}", client);
});

app.MapPut("/clients/{id}", async (int id, AppDbContext db, Client inputClient) =>
{
    var client = await db.Clients.FindAsync(id);
    if (client == null) return Results.NotFound();

    client.Id = inputClient.Id;
    client.Nom = inputClient.Nom;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/clients/{id}", async (int id, AppDbContext db) =>
{
    var client = await db.Clients.FindAsync(id);
    if (client == null) return Results.NotFound();

    db.Clients.Remove(client);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
