using Microsoft.EntityFrameworkCore;
using Archery.Data;
using Archery.Models.Entity;
using Archery.Models.DTO;

namespace Archery.Repository
{
    public class ArcherRepository : IArcherRepository
    {
        private readonly AppDbContext _db;

        public ArcherRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ArcherDTO>> GetAllAsync()
        {
            return await _db.Archers
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
            return await _db.Archers
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
            return await _db.Archers
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

            _db.Archers.Add(entity);
            await _db.SaveChangesAsync();

            // Gắn Equipment
            if (dto.EquipmentIds?.Any() == true)
            {
                foreach (var eqId in dto.EquipmentIds)
                {
                    _db.ArcherEquipments.Add(new ArcherEquipment
                    {
                        ArcherID = entity.ArcherID,
                        EquipmentID = eqId
                    });
                }
                await _db.SaveChangesAsync();
            }

            dto.ArcherId = entity.ArcherID;
            return dto;
        }

        public async Task<bool> UpdateAsync(int archerId, ArcherDTO dto)
        {
            var entity = await _db.Archers
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
                _db.Addresses.Add(newAddress);
                await _db.SaveChangesAsync();
                entity.AddressID = newAddress.AddressID;
            }

            // Update Equipment
            if (dto.EquipmentIds != null)
            {
                // Xóa tất cả equipment cũ
                var oldEquip = _db.ArcherEquipments.Where(ae => ae.ArcherID == entity.ArcherID);
                _db.ArcherEquipments.RemoveRange(oldEquip);

                // Thêm lại equipment mới
                foreach (var eqId in dto.EquipmentIds)
                {
                    _db.ArcherEquipments.Add(new ArcherEquipment
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

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int archerId)
        {
            var entity = await _db.Archers.FindAsync(archerId);
            if (entity == null) return false;

            var eq = _db.ArcherEquipments.Where(ae => ae.ArcherID == archerId);
            _db.ArcherEquipments.RemoveRange(eq);

            _db.Archers.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
