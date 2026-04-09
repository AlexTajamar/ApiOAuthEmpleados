using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiOAuthEmpleados.Repositories
{
    public class RepositoryHospital
    {
        public HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            var consulta = from datos in this.context.Empleados
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<Empleado> FindEmpleadoAsync(int idempleado)
        {
            var consulta = from datos in this.context.Empleados
                           where datos.IdEmpleado == idempleado
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<Empleado> LogInEmpleado(string apellifo, int idEmpleado)
        {
            return await this.context.Empleados
                .Where(Z => Z.Apellido == apellifo && Z.IdEmpleado == idEmpleado)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Empleado>> GetCompisAsync(int idDepartamento)
        {
            var consulta = from datos in this.context.Empleados
                           where datos.IdDepartamento == idDepartamento
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Empleados
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosByOficiosAsync(List<string> oficios)
        {
            var consulta = from datos in this.context.Empleados
                           where oficios.Contains(datos.Oficio)
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task IncrementarSalariosAsync(int incremento, List<string> oficios)
        {
           List<Empleado> empleados = await this.GetEmpleadosByOficiosAsync(oficios);

            foreach (var empleado in empleados)
            {
            {
                empleado.Salario += incremento;
            }

            await this.context.SaveChangesAsync();
        }
    }
    }

