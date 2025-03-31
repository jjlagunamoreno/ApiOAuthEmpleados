using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Services;

namespace ApiOAuthEmpleados.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Empleados
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>>
            GetEmpleadosByOficiosAsync(List<string> oficios)
        {
            var consulta = from datos in this.context.Empleados
                           where oficios.Contains(datos.Oficio)
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task IncrementarSalariosAsync
            (int incremento, List<string> oficios)
        {
            List<Empleado> empleados = await
                this.GetEmpleadosByOficiosAsync(oficios);
            foreach (Empleado emp in empleados)
            {
                emp.Salario += incremento;
            }
            await this.context.SaveChangesAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            return await this.context.Empleados.ToListAsync();
        }

        public async Task<Empleado>
            FindEmpleadoAsync(int idEmpleado)
        {
            return await this.context.Empleados
                .FirstOrDefaultAsync(z => z.IdEmpleado == idEmpleado);
        }

        public async Task<List<Empleado>>
            GetCompisEmpleadoAsync(int idDepartamento)
        {
            return await this.context.Empleados
                .Where(x => x.IdDepartamento == idDepartamento)
                .ToListAsync();
        }

        public async Task<Empleado>
            LogInEmpleadoAsync(string apellido, int idEmpleado)
        {
            return await this.context.Empleados
                .Where(x => x.Apellido == apellido
                && x.IdEmpleado == idEmpleado)
                .FirstOrDefaultAsync();
        }
    }
}
