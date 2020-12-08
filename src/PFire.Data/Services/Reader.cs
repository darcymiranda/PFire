using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PFire.Data.Services
{
    internal interface IReader
    {
        IQueryable<T> Query<T>() where T : Entity;
    }

    internal class Reader : IReader
    {
        private readonly IDatabaseContext _context;

        public Reader(IDatabaseContext context)
        {
            _context = context;
        }

        public IQueryable<T> Query<T>() where T : Entity
        {
            return _context.Set<T>().AsNoTracking();
        }
    }
}
