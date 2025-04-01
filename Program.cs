using ClinAgenda.src.Application.UseCases;
using ClinAgenda.src.Core.Interfaces;
using ClinAgenda.src.Infrastructure.Repositories;
using ClinAgendaAPI;
using ClinAgendaAPI.StatusUseCase;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Configuração da conexão com MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<MySqlConnection>(_ => new MySqlConnection(connectionString));

// Injeção de dependências 
builder.Services.AddScoped<StatusUseCase>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();


builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<SpecialtyUseCase>();

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<PatientUseCase>();

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<DoctorUseCase>();

builder.Services.AddScoped<IDoctorSpecialtyRepository, DoctorSpecialtyRepository>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<AppointmentUseCase>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();