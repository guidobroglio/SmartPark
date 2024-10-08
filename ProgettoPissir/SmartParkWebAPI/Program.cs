using Microsoft.EntityFrameworkCore;
using SmartParkCore.Data;
using SmartParkCore.Data.Repository;
using SmartParkManager.Gestori;
using SmartParkIoT.MWbot;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SmartParkCore.Security;
using Microsoft.AspNetCore.Identity;
using SmartParkManager.Hubs;
using SmartParkIoT.Parcheggio;
using Microsoft.AspNetCore.SignalR;
using MQTTnet.Client;

var builder = WebApplication.CreateBuilder(args);

// Adding Identity services with Guid for IdentityRole
builder.Services.AddIdentity<Utente, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// Configure JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//Aggiunta policy per autorizzazione
builder.Services.AddAuthorization(options =>
{
    //Ruoli: Amministratore, Utente, UtentePremium
    options.AddPolicy("RuoloUtente", policy => policy.RequireRole(Ruolo.Utente));
    options.AddPolicy("RuoloUtentePremium", policy => policy.RequireRole(Ruolo.UtentePremium));
    options.AddPolicy("RuoloAmministratore", policy => policy.RequireRole(Ruolo.Amministratore));
});

//Connessione al database
var connectionString = builder.Configuration.GetConnectionString("(default)") ?? throw new InvalidOperationException("Connection string 'DataContextConnection' not found.");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionString));

AddRepositories();

builder.Services.AddSignalR().AddHubOptions<NotificaRicarica>(options =>
{
    options.EnableDetailedErrors = true;
});

 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://localhost:7028")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

var app = builder.Build();

// Seed roles on startup
var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

string[] ruoli = { Ruolo.Amministratore, Ruolo.Utente, Ruolo.UtentePremium };
await ImpostaRuoli(roleManager, ruoli);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//---
scope = app.Services.CreateScope();

var mwbot = scope.ServiceProvider.GetRequiredService<MWbot>();
var monitor = scope.ServiceProvider.GetRequiredService<MonitorPosti>();
var gestoreMWbot = scope.ServiceProvider.GetRequiredService<IGestoreMWbot>();

await mwbot.AvviaMWbot();
await monitor.AvviaMonitorPosti();
await gestoreMWbot.AvviaGestoreRicariche();

app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificaRicarica>("/notificaHub");

app.Run();

void AddRepositories()
{
    var services = builder.Services;
    var configuration = builder.Configuration;

    services.AddScoped<IRepositorySosta, RepositorySosta>();
    services.AddScoped<IRepositoryVeicolo, RepositoryVeicolo>();
    services.AddScoped<IRepositoryPosto, RepositoryPosto>();
    services.AddScoped<IRepositoryRicarica, RepositoryRicarica>();
    services.AddScoped<IRepositoryUtente, RepositoryUtente>();
    services.AddScoped<IRepositoryPagamento, RepositoryPagamento>();
    services.AddScoped<IRepositoryPrezzo, RepositoryPrezzo>();
    services.AddScoped<IRepositoryPrenotazione, RepositoryPrenotazione>();
    services.AddScoped<IGestoreParcheggio, GestoreParcheggio>();
    services.AddScoped<IGestorePrenotazioni, GestorePrenotazioni>();
    services.AddScoped<IGestorePagamenti, GestorePagamenti>();
    services.AddScoped<IGestoreMonitor, GestoreSensori>();
    services.AddScoped<IGestoreVeicoli, GestoreVeicoli>();
    services.AddScoped<IGestorePrezzi, GestorePrezzi>();
    services.AddScoped<IGestoreMWbot, GestoreMWbot>();
    services.AddScoped<IGestoreRicariche, GestoreRicariche>();
    services.AddScoped<MWbot>();
    services.AddScoped<MonitorPosti>();
}

async Task ImpostaRuoli(RoleManager<IdentityRole<Guid>> roleManager, string[] ruoli)
{
    foreach (var ruolo in ruoli)
    {
        if (!await roleManager.RoleExistsAsync(ruolo))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(ruolo));
        }
    }
}
