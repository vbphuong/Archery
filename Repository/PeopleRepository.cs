using Microsoft.EntityFrameworkCore;
using Archery.Data;
using Archery.Models.DTO;
using Archery.Models.Entity;

namespace Archery.Repository
{
    public class PeopleRepository : IPeopleRepository
    {
        private readonly AppDbContext _dbContext;

        public PeopleRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserDTO>> GetAllAsync()
        {
            return await _dbContext.User
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
            return await _dbContext.User
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

            var existing = await _dbContext.User.AnyAsync(u => u.Email == dto.Email);
            if (existing) throw new ArgumentException("Email already exists");

            // Xác định role
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == dto.RoleName);
            if (role == null)
                role = await _dbContext.Roles.FirstAsync(r => r.RoleID == 1); // Mặc định là Archer

            // Tạo User mới
            var entity = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = "AQAAAAIAAYagAAAAEAzyqbhJCZY3/RiBZI9jngVwnGoyvgUgYb7raS5is7YhDde3wXDVA0dLlsnAx1i3fg==",
                RoleID = role.RoleID
            };

            await _dbContext.User.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            // Nếu là Archer thì tạo luôn record trong bảng Archer
            if (role.RoleName == "Archer")
            {
                var archer = new Archer
                {
                    UserID = entity.UserID,
                    // Các field khác để trống, Archer sẽ tự điền sau
                };

                await _dbContext.Archers.AddAsync(archer);
                await _dbContext.SaveChangesAsync();
            }

            dto.UserId = entity.UserID;
            dto.RoleName = role.RoleName;
            return dto;
        }


        public async Task<bool> UpdateAsync(int id, UserDTO dto)
        {
            var user = await _dbContext.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == id);
            if (user == null) return false;

            if (user.Role != null && user.Role.RoleName == "Admin") return false;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;

            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == dto.RoleName);
            if (role != null && role.RoleName != "Admin")
            {
                user.RoleID = role.RoleID;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _dbContext.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserID == id);
            if (user == null) return false;

            if (user.Role != null && user.Role.RoleName == "Admin") return false;

            _dbContext.User.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}