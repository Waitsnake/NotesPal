namespace NotesPal.Data
{
    public class PlayerInfo
    {
        public string PlayerName { get; }
        public uint WorldId { get; }
        public uint ContentId { get; }
        public uint ObjectId { get; }

        public PlayerInfo(string playerName, uint worldId, uint contentId, uint objectId)
        {
            PlayerName = playerName;
            WorldId = worldId;
            ContentId = contentId;
            ObjectId = objectId;
        }
    }
}

