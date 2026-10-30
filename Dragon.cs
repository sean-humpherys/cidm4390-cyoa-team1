
using CYOA;

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

        public string DisplayStats(Language language = Language.English)
        {
            string dragonLabel = language switch
            {
                Language.Spanish => "Dragón",
                Language.French => "Dragon",
                _ =>"Dragon"
            };

            string strengthLabel = language switch
            {
                Language.Spanish => "Fuerza",
                Language.French => "Force",
        _       => "Strength"
            };

            string healthLabel = language switch
            {
                Language.Spanish => "Puntos de salud",
                Language.French => "Points de vie",
                _ => "Health"
            };

            string agilityLabel = language switch
            {
                Language.Spanish => "Agilidad",
                Language.French => "Agilité",
                _ => "Agility"
            };

            string weaponLabel = language switch
            {
                Language.Spanish => "Arma",
                Language.French => "Arme",
                _ => "Weapon"
            };

            string weaponName = language switch
            {
                Language.Spanish => "garras",
                Language.French => "griffes",
                _ => weapon.type
            };

            return $"{dragonLabel}: {name}\n" +
                   $"{strengthLabel}: {strength}\n" +
                   $"{healthLabel}: {health}\n" +
                   $"{agilityLabel}: {agility}\n" +
                   $"{weaponLabel}: {weaponName}";
        }
    }
}