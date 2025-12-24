using System;

namespace NotesPal
{
	public static class Logger
	{
		// Loggt eine allgemeine Info-Nachricht
		public static void Info(string message) => Services.Instance.PluginLog.Info(message);

		// Loggt eine Debug-Nachricht
		public static void Debug(string message) => Services.Instance.PluginLog.Debug(message);

		// Loggt eine detaillierte (Verbose) Nachricht
		public static void Verbose(string message) => Services.Instance.PluginLog.Verbose(message);

		// Loggt eine Warnung
		public static void Warn(string message) => Services.Instance.PluginLog.Warning(message);

		// Loggt eine Fehlermeldung
		public static void Error(string message) => Services.Instance.PluginLog.Error(message);

		// Loggt eine Ausnahme mit Nachricht und StackTrace
		public static void Exception(string? message, Exception e)
		{
			Error($"EXCEPTION! {message} ({e.Message})");
			Error(e.StackTrace ?? "(null)");
		}
	}
}

