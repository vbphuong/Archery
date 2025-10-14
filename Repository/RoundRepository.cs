using Archery.Data;
using Archery.Models.DTO;
using Archery.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Archery.Repository
{
    public class RoundRepository : IRoundRepository
    {
        private readonly AppDbContext _context;

        public RoundRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<RoundDTO>> GetAllRoundsAsync(int pageNumber, int pageSize)
        {
            var baseQuery = _context.Rounds
                .Include(r => r.Equipment)
                .GroupJoin(
                    _context.EquivalentRounds,
                    r => r.RoundID,
                    e => e.BaseRoundID,
                    (r, eqs) => new { Round = r, Equivalent = eqs.FirstOrDefault() }
                );

            var totalCount = await baseQuery.CountAsync();

            var pagedData = await baseQuery
                .OrderBy(x => x.Equivalent == null ? 0 : 1)
                .ThenBy(x => x.Round.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new RoundDTO
                {
                    RoundID = x.Round.RoundID,
                    Name = x.Round.Name,
                    Class = x.Round.Class,
                    Gender = x.Round.Gender,
                    EquipmentName = x.Round.Equipment != null ? x.Round.Equipment.Type : null,
                    EquivalentRoundName = x.Equivalent != null
                        ? _context.Rounds
                            .Where(er => er.RoundID == x.Equivalent.EquivalentRoundID)
                            .Select(er => er.Name)
                            .FirstOrDefault()
                        : null,
                    Active = x.Equivalent != null ? x.Equivalent.Active : "No",
                    DateRecorded = x.Equivalent != null ? x.Equivalent.DateRecorded : null,
                    TimeRecorded = x.Equivalent != null ? x.Equivalent.TimeRecorded : null
                })
                .ToListAsync();

            return new PagedResult<RoundDTO>
            {
                Data = pagedData,
                TotalCount = totalCount
            };
        }

        public async Task AddEquivalentAsync(int baseRoundId, int equivalentRoundId)
        {
            if (baseRoundId == equivalentRoundId)
                throw new ArgumentException("Base round and equivalent round cannot be the same.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Chỉ vô hiệu hóa những record đang 'Yes'
                var oldRecords = await _context.EquivalentRounds
                    .Where(e => e.BaseRoundID == baseRoundId && e.Active == "Yes")
                    .ToListAsync();

                foreach (var record in oldRecords)
                    record.Active = "No";

                var newEq = new EquivalentRound
                {
                    BaseRoundID = baseRoundId,
                    EquivalentRoundID = equivalentRoundId,
                    DateRecorded = DateTime.Now.Date,
                    TimeRecorded = DateTime.Now.TimeOfDay,
                    Active = "Yes"  
                };

                _context.EquivalentRounds.Add(newEq);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddEquivalentByNameAsync(int baseRoundId, string equivalentRoundName)
        {
            var allowedNames = new[] { "WA90/1440", "WA70/1440", "WA60/1440", "AA50/1440", "AA40/1440" };

            if (!allowedNames.Contains(equivalentRoundName))
                throw new ArgumentException("Invalid equivalent round name.");

            var baseRound = await _context.Rounds.FirstOrDefaultAsync(r => r.RoundID == baseRoundId);
            if (baseRound == null)
                throw new ArgumentException("Base round not found.");

            if (baseRound.Gender != "Male")
                throw new ArgumentException("Base round must be Male.");

            // Kiểm tra Round Female tương ứng có tồn tại chưa
            var equivalentRound = await _context.Rounds.FirstOrDefaultAsync(r =>
                r.Name == equivalentRoundName &&
                r.Gender == "Female" &&
                r.Class == baseRound.Class &&
                r.EquipmentID == baseRound.EquipmentID);

            // Nếu chưa có thì tạo mới
            if (equivalentRound == null)
            {
                equivalentRound = new Round
                {
                    Name = equivalentRoundName,
                    Class = baseRound.Class,
                    Gender = "Female",
                    EquipmentID = baseRound.EquipmentID
                };

                _context.Rounds.Add(equivalentRound);
                await _context.SaveChangesAsync();
            }

            // Gọi lại hàm gốc để tạo EquivalentRound mapping
            await AddEquivalentAsync(baseRound.RoundID, equivalentRound.RoundID);
        }

        public async Task<IEnumerable<RoundDTO>> GetAllRoundNamesAsync()
        {
            return await _context.Rounds
                .Include(r => r.Equipment)
                .Select(r => new RoundDTO
                {
                    RoundID = r.RoundID,
                    Name = r.Name,
                    Class = r.Class,
                    Gender = r.Gender,
                    EquipmentName = r.Equipment != null ? r.Equipment.Type : null
                })
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}