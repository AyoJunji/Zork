using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Zork.Common
{
    public class World
    {
        public Item[] Items { get; }
        [JsonIgnore]
        public IReadOnlyDictionary<string, Item> ItemsByName => _itemsByName;

        public Room[] Rooms { get; }
        [JsonIgnore]
        public IReadOnlyDictionary<string, Room> RoomsByName => _roomsByName;

        public World(Room[] rooms, Item[] items)
        {
            Items = items;
            _itemsByName = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
            foreach (Item item in Items)
            {
                _itemsByName.Add(item.Name, item);
            }

            Rooms = rooms;
            _roomsByName = new Dictionary<string, Room>(StringComparer.OrdinalIgnoreCase);
            foreach (Room room in rooms)
            {
                _roomsByName.Add(room.Name, room);
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            foreach (Room room in Rooms)
            {
                room.UpdateInventory(this);
                room.UpdateNeighbors(this);
            }
        }

        private readonly Dictionary<string, Item> _itemsByName;
        private readonly Dictionary<string, Room> _roomsByName;
        
    }
}