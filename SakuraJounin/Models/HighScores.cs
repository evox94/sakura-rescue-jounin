using static SakuraJounin.Models.HighScores;

namespace SakuraJounin.Models
{
    public record HighScores(HighScore[] Values)
    {
        public record HighScore(string Name, double Count)
        {
        }
    }
}
