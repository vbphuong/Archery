using Archery.Models.Entity;

namespace Archery.Models.Factory
{
    public class NormalScoreFactory : IScoreFactory
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
                ApprovalStatus = "Pending"
            };
        }
    }
}

