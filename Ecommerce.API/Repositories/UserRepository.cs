using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EcommerceDbContext _context;
        public UserRepository(EcommerceDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            user.IsActive = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAllAsync(string? includeProperties = null)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }
            return await query.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id, string? includeProperties = null)
        {
            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }


        public async Task<User?> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetByIdsAsync(List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return Enumerable.Empty<User>();

            return await _context.Users
                                   .Where(u => ids.Contains(u.Id))
                                   .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            if (string.IsNullOrEmpty(role))
                return Enumerable.Empty<User>();

            if (!Enum.TryParse<RoleStatus>(role, true, out var parsedRole))
                return Enumerable.Empty<User>();

            return await _context.Users
                                   .Where(u => u.Role == parsedRole && u.IsActive)
                                   .ToListAsync();

        }

        public async Task<IEnumerable<User>> GetUsersByRolesAdminOrStaffAsync(IEnumerable<string> roles)
        {
            if (roles == null || !roles.Any())
                return Enumerable.Empty<User>();

            // Parse role strings to RoleStatus enum
            var validRoles = roles
                .Select(role => Enum.TryParse<RoleStatus>(role, true, out var parsed) ? parsed : (RoleStatus?)null)
                .Where(r => r.HasValue)
                .Select(r => r.Value)
                .ToList();

            if (!validRoles.Any())
                return Enumerable.Empty<User>();

            return await _context.Users
                .Where(u => validRoles.Contains(u.Role) && u.IsActive)
                .ToListAsync();
        }

        public async Task<User?> FindAvailableStaffAsync()
        {
            return await _context.Users
                .Where(u => u.Role == RoleStatus.STAFF || u.Role == RoleStatus.ADMIN)
                .OrderBy(u => Guid.NewGuid())
                .FirstOrDefaultAsync();
        }
    }
}
