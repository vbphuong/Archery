using Microsoft.EntityFrameworkCore;
using Archery.Data;
using Archery.Models.Entity;
using Archery.Models.DTO;

namespace Archery.Repository
{
    public class ArcherRepository : BaseRepository, IArcherRepository
    {
        public ArcherRepository(AppDbContext context) : base(context) { }

        public async Task<List<ArcherDTO>> GetAllAsync()
        {
            return await _context.Archers
                .Include(a => a.User)
                .Include(a => a.Address)
                    .ThenInclude(addr => addr!.City)
                        .ThenInclude(city => city!.State)
                            .ThenInclude(state => state!.Country)
                .Include(a => a.ArcherEquipments)
                    .ThenInclude(ae => ae.Equipment)
                .Select(a => new ArcherDTO
                {
                    ArcherId = a.ArcherID,
                    UserId = a.UserID,
                    FullName = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : "Unknown",
                    Email = a.User != null ? a.User.Email : "N/A",
                    Gender = a.Gender,
                    DateOfBirth = a.DateOfBirth,
                    Status = a.Status,
                    AddressId = a.AddressID,
                    Country = a.Address != null ? a.Address.City!.State!.Country!.CountryName : null,
                    State = a.Address != null ? a.Address.City!.State!.StateName : null,
                    City = a.Address != null ? a.Address.City!.CityName : null,
                    EquipmentIds = a.ArcherEquipments.Select(ae => ae.EquipmentID).ToList(),
                    EquipmentNames = a.ArcherEquipments.Select(ae => ae.Equipment.Type).ToList()
                })
                .ToListAsync();
        }

