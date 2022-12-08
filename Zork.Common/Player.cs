using System;
using System.Collections.Generic;

namespace Zork.Common
{
    public class Player
    {
        public EventHandler<Room> LocationChanged;

        public event EventHandler<int> ScoreChanged;

        public event EventHandler<int> MovesChanged;

        public event EventHandler<int> HealthChanged;

        public Room CurrentRoom
        {
            get => _currentRoom;
            set
            {
                if(_currentRoom != value)
                {
                    _currentRoom = value;
                    LocationChanged?.Invoke(this, _currentRoom);
                }
            }
        }

        public int Moves
        {
            get
            {
                return _moves;
            }
            set
            {
                if (_moves != value)
                {
                    _moves = value;
                    MovesChanged?.Invoke(this, _moves);
                }
            }
        }

        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                if (_score != value)
                {
                    _score = value;
                    ScoreChanged?.Invoke(this, _score);
                }
            }
        }

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
        public IEnumerable<Item> Inventory => _inventory;

        public Player(World world, string startingLocation, int health)
        {
            _world = world;
            Health = health;

            if (_world.RoomsByName.TryGetValue(startingLocation, out _currentRoom) == false)
            {
                throw new Exception($"Invalid starting location: {startingLocation}");
            }

            _inventory = new List<Item>();
        }

        public bool Move(Directions direction)
        {
            bool didMove = _currentRoom.Neighbors.TryGetValue(direction, out Room neighbor);
            if (didMove)
            {
                CurrentRoom = neighbor;
            }

            return didMove;
        }

        public void AddItemToInventory(Item itemToAdd)
        {
            if (_inventory.Contains(itemToAdd))
            {
                throw new Exception($"Item {itemToAdd} already exists in inventory.");
            }

            _inventory.Add(itemToAdd);
        }

        public void RemoveItemFromInventory(Item itemToRemove)
        {
            if (_inventory.Remove(itemToRemove) == false)
            {
                throw new Exception("Could not remove item from inventory.");
            }
        }

        public void TakeDamage(int damageAmount)
        {
            _health -= damageAmount;
        }

        private readonly World _world;
        private Room _currentRoom;
        private readonly List<Item> _inventory;
        private int _moves;
        private int _score;
        private int _health;
    }
}
