using ApiCubosExamenVCR.Models;
using ApiCubosExamenVCR.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCubosExamenVCR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;

        public CubosController(RepositoryCubos repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetCubos()
        {
            List<Cubo> cubos = await this.repo.GetCubosAsync();
            if (cubos.Count == 0)
            {
                return NotFound("No hay cubos");
            }
            else
            {
                foreach (Cubo cubo in cubos)
                {
                    cubo.Imagen = "https://storagetajamarvcr.blob.core.windows.net/imgscubos/" + cubo.Imagen; ;
                }
                return Ok(cubos);
            }
        }
    }
}
