
namespace cidm4390_cyoa_team1
{
    public class Weapon
    {
        public string type { get; set; }
        public int maxDamage { get; set; }
        public string asciiArt { get; set; }

        public Weapon(string type, int maxDamage, string ASCIIart)
        {
            this.type = type;
            this.maxDamage = maxDamage;
            this.asciiArt = ASCIIart;
        }
    }
}
