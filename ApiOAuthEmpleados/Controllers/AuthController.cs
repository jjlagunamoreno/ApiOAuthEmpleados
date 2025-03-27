using System.IdentityModel.Tokens.Jwt;
using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryHospital repo;
        //CUANDO GENEREMOS EL TOKEN DEBEMOS INTEGRAR
        //ALGUNOS DATOS COMO ISSUER Y DEMÁS
        private HelperActionServicesOAuth helper;

        public AuthController(RepositoryHospital repo,
            HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult>
            Login(LoginModel model) 
        {
            Empleado empleado = await this.repo.LogInEmpleadoAsync(model.UserName,
                int.Parse(model.Password));
            if (empleado == null)
            {
                return Unauthorized();
            }
            else
            {
                //DEBEMOS CREAR UNAS CREDENCIALES PARA 
                //INCLUIRLAS DENTRO DEL TOKEN Y QUE ESTARÁN
                //COMPUESTAS POR EL SECRET KEY CIFRADO Y EL
                //TIPO DE CIFRADO QUE INCLUIREMOS EN EL TOKEN
                SigningCredentials credentials =
                    new SigningCredentials
                    (this.helper.GetKeyToken(),
                    SecurityAlgorithms.HmacSha256);
                //EL TOKEN SE GENERA CON UNA CLASE
                //Y DEBEMOS INDICAR LOS DATOS QUE 
                //ALMACENARÁ EN SU INTERIOR
                JwtSecurityToken token =
                    new JwtSecurityToken(
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(20),
                        notBefore: DateTime.UtcNow
                        );
                //POR ÚLTIMO, DEVOLVEMOS LA RESPUESTA AFIRMATIVA
                //CON UN OBJETO QUE CONTENGA EL TOKEN (anónimo)
                return Ok(new
                {
                    response = 
                    new JwtSecurityTokenHandler()
                    .WriteToken(token)
                });
            }
        }
    }
}
