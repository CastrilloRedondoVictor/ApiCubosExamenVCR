using ApiCubosExamenVCR.Helpers;
using ApiCubosExamenVCR.Models;
using ApiCubosExamenVCR.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiCubosExamenVCR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private IConfiguration _configuration;
        private RepositoryCubos _repo;
        private HelperActionServicesOAuth _helper;

        public AuthController(IConfiguration configuration, RepositoryCubos repo, HelperActionServicesOAuth helper)
        {
            this._configuration = configuration;
            this._repo = repo;
            this._helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            Usuario usuario = await this._repo.ValidarUsuario(model.Email, model.Pass);

            if (usuario == null)
            {
                return Unauthorized();
            }

            SigningCredentials credentials = new SigningCredentials(this._helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);
            string jsonUsuario = JsonConvert.SerializeObject(usuario);

            Claim[] info = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new Claim("Email", usuario.Email)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                claims: info,
                issuer: this._helper.Issuer,
                audience: this._helper.Audience,
                signingCredentials: credentials,
                expires: DateTime.UtcNow.AddMinutes(30),
                notBefore: DateTime.UtcNow
            );

            return Ok(new
            {
                response = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(string nombre, string email, string pass)
        {
            await this._repo.CreateUsuario(nombre, email, pass);
            return Ok();
        }


        [HttpGet]
        [Authorize]
        [Route("[Action]")]
        public async Task<IActionResult> GetPerfilUsuario()
        {
            int id = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "IdUsuario").Value);
            var usuario = await this._repo.GetUsuarioAsync(id);
            if (usuario == null)
            {
                return NotFound("No existe el usuario");
            }
            else
            {
                usuario.Imagen = "https://storagetajamarvcr.blob.core.windows.net/imgsusuarios/" + usuario.Imagen; 
                return Ok(usuario);
            }
        }
    }
}
