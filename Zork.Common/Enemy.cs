using System;

namespace Zork.Common
{
    public class Enemy
    {
        public string Name { get; }

        public string LookDescription { get; }

        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                if (_health != value)
                {
                    _health = value;
                    HealthChanged?.Invoke(this, _health);
                }
            }
        }

        public event EventHandler<int> HealthChanged;

        public Enemy(string name, string lookDescription, int health)
        {
            Name = name;
            LookDescription = lookDescription;
            Health = health;
        }

        public void TakeDamage(int damageAmount)
        {
            _health -= damageAmount;
        }
        public override string ToString() => Name;

        private int _health;
    }
}
