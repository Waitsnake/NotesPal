using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Plugin;
using NotesPal.Data;
using NotesPal.Windows;
using NotesPal.Extensions;

namespace NotesPal.Context
{
    public static class NoteContextAction
    {
        private const char PrefixChar = 'N';  // for notes in context menu 

        public static void Initialize()
        {
            Plugin.ContextMenu.OnMenuOpened += OnMenuOpen;
        }

        private static void OnMenuOpen(IMenuOpenedArgs args)
        {
            if (!IsValidPlayerMenu(args))
                return;

            var player = args.GetPlayer();
            if (player == null)
                return;

            var hasNote = NoteDb.Exists(player.PlayerName, player.WorldId);

            var menuItem = new MenuItem
            {
                PrefixChar = PrefixChar,
                PrefixColor = hasNote ? (ushort)60 : (ushort)31,  // Colors for "Open Note" or "New Note"
                Name = hasNote ? "Open Note" : "New Note",
                OnClicked = OnNoteClicked
            };

            args.AddMenuItem(menuItem);
        }

        private static void OnNoteClicked(IMenuItemClickedArgs args)
        {
            var player = args.GetPlayer();
            if (player == null)
                return;

            NoteEditor.OpenNoteForPlayer(player.PlayerName, player.WorldId);
        }

        public static void Dispose()
        {
            Plugin.ContextMenu.OnMenuOpened -= OnMenuOpen;
        }

        private static bool IsValidPlayerMenu(IMenuOpenedArgs args, bool includeSelf = false)
        {
            if (args.Target is not MenuTargetDefault menuTargetDefault)
                return false;

            switch (args.AddonName)
            {
                case null:
                case "LookingForGroup":
                case "PartyMemberList":
                case "FriendList":
                case "FreeCompany":
                case "SocialList":
                case "ContactList":
                case "ChatLog":
                    if (menuTargetDefault.TargetName != string.Empty)
                    {
                        if (!includeSelf)
                        {
                            var name = Plugin.ObjectCollection.LocalPlayer?.Name.TextValue;
                            var worldId = Plugin.ObjectCollection.LocalPlayer?.HomeWorld.RowId;
                            if (menuTargetDefault.TargetName == name && menuTargetDefault.TargetHomeWorld.RowId == worldId)
                            {
                                Plugin.PluginLog.Verbose("ContextMenu: Self context menu.");
                                return false;
                            }
                        }

                        return true;
                    }

                    return false;
            }

            return false;
        }
    }
}

