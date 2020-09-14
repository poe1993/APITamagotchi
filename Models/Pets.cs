using System;

namespace APITamagotchi.Models
{
    public class Pets
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; } = new DateTime();
        public int HungerLevel { get; set; } = 0;
        public int HappinessLevel { get; set; } = 0;
    }

    public class PlayTime
    {
        public int Id { get; set; }
        public DateTime When { get; set; } = new DateTime();
        public int PetId { get; set; }
        public Pets Pet { get; set; }
    }

    public class Feedings
    {
        public int Id { get; set; }
        public DateTime When { get; set; } = new DateTime();
        public int PetId { get; set; }
        public Pets Pet { get; set; }
    }

    public class Scoldings
    {
        public int Id { get; set; }
        public DateTime When { get; set; } = new DateTime();
        public int PetId { get; set; }
        public Pets Pet { get; set; }
    }
}