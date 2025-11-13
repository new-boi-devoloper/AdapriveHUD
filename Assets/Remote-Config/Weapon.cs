using System;

namespace Remote_Config
{
    [Serializable]
    public class Weapon
    {
        public string id;
        public int damage;
        public float cooldown;
    
        public Weapon(string id, int damage, float cooldown)
        {
            this.id = id;
            this.damage = damage;
            this.cooldown = cooldown;
        }
    
        public override string ToString()
        {
            return $"Weapon {id}: Damage={damage}, Cooldown={cooldown}";
        }
    }
}