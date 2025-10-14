using Archery.Models.Entity;
namespace Archery.Models.Factory
{
    public interface IScoreFactory
    {
        Score CreateScore(int archerId, int roundId, int competitionId);
    }
}
