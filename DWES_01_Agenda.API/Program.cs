using DWES_01_Agenda.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string connectionString = "Data Source=agenda.db";
builder.Services.AddDependenciesProvider("EFCore", connectionString);

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (KeyNotFoundException ex)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync(ex.Message);
    }
    catch (ArgumentException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync(ex.Message);
    }
});

app.MapControllers();

app.Run();
