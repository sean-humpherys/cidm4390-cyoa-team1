
namespace cidm4390_cyoa_team1
{
    public class Die
    {
        private readonly Random random = new Random();

        public int Roll(int sides)
        {
            if (sides < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sides),
                    "A die must have at least one side.");
            }

            return random.Next(1, sides + 1);
        }
    }
}
