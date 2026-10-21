namespace cidm4390_cyoa_team1
{
    public class Player
    {
        public string name { get; set; }
        public Race race { get; set; }
        public Occupation occupation { get; set; }
        public int strength { get; set; }
        public int agility { get; set; }
        public int health { get; set; }
        public Weapon weapon { get; set; }
        public Player()
        {
            name = "Player";
            race = Race.Human;
            occupation = Occupation.Fighter;
            strength = 10;
            agility = 10;
            health = 20;
            weapon = new Weapon("long sword", 12, "====>");
        }
        public Player(string name, Race race, Occupation occupation)
        {
            this.name = name;
            this.race = race;
            this.occupation = occupation;
            strength = 10;
            agility = 10;
            health = 20;
            weapon = new Weapon("long sword", 12, "====>");
        }
        public string MissedMeTaunt()
        {
            return name + " says: You missed me!";
        }
        public string WasHitTaunt()
        {
            return name + " says: Ouch! You hit me!";
        }
        public string DisplayStats()
        {
            return "Player: " + name + "\n" +
                    "Race: " + race + "\n" +
                    "Occupation: " + occupation + "\n" +
                    "Strength: " + strength + "\n" +
                    "Agility: " + agility + "\n" +
                    "Health: " + health + "\n" +
                    "Weapon: " + weapon.type;

        }
    }
}