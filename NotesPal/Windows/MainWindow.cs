using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ImGuiFileDialog;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Text.Json;
using NotesPal.Data;

namespace NotesPal.Windows
{
    public class MainWindow : Window
    {
        private static MainWindow? _instance;
        public static MainWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainWindow();
                    Services.Instance.WindowSystem.AddWindow(_instance);
                }
                return _instance;
            }
        }

        private string searchTerm = string.Empty;
        private List<NoteModel> allNotes = new();
        private readonly FileDialogManager fileDialog = new();
        private bool showDeleteAllConfirmation = false;

        private MainWindow() : base("NotesPal v{Plugin.Version}", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize)
        {
            Size = new System.Numerics.Vector2(600, 500);
        }

        public override void OnOpen()
        {
            // Load data once when opening the window
            allNotes = NoteDb.GetAll().OrderBy(n => n.Name).ToList();
        }

		public override void Draw()
		{
			// Import and Export buttons first
			if (ImGui.Button("Import Notes"))
			{
				fileDialog.OpenFileDialog(
					"Import Notes JSON",
					"JSON files{.json},.*",
					(ok, path) =>
					{
						if (ok && !string.IsNullOrEmpty(path))
						{
							try
							{
								var imported = JsonSerializer.Deserialize<List<NoteModel>>(File.ReadAllText(path));
								if (imported != null)
								{
									foreach (var n in imported)
										NoteDb.Upsert(n, updateTimestamp: false);
									ReloadNotes();
								}
							}
							catch (Exception ex)
							{
								Logger.Error($"Import failed: {ex}");
							}
						}
					});
			}

			ImGui.SameLine();
			if (ImGui.Button("Export Notes"))
			{
				fileDialog.SaveFileDialog(
					"Export Notes JSON",
					"JSON files{.json},.*",
					"notes_export.json",
					"json",
					(ok, path) =>
					{
						if (ok && !string.IsNullOrEmpty(path))
						{
							try
							{
								var json = JsonSerializer.Serialize(allNotes, new JsonSerializerOptions { WriteIndented = true });
								File.WriteAllText(path, json);
							}
							catch (Exception ex)
							{
								Logger.Error($"Export failed: {ex}");
							}
						}
					});
			}

			ImGui.SameLine();
			if (ImGui.Button("Delete All Notes"))
			{
				showDeleteAllConfirmation = true;
			}

			// Delete All button confirmation
			if (showDeleteAllConfirmation)
			{
				ImGui.OpenPopup("Confirm Delete All");
				if (ImGui.BeginPopupModal("Confirm Delete All", ref showDeleteAllConfirmation, ImGuiWindowFlags.AlwaysAutoResize))
				{
					ImGui.Text("Are you sure you want to delete all notes? This action cannot be undone.");
					ImGui.Separator();

					if (ImGui.Button("Yes"))
					{
						NoteDb.DeleteAll();
						allNotes.Clear();
						showDeleteAllConfirmation = false;
					}

					ImGui.SameLine();
					if (ImGui.Button("No"))
					{
						showDeleteAllConfirmation = false;
					}

					ImGui.EndPopup();
				}
			}

			// Search box
			ImGui.Separator(); // Optional: adds a separator for better visual separation
			ImGui.Text("Search:");
			ImGui.SameLine();
			ImGui.InputText("##search", ref searchTerm, 64);

			// Filter notes based on search term
			var filtered = string.IsNullOrWhiteSpace(searchTerm)
				? allNotes
				: allNotes.Where(n => n.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
									  (n.NoteText?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true))
						  .ToList();

			// Display note list in a table
			if (ImGui.BeginTable("noteTable", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.Resizable))
			{
				ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
				ImGui.TableSetupColumn("World", ImGuiTableColumnFlags.WidthFixed, 160); // Added column for World name
				ImGui.TableSetupColumn("Last Modified", ImGuiTableColumnFlags.WidthFixed, 160);
				ImGui.TableHeadersRow();

				foreach (var note in filtered)
				{
					ImGui.TableNextRow();

					ImGui.TableSetColumnIndex(0);
					if (ImGui.Selectable(note.Name, false))
					{
						// Open the note in editor
						NoteEditor.OpenNote(note);
					}

					ImGui.TableSetColumnIndex(1);
					ImGui.Text(WorldNames.GetWorldName(note.WorldId)); // Use the WorldNames.GetWorldName method

					ImGui.TableSetColumnIndex(2);
					ImGui.Text(note.LastModified.ToString("yyyy-MM-dd HH:mm"));
				}

				ImGui.EndTable();
			}

			// Display total count of notes at the bottom
			ImGui.Separator(); // Optional: adds a separator for visual separation
			ImGui.Text($"Total Notes: {allNotes.Count}");
		}



        public void ReloadNotes()
        {
            allNotes = NoteDb.GetAll().OrderBy(n => n.Name).ToList();
        }
    }
}

