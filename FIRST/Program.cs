using System.Text;
using FIRST.Data;
using FIRST.Extensions;
using FIRST.Hubs;
using FIRST.Middlewares;
using FIRST.Models;
using FIRST.Repositories.Chat;
using FIRST.Repositories.Contacts;
using FIRST.Repositories.Students;
using FIRST.Repositories.Subjects;
using FIRST.Repositories.Teachers;
using FIRST.Services;
using FIRST.Services.Auth;
using FIRST.Services.Chat;
using FIRST.Services.Contacts;
using FIRST.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using FIRST.Repositories.Classes;
using FIRST.Services.Classes;
using FIRST.Services.Classes.Queries;
using FIRST.Repositories.Classes.Commands;
using FIRST.Services.Classes.Commands;

var builder = WebApplication.CreateBuilder(args);

// Config variables
var corsSection = builder.Configuration.GetSection("Cors");
var corsPolicyName = corsSection.GetValue<string>("PolicyName") ?? "FrontendOrigins";
var allowedOrigins = corsSection.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var allowCredentials = corsSection.GetValue<bool>("AllowCredentials");


//
// 1) Database
//
var connectionString = builder.Configuration.GetConnectionString("DB");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// IMediator
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

//
// 2) Dependency Injection (Repositories, Services, Helpers)
//
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Students
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<StudentService>();

// Teachers
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<TeacherService>();

//Subjects
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<SubjectService>();

// Auth
builder.Services.AddScoped<AuthService>();


// Hub
builder.Services.AddSignalR();
builder.Services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, UserIdProvider>();


// Contacts
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ContactService>();


// Classes Query
builder.Services.AddScoped<IClassesRepository, ClassesRepository>();
builder.Services.AddScoped<ClassQueryService>();

// Classes Command
builder.Services.AddScoped<IClassesCommandRepository, ClassesCommandRepository>();
builder.Services.AddScoped<ClassCommandService>();



// Messages
builder.Services.AddScoped<ChatService>();

// cc
// Conversations

builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<ConversationService>();

// demnades
builder.Services.AddScoped<DemandeService>();
builder.Services.AddScoped<DemandeTypeService>();


// Tracking connection with singelton pattern 
builder.Services.AddSingleton<IPresenceTracker, PresenceTracker>();
//
// 3) Controllers + Validation response
//
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    k => k.Key,
                    v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return new BadRequestObjectResult(
                ApiResponse<object>.Fail("Validation failed", errors)
            );
        };
    });

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
{
    if (allowedOrigins.Length == 0)
        throw new InvalidOperationException("CORS: AllowedOrigins is empty.");

    policy.WithOrigins(allowedOrigins)
          .AllowAnyHeader()
          .AllowAnyMethod();

    if (allowCredentials)
        policy.AllowCredentials();
});

});


//
// 4) Authentication / Authorization (JWT)
//
var jwtSection = builder.Configuration.GetRequiredSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        options.Events = new JwtBearerEvents
        {
             OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/app"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.Fail("Unauthorized: missing or invalid token")
                );
            },

            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.Fail("Forbidden: you don't have access")
                );
            }
        };
    });

builder.Services.AddAuthorization();

//
// 5) Swagger / OpenAPI
//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


//
// 6) Middleware pipeline
//
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.UseApiStatusCodePages();

app.MapControllers();
app.MapHub<AppHub>("/hubs/app");

app.Run();
