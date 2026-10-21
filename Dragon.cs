
namespace cidm4390_cyoa_team1
{
    public class Dragon
    {
        public string name { get; set; }
        public int strength { get; set; }
        public int health { get; set; }
        public int agility { get; set; }
        public Weapon weapon { get; set; }

        public Dragon(string name)
        {
            this.name = name;

            Die die = new Die();

            strength = die.Roll(20);
            health = die.Roll(20);
            agility = die.Roll(20);

            weapon = new Weapon("claws", 12, "###");
        }

        public string MissedMeTaunt()
        {
            return $"{name} roars: You missed me!";
        }

        public string WasHitTaunt()
        {
            return $"{name} growls: You'll regret that!";
        }

        public string DisplayStats()
        {
            return $"Dragon: {name}\n" +
                   $"Strength: {strength}\n" +
                   $"Health: {health}\n" +
                   $"Agility: {agility}\n" +
                   $"Weapon: {weapon.type}";
        }
    }
}