        public async Task<ArcherDTO?> GetByIdAsync(int archerId)
        {
            return await _context.Archers
                .Include(a => a.User)
                .Include(a => a.Address)
                    .ThenInclude(addr => addr!.City)
                        .ThenInclude(city => city!.State)
                            .ThenInclude(state => state!.Country)
                .Include(a => a.ArcherEquipments)
                    .ThenInclude(ae => ae.Equipment)
                .Where(a => a.ArcherID == archerId)
                .Select(a => new ArcherDTO
                {
                    ArcherId = a.ArcherID,
                    UserId = a.UserID,
                    FullName = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : "Unknown",
                    Email = a.User != null ? a.User.Email : "N/A",
                    Gender = a.Gender,
                    DateOfBirth = a.DateOfBirth,
                    Status = a.Status,
                    AddressId = a.AddressID,
                    AddressLine = a.Address != null ? a.Address.AddressLine : null,
                    CityId = a.Address != null ? a.Address.CityID : null,
                    StateId = a.Address != null ? a.Address.City!.StateID : null,
                    CountryId = a.Address != null ? a.Address.City!.State!.CountryID : null,
                    Country = a.Address != null ? a.Address.City!.State!.Country!.CountryName : null,
                    State = a.Address != null ? a.Address.City!.State!.StateName : null,
                    City = a.Address != null ? a.Address.City!.CityName : null,
                    EquipmentIds = a.ArcherEquipments.Select(ae => ae.EquipmentID).ToList(),
                    EquipmentNames = a.ArcherEquipments.Select(ae => ae.Equipment.Type).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ArcherDTO?> GetByUserIdAsync(int userId)
        {
            return await _context.Archers
                .Include(a => a.User)
                .Include(a => a.Address)
                    .ThenInclude(addr => addr!.City)
                        .ThenInclude(city => city!.State)
                            .ThenInclude(state => state!.Country)
                .Include(a => a.ArcherEquipments)
                    .ThenInclude(ae => ae.Equipment)
                .Where(a => a.UserID == userId)
                .Select(a => new ArcherDTO
                {
                    ArcherId = a.ArcherID,
                    UserId = a.UserID,
                    FullName = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : "Unknown",
                    Email = a.User != null ? a.User.Email : "N/A",
                    Gender = a.Gender,
                    DateOfBirth = a.DateOfBirth,
                    Status = a.Status,
                    AddressId = a.AddressID,
                    AddressLine = a.Address != null ? a.Address.AddressLine : null,
                    CityId = a.Address != null ? a.Address.CityID : null,
                    StateId = a.Address != null ? a.Address.City!.StateID : null,
                    CountryId = a.Address != null ? a.Address.City!.State!.CountryID : null,
                    Country = a.Address != null ? a.Address.City!.State!.Country!.CountryName : null,
                    State = a.Address != null ? a.Address.City!.State!.StateName : null,
                    City = a.Address != null ? a.Address.City!.CityName : null,
                    EquipmentIds = a.ArcherEquipments.Select(ae => ae.EquipmentID).ToList(),
                    EquipmentNames = a.ArcherEquipments.Select(ae => ae.Equipment.Type).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ArcherDTO> CreateAsync(ArcherDTO dto)
        {
            var entity = new Archer
            {
                UserID = dto.UserId,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                AddressID = dto.AddressId,
                Status = dto.Status ?? "Active"
            };

            _context.Archers.Add(entity);
            await _context.SaveChangesAsync();

            // Gắn Equipment
            if (dto.EquipmentIds?.Any() == true)
            {
                foreach (var eqId in dto.EquipmentIds)
                {
                    _context.ArcherEquipments.Add(new ArcherEquipment
                    {
                        ArcherID = entity.ArcherID,
                        EquipmentID = eqId
                    });
                }
                await _context.SaveChangesAsync();
            }

            dto.ArcherId = entity.ArcherID;
            return dto;
        }

        public async Task<bool> UpdateAsync(int archerId, ArcherDTO dto)
        {
            var entity = await _context.Archers
                .Include(a => a.ArcherEquipments)
                .Include(a => a.Address)
                .FirstOrDefaultAsync(a => a.ArcherID == archerId);

            if (entity == null) return false;

            entity.DateOfBirth = dto.DateOfBirth;
            entity.Gender = dto.Gender;

            // Update Address
            if (dto.CityId.HasValue)
            {
                var newAddress = new Address
                {
                    CityID = dto.CityId.Value,
                    AddressLine = dto.AddressLine
                };
                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();
                entity.AddressID = newAddress.AddressID;
            }

            // Update Equipment
            if (dto.EquipmentIds != null)
            {
                // Xóa tất cả equipment cũ
                var oldEquip = _context.ArcherEquipments.Where(ae => ae.ArcherID == entity.ArcherID);
                _context.ArcherEquipments.RemoveRange(oldEquip);

                // Thêm lại equipment mới
                foreach (var eqId in dto.EquipmentIds)
                {
                    _context.ArcherEquipments.Add(new ArcherEquipment
                    {
                        ArcherID = entity.ArcherID,
                        EquipmentID = eqId
                    });
                }
            }

            // Update status (Admin/Recorder)
            if (dto.Role == "Admin" || dto.Role == "Recorder")
                entity.Status = dto.Status ?? "Active";
            else if (string.IsNullOrEmpty(entity.Status))
                entity.Status = "Active";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int archerId)
        {
            var entity = await _context.Archers.FindAsync(archerId);
            if (entity == null) return false;

            var eq = _context.ArcherEquipments.Where(ae => ae.ArcherID == archerId);
            _context.ArcherEquipments.RemoveRange(eq);

            _context.Archers.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // Top Elite
        public async Task<IEnumerable<EliteArcherDTO>> GetTopEliteArchersAsync(int topCount)
        {
            // Get total score 
            var totals = await _context.Scores
                .GroupBy(s => s.ArcherID)
                .Select(g => new
                {
                    ArcherID = g.Key,
                    Total = g.Sum(x => x.TotalScore)
                })
                .OrderByDescending(x => x.Total)
                .Take(topCount)
                .ToListAsync();

            // join với Archers + User info
            var archerIds = totals.Select(t => t.ArcherID).ToList();

            var archers = await _context.Archers
                .Include(a => a.User)
                .Where(a => archerIds.Contains(a.ArcherID))
                .ToListAsync();

            // map to DTO and set Rank
            var result = totals.Select((t, idx) =>
            {
                var a = archers.FirstOrDefault(x => x.ArcherID == t.ArcherID);
                var dto = new EliteArcherDTO
                {
                    ArcherId = a?.ArcherID ?? t.ArcherID,
                    UserId = a?.UserID ?? 0,
                    FullName = a?.User != null ? (a.User.FirstName + " " + a.User.LastName).Trim() : null,
                    Email = a?.User?.Email,
                    Gender = a?.Gender,
                    DateOfBirth = a?.DateOfBirth,
                    Status = a?.Status,
                    Rank = idx + 1,
                    TotalScore = (int)t.Total,
                    MonthYear = System.DateTime.Now.ToString("yyyy-MM"),
                };
                return dto;
            }).ToList();

            return result;
        }
    }
}
