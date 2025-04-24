using ApiCubosExamenVCR.Data;
using ApiCubosExamenVCR.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCubosExamenVCR.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext _context;

        public RepositoryCubos(CubosContext context)
        {
            _context = context;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            return await _context.Cubos.ToListAsync();
        }

        public async Task CreateUsuario(string nombre, string email, string pass) {

            int newId = await this._context.Usuarios.MaxAsync(u => u.IdUsuario) + 1;

            Usuario usuario = new Usuario
            {
                IdUsuario = newId,
                Nombre = nombre,
                Email = email,
                Pass = pass,
                Imagen = ""
            };

            this._context.Usuarios.Add(usuario);
            await this._context.SaveChangesAsync();
        }


        public async Task<Usuario> GetUsuarioAsync(int id)
        {
            return await this._context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
        }
        public async Task<Usuario> ValidarUsuario(string email, string password)
        {
            return await this._context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Pass == password);
        }
    }
}
