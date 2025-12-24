using System.Reflection;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using System.Linq;
using NotesPal.Data;
using NotesPal.Windows;

namespace NotesPal.Commands
{
    public static class CommandCreator
    {
        public static void Initialize()
        {
            Services.Instance.CommandManager.AddHandler("/note", new CommandInfo(OnOpenEditorCommand)
            {
                HelpMessage = "Opens the note editor for the currently-targeted player.",
                ShowInHelp = true
            });
            Services.Instance.CommandManager.AddHandler("/notes", new CommandInfo(OnOpenMainWindowCommand)
            {
                HelpMessage = "Opens the main NotesPal window.",
                ShowInHelp = true
            });
        }

        private static void OnOpenEditorCommand(string name, string args)
        {
            if (!TryOpenEditor())
                OnOpenMainWindowCommand(name, args);
        }

        private static bool TryOpenEditor()
        {
            var localPlayer = Services.Instance.ObjectTable.LocalPlayer;
            if (localPlayer == null)
                return false;

            var targetId = localPlayer.TargetObjectId;
            if (targetId == 0)
                return false;

            var gameObject = Services.Instance.ObjectTable.FirstOrDefault(o => o.GameObjectId == targetId);
            if (gameObject == null)
                return false;

            if (gameObject.ObjectKind != ObjectKind.Player)
                return false;

            if (gameObject is IPlayerCharacter targetPlayer)
            {
                var name = targetPlayer.Name.TextValue;
                var worldId = targetPlayer.HomeWorld;
                NoteEditor.OpenNoteForPlayer(name, worldId.RowId);
                return true;
            }

            return false;
        }

        private static void OnOpenMainWindowCommand(string name, string args)
        {
            MainWindow.Instance.IsOpen = true;
        }
    }
}

