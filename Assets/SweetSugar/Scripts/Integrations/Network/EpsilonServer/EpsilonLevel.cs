namespace SweetSugar.Scripts.Integrations.Network.EpsilonServer
{
    public class EpsilonLevel
    {
        public int level;
        public int stars;
        public int score;

        public EpsilonLevel(int level, int stars, int score)
        {
            this.level = level;
            this.stars = stars;
            this.score = score;
        }

        public bool IsGreatThen(EpsilonLevel other)
        {
            return this.stars > other.stars || this.score > other.score;
        }
    }
}