using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MountainTracker.Infrastructure.Data;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MountainTrackerDbContext context)
            : base(context)
        {
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == login);
        }
    }
}
