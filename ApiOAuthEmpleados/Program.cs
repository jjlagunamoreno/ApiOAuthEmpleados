using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// INICIALIZAMOS EL HELPER PARA LA CYPTOGRAFÍA
// LUEGO INYECTAMOS HttpContextAccessor
HelperCryptography.Initialize(builder.Configuration);
builder.Services.AddHttpContextAccessor();

//**************************************************************************************//
//                                      SEGURIDAD
//**************************************************************************************//
//CREAMOS UNA INSTANCIA DE NUESTRO HELPER
HelperActionServicesOAuth helper =
    new HelperActionServicesOAuth(builder.Configuration);
//ESTA INSTANCIA SOLAMENTE DEBEMOS CREARLA UNA VEZ
//PARA QUE NUESTRA APLICACION PUEDA VALIDAR CON TODO LO QUE HA CREADO
builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);
//HABILITAMOS LA SEGURIDAD UTILIZANDO LA CLASE HELPER
builder.Services.AddAuthentication(helper.GetAuthenticateSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());
//**************************************************************************************//

// CADENA DE CONEXIÓN A LA BASE DE DATOS DE AZURE
string connectionString = builder.Configuration.GetConnectionString("SqlAzure");

// INYECCIÓN AL CONTEXTO DE LA CADENA DE CONEXIÓN
builder.Services.AddDbContext<HospitalContext>(options =>
    options.UseSqlServer(connectionString));

// REPOSITORIO
builder.Services.AddTransient<RepositoryHospital>();

// HELPER EMPLEADO TOKEN
builder.Services.AddScoped<HelperEmpleadoToken>();

// RESTO DE SERVICIOS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    
}

app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Seguridad Empleados");
    options.RoutePrefix = "";
});

app.UseHttpsRedirection();

app.UseAuthentication();// <- SEGURIDAD
app.UseAuthorization();
app.MapControllers();
app.Run();
