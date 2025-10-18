using Microsoft.EntityFrameworkCore;
using Archery.Data;
using Archery.Models.DTO;
using Archery.Models.Entity;

namespace Archery.Repository
{
    public class PeopleRepository : BaseRepository, IPeopleRepository
    {
        public PeopleRepository(AppDbContext context) : base(context) { }

        public async Task<List<UserDTO>> GetAllAsync()
        {
            return await _context.User
                .Include(u => u.Role)
                .Select(u => new UserDTO
                {
                    UserId = u.UserID,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleName = u.Role != null ? u.Role.RoleName : "Unknown"
                })
                .ToListAsync();
        }

        public async Task<UserDTO?> GetByIdAsync(int id)
        {
            return await _context.User
                .Include(u => u.Role)
                .Where(u => u.UserID == id)
                .Select(u => new UserDTO
                {
                    UserId = u.UserID,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleName = u.Role != null ? u.Role.RoleName : "Unknown"
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UserDTO> CreateAsync(UserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName))
            {
                throw new ArgumentException("All fields are required");
            }

            var existing = await _context.User.AnyAsync(u => u.Email == dto.Email);
            if (existing) throw new ArgumentException("Email already exists");

            // Xác định role
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == dto.RoleName);
            if (role == null)
                role = await _context.Roles.FirstAsync(r => r.RoleID == 1); // Mặc định là Archer

            var entity = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = "AQAAAAIAAYagAAAAEAzyqbhJCZY3/RiBZI9jngVwnGoyvgUgYb7raS5is7YhDde3wXDVA0dLlsnAx1i3fg==",
                RoleID = role.RoleID
            };

            await _context.User.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Nếu là Archer thì tạo luôn record trong bảng Archer
            if (role.RoleName == "Archer")
            {
                var archer = new Archer
                {
                    UserID = entity.UserID,
                    // Các field khác để trống, Archer sẽ tự điền sau
                };

                await _context.Archers.AddAsync(archer);
                await _context.SaveChangesAsync();
            }

            dto.UserId = entity.UserID;
            dto.RoleName = role.RoleName;
            return dto;
        }


        public async Task<bool> UpdateAsync(int id, UserDTO dto)
        {
            var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == id);
            if (user == null) return false;

            if (user.Role != null && user.Role.RoleName == "Admin") return false;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == dto.RoleName);
            if (role != null && role.RoleName != "Admin")
            {
                user.RoleID = role.RoleID;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == id);
            if (user == null) return false;

            if (user.Role != null && user.Role.RoleName == "Admin") return false;

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}