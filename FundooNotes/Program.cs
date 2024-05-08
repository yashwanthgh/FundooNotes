using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using RepositoryLayer.Context;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using NLog.Web;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // NLog
        builder.Logging.AddDebug();
        var logpath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        NLog.GlobalDiagnosticsContext.Set("LogDirectory", logpath);
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Host.UseNLog();

        // AddScoped
        builder.Services.AddSingleton<DapperContext>();
        builder.Services.AddScoped<IRegisterRL, RegisterServiceRL>();
        builder.Services.AddScoped<IRegisterBL, RegisterServiceBL>();
        builder.Services.AddScoped<ILoginRL, LoginServiceRL>();
        builder.Services.AddScoped<ILoginBL, LoginServiceBL>();
        builder.Services.AddScoped<IAuthServiceRL, AuthServiceRL>();

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