using Archery.Data;
using Archery.Models.DTO;
using Archery.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Archery.Repository
{
    public class ArrowRepository : IArrowRepository
    {
        private readonly AppDbContext _context;

        public ArrowRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArrowDTO>> GetAllAsync()
        {
            return await _context.Arrows
                .Include(a => a.End)
                .Select(a => new ArrowDTO
                {
                    ArrowID = a.ArrowID,
                    Value = a.Value,
                    EndID = a.EndID,
                    EndNumber = a.End != null ? a.End.EndNumber : null,
                    RangeID = a.End != null ? a.End.RangeID : null
                })
                .ToListAsync();
        }

        public async Task<ArrowDTO?> GetByIdAsync(int id)
        {
            var arrow = await _context.Arrows
                .Include(a => a.End)
                .FirstOrDefaultAsync(a => a.ArrowID == id);

            if (arrow == null) return null;

            return new ArrowDTO
            {
                ArrowID = arrow.ArrowID,
                Value = arrow.Value,
                EndID = arrow.EndID,
                EndNumber = arrow.End?.EndNumber,
                RangeID = arrow.End?.RangeID
            };
        }

        public async Task<IEnumerable<ArrowDTO>> GetByEndAsync(int endId)
        {
            return await _context.Arrows
                .Where(a => a.EndID == endId)
                .Select(a => new ArrowDTO
                {
                    ArrowID = a.ArrowID,
                    Value = a.Value,
                    EndID = a.EndID,
                    EndNumber = a.End != null ? a.End.EndNumber : null,
                    RangeID = a.End != null ? a.End.RangeID : null
                })
                .ToListAsync();
        }

        public async Task<ArrowDTO> CreateAsync(ArrowDTO dto)
        {
            if (dto.EndID == null)
                throw new ArgumentException("EndID is required when creating an Arrow.");

            var arrow = new Arrow
            {
                Value = dto.Value,
                EndID = dto.EndID.Value
            };

            _context.Arrows.Add(arrow);
            await _context.SaveChangesAsync();

            dto.ArrowID = arrow.ArrowID;
            return dto;
        }


        public async Task<bool> UpdateAsync(int id, ArrowDTO dto)
        {
            var arrow = await _context.Arrows.FindAsync(id);
            if (arrow == null) return false;

            arrow.Value = dto.Value;

            _context.Arrows.Update(arrow);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var arrow = await _context.Arrows.FindAsync(id);
            if (arrow == null) return false;

            _context.Arrows.Remove(arrow);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}