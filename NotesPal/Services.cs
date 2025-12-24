using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.IoC;

namespace NotesPal
{
    public class Services
    {
        public static Services Instance = null!;

        // Plugin Interface
        public required IDalamudPluginInterface PluginInterface { get; set; }

        // Window Management
        public required WindowSystem WindowSystem { get; set; }

        // Plugin Services (Injected)
        [PluginService] public required IChatGui ChatGui { get; set; }
        [PluginService] public required ICommandManager CommandManager { get; set; }
        [PluginService] public required IObjectTable ObjectTable { get; set; }
        [PluginService] public required IPluginLog PluginLog { get; set; }

    }
}

