using System;  // For DateTime
using System.Collections.Generic;  // For List<T>
using System.Security.Cryptography;  // For MD5
using System.Text;  // For Encoding
using System.Linq;  // For LINQ methods like Select

namespace NotesPal.Data
{
    public class NoteModel
    {
        public string Id => GetId(Name, WorldId);
        public required string Name { get; set; }
        public required uint WorldId { get; set; }
        public string? NoteText { get; set; }
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public static string GetId(string name, uint worldId) 
            => string.Join(
                "",
                MD5.Create().ComputeHash(
                    Encoding.ASCII.GetBytes($"{name}@{worldId}")
                ).Select(b => $"{b:X2}")
            );
    }
}

