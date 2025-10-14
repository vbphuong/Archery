
namespace Archery.Models.Factory
{
    public static class ScoreFactoryProducer
    {
        public static IScoreFactory GetFactory(bool hasEquivalent)
        {
            if (hasEquivalent)
                return new EquivalentScoreFactory();
            return new NormalScoreFactory();
        }
    }
}

