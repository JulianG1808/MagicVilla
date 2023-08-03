using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;
using System.Security.Cryptography.X509Certificates;

namespace MagicVilla_API.Repository
{
    public class VillaRepository :Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        //                               se le pasa el dbContext al padre con base(db)
        public VillaRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;   
        }

        public async Task<Villa> Refresh(Villa entity)
        {
            entity.FechaActualizacion = DateTime.Now;
            _db.Villas.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
