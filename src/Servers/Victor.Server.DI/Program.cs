namespace Victor.Server.DI;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
 
using Serilog;

class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var logger = new LoggerConfiguration()
                     .ReadFrom.Configuration(builder.Configuration)
                     .Enrich.FromLogContext()
                     .Enrich.WithThreadId()
                     .CreateLogger();
        builder.Host.UseSerilog(logger);
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.UseSerilogRequestLogging();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        //app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}