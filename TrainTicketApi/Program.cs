using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TrainTicketApi.Data;
using TrainTicketApi.Repositories;
using TrainTicketApi.Repositories.Interfaces;
using TrainTicketApi.Services;
using TrainTicketApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Quáº£n lÃ½ bÃ¡n vÃ© tÃ u há»a",
        Version = "v1",
        Description = "API backend cÃ³ xÃ¡c thá»±c JWT Ä‘á»ƒ quáº£n lÃ½ tuyáº¿n tÃ u, chuyáº¿n tÃ u, khÃ¡ch hÃ ng, vÃ© tÃ u vÃ  bÃ¡o cÃ¡o."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nháº­p theo Ä‘á»‹nh dáº¡ng: Bearer {token_cua_ban}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key.");
var jwtIssuer = jwtSection["Issuer"] ?? "TrainTicketApi";
var jwtAudience = jwtSection["Audience"] ?? "TrainTicketWinForms";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        NameClaimType = System.Security.Claims.ClaimTypes.Name,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[JWT] Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var name = context.Principal?.Identity?.Name ?? "(unknown)";
            Console.WriteLine($"[JWT] Token validated for user: {name}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<ITrainTripRepository, TrainTripRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<ITrainTripService, TrainTripService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Desktop client currently uses HTTP base URL (http://localhost:5216).
// Keep API on HTTP to avoid automatic redirects that can drop Authorization header.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Tự động apply pending migrations
    dbContext.Database.Migrate();

    await SeedIdentityAsync(roleManager, userManager);
    
    // Tạo data mẫu nếu DB trống
    DbSeeder.SeedData(dbContext);
}

app.Run();

static async Task SeedIdentityAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    var roles = new[] { "Admin", "Staff" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    const string adminUserName = "admin";
    const string adminPassword = "admin123";
    var adminUser = await userManager.FindByNameAsync(adminUserName);
    if (adminUser is null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminUserName,
            Email = "admin@trainticket.local",
            EmailConfirmed = true
        };
        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    const string staffUserName = "staff";
    const string staffPassword = "staff123";
    var staffUser = await userManager.FindByNameAsync(staffUserName);
    if (staffUser is null)
    {
        staffUser = new IdentityUser
        {
            UserName = staffUserName,
            Email = "staff@trainticket.local",
            EmailConfirmed = true
        };
        var createResult = await userManager.CreateAsync(staffUser, staffPassword);
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(staffUser, "Staff");
        }
    }
}
