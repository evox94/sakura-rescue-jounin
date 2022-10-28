namespace SakuraJounin.Models
{
    public class GameSettings
    {
        public GameSettings()
        {
            RedisKey = string.Empty;
        }

        public string RedisKey { get; set; }

        public int RequiredGenins { get; set; }

        public int MaxAttempts { get; set; }

        public int GeninBackoff { get; set; }

    }
}
