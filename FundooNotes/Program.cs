using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using RepositoryLayer.Context;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<DapperContext>();
        builder.Services.AddScoped<IRegisterRL, RegisterServiceRL>();
        builder.Services.AddScoped<IRegisterBL, RegisterServiceBL>();

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}