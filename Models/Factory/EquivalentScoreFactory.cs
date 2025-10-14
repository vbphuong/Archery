using Archery.Models.Entity;

namespace Archery.Models.Factory
{
    public class EquivalentScoreFactory : IScoreFactory
    {
        public Score CreateScore(int archerId, int roundId, int competitionId)
        {
            return new Score
            {
                ArcherID = archerId,
                RoundID = roundId,
                CompetitionID = competitionId,
                TotalScore = 0,
                DateRecorded = DateTime.Now,
                ApprovalStatus = "AutoApproved" // ví dụ
            };
        }
    }
}

