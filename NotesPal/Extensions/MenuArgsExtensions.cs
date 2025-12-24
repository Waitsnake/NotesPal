using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Utility;
using NotesPal.Data;

namespace NotesPal.Extensions
{
    public static class MenuArgsExtensions
    {
        // Retrieves player information from a menu item clicked event
        public static PlayerInfo? GetPlayer(this IMenuItemClickedArgs args)
            => GetPlayerFromTarget(args.Target);

        // Retrieves player information from a menu opened event
        public static PlayerInfo? GetPlayer(this IMenuOpenedArgs args)
            => GetPlayerFromTarget(args.Target);

        // Extracts player information from the target of the context menu
        private static PlayerInfo? GetPlayerFromTarget(object target)
        {
            if (target is not MenuTargetDefault menuTarget)
                return null;

            var playerName = menuTarget.TargetName;
            var worldId = menuTarget.TargetHomeWorld;
            var contentId = menuTarget.TargetContentId;
            var objectId = menuTarget.TargetObjectId;

            // Optional validation (e.g., character name validity check, contentId check)
            if (!playerName.IsValidCharacterName() || contentId == 0 || objectId == 0)
                return null;

            // Return the player info with the necessary details
            return new PlayerInfo(playerName, worldId.RowId, (uint)contentId, (uint)objectId);
        }
    }
}

