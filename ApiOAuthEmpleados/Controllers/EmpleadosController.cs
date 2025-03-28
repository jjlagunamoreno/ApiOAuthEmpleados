using System.Security.Claims;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private RepositoryHospital repo;
        public EmpleadosController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Empleado>>>
            GetEmpleados()
        {
            return await this.repo.GetEmpleadosAsync();
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>>
        FindEmpleado(int id)
        {
            return await this.repo.FindEmpleadoAsync(id);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Empleado>>
        Perfil()
        {
            Claim claim = HttpContext.User.FindFirst
                (z => z.Type == "UserData");
            string json = claim.Value;
            Empleado empleado = JsonConvert
                .DeserializeObject<Empleado>(json);
            return await this.repo.FindEmpleadoAsync(empleado.IdEmpleado);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<List<Empleado>>
            compis()
        {
            string json = HttpContext.User.FindFirst
                (x => x.Type == "UserData").Value;
            Empleado empleado = JsonConvert
                .DeserializeObject<Empleado>(json);
            return await this.repo.GetCompisEmpleadoAsync
                (empleado.IdDepartamento);
        }
    }
}
