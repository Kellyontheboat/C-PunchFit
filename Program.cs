using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication_punchFit.graphql;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApplication_punchFit.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:7253")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();

// Configure JWT authentication
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"]);
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated: " + context.SecurityToken);
            return Task.CompletedTask;
        }
    };
});

//// Register the Pooled DbContext Factory
//builder.Services.AddPooledDbContextFactory<workoutContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("PunchFitDBConnection")));

// Register the DbContext
builder.Services.AddDbContext<workoutContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PunchFitDBConnection")));

// Register the AuthService for login
builder.Services.AddScoped<AuthService>();

// Add GraphQL services automatic type registration
builder.Services
    .AddGraphQLServer()
    //.RegisterDbContextFactory<workoutContext>() // Register the DbContext factory for HotChocolate
    .AddAuthorization() // Add authorization middleware
    .AddTypes(); // Automatically register types

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles(); // support file in wwwroot
app.MapGraphQL("/graphql"); // Map GraphQL endpoint

app.Run();