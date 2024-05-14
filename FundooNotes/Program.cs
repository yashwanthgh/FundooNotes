using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using RepositoryLayer.Context;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using NLog.Web;
using ModelLayer.EmailModel;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
        builder.Services.AddLogging();

        // AddScoped
        builder.Services.AddSingleton<DapperContext>();
        builder.Services.AddScoped<IRegisterRL, RegisterServiceRL>();
        builder.Services.AddScoped<IRegisterBL, RegisterServiceBL>();
        builder.Services.AddScoped<ILoginRL, LoginServiceRL>();
        builder.Services.AddScoped<ILoginBL, LoginServiceBL>();
        builder.Services.AddScoped<IAuthServiceRL, AuthServiceRL>();
        builder.Services.AddScoped<INotesBL, NotesServiceBL>();
        builder.Services.AddScoped<INotesRL, NotesServiceRL>();
        builder.Services.AddScoped<ILabelBL, LabelServiceBL>();
        builder.Services.AddScoped<ILabelRL, LabelServiceRL>();
        builder.Services.AddScoped<IEmailRL, EmailServiceRL>();
        builder.Services.AddScoped<IEmailBL, EmailServiceBL>();
        builder.Services.AddScoped<ICollaborationRL, CollaborationServiceRL>();
        builder.Services.AddScoped<ICollaborationBL, CollaborationServiceBL>();

        builder.Services.Configure<EmailSettingModel>(builder.Configuration.GetSection("SmtpSettings"));
        builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<EmailSettingModel>>().Value);

        builder.Services.AddDistributedMemoryCache();

        // Add services to the container.
        builder.Services.AddControllers();

        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"]);

        // Add authentication services with JWT Bearer token validation to the service collection
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            // Add JWT Bearer authentication options
            .AddJwtBearer(options =>
            {
                // Configure token validation parameters
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Specify whether the server should validate the signing key
                    ValidateIssuerSigningKey = true,

                    // Set the signing key to verify the JWT signature
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    // Specify whether to validate the issuer of the token (usually set to false for development)
                    ValidateIssuer = false,

                    // Specify whether to validate the audience of the token (usually set to false for development)
                    ValidateAudience = false,
                };
            });

        // Configure Swagger/OpenAPI
        // Configure Swagger generation options
        builder.Services.AddSwaggerGen(c =>
        {
            // Define Swagger document metadata (title and version)
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

            // Configure JWT authentication for Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                // Describe how to pass the token
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization", // The name of the header containing the JWT token
                In = ParameterLocation.Header, // Location of the JWT token in the request headers
                Type = SecuritySchemeType.Http, // Specifies the type of security scheme (HTTP in this case)
                Scheme = "bearer", // The authentication scheme to be used (in this case, "bearer")
                BearerFormat = "JWT" // The format of the JWT token
            });

            // Specify security requirements for Swagger endpoints
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    // Define a reference to the security scheme defined above
                     new OpenApiSecurityScheme
                     {
                        Reference = new OpenApiReference
                        {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer" // The ID of the security scheme (defined in AddSecurityDefinition)
                        }
                     },
                     new string[] {} // Specify the required scopes (in this case, none)
                }
            });
        });

        
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