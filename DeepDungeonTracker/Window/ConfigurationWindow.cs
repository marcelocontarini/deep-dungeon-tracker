﻿using ImGuiNET;
using System;
using System.Reflection.Emit;
using System.Text.Json;

namespace DeepDungeonTracker
{
    public sealed class ConfigurationWindow : WindowEx, IDisposable
    {
        private Data Data { get; }

        private string[] FieldNames { get; init; }

        public ConfigurationWindow(string id, Configuration configuration, Data data) : base(id, configuration, ImGuiWindowFlags.AlwaysAutoResize)
        {
            this.Data = data;
            this.FieldNames = new string[] { "Kills", "Mimics", "Mandragoras", "Mimicgoras", "NPCs", "Coffers", "Enchantments", "Traps", "Deaths", "Regen Potions", "Potsherds", "Lurings", "Maps", "Time Bonuses" };
            this.SizeConstraints = new() { MaximumSize = new(600.0f, 600.0f) };
        }

        public void Dispose() { }

        public override void Draw()
        {
            if (ImGui.BeginTabBar("Tab Bar"))
            {
                if (ImGui.BeginTabItem("General"))
                {
                    this.General();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Tracker"))
                {
                    this.Tracker();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Floor Set Time"))
                {
                    this.FloorSetTime();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Score"))
                {
                    this.Score();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Statistics"))
                {
                    this.Statistics();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Information"))
                {
                    this.Information();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }

            ImGui.Separator();
            this.Button(this.Configuration.Reset, "Reset all settings to default", true);
        }

        private void General()
        {
            var config = this.Configuration.General;
            this.CheckBox(config.ShowAccurateTargetHPPercentage, x => config.ShowAccurateTargetHPPercentage = x, "Show accurate target HP %");
            WindowEx.Tooltip("It doesn't apply to Focus Target.");
            this.CheckBox(config.SolidBackgroundWindow, x => config.SolidBackgroundWindow = x, "Force solid background to all windows");
            this.CheckBox(config.UseInGameCursor, x => config.UseInGameCursor = x, "Use in game cursor");
        }

        private void Tracker()
        {
            var config = this.Configuration.Tracker;
            this.CheckBox(config.Lock, x => config.Lock = x, "Lock");
            ImGui.SameLine();
            this.CheckBox(config.Show, x => config.Show = x, "Show");
            WindowEx.Tooltip("You need to be either inside a Deep Dungeon or in the area outside to get into it.");
            ImGui.SameLine();
            this.CheckBox(config.ShowInBetweenFloors, x => config.ShowInBetweenFloors = x, "Show in between floors");
            this.CheckBox(config.ShowFloorEffectPomanders, x => config.ShowFloorEffectPomanders = x, "Show floor effect pomanders");
            WindowEx.Tooltip("Show pomander icons (at the top left of the window) representing their effect on the current floor.");
            this.DragFloat(config.Scale, x => config.Scale = x, "Scale", 0.01f, 0.25f, 2.0f, "%.2f");
            this.CheckBox(config.IsFloorNumberVisible, x => config.IsFloorNumberVisible = x, "##IsFloorNumberVisible");
            ImGui.SameLine();
            this.ColorEdit4(config.FloorNumberColor, x => config.FloorNumberColor = x, "Floor");
            ImGui.SameLine();
            this.CheckBox(config.IsSetNumberVisible, x => config.IsSetNumberVisible = x, "##IsSetNumberVisible");
            ImGui.SameLine();
            this.ColorEdit4(config.SetNumberColor, x => config.SetNumberColor = x, "Set");
            ImGui.SameLine();
            this.CheckBox(config.IsTotalNumberVisible, x => config.IsTotalNumberVisible = x, "##IsTotalNumberVisible");
            ImGui.SameLine();
            this.ColorEdit4(config.TotalNumberColor, x => config.TotalNumberColor = x, "Total");

            var left = ImGui.GetCursorPosX();
            for (var i = 0; i < config.Fields?.Length; i++)
            {
                var field = config.Fields[i];
                var fieldName = this.FieldNames[field.Index];
                var lastIndex = config.Fields.Length - 1;

                this.ArrowButton(() =>
                {
                    var source = i;
                    var target = i > 0 ? i - 1 : lastIndex;
                    (config.Fields[source], config.Fields[target]) = (config.Fields[target], config.Fields[source]);
                }, $"##Up{fieldName}", ImGuiDir.Up, true);
                ImGui.SameLine();

                this.ArrowButton(() =>
                {
                    var source = i;
                    var target = i < lastIndex ? i + 1 : 0;
                    (config.Fields[source], config.Fields[target]) = (config.Fields[target], config.Fields[source]);
                }, $"##Down{fieldName}", ImGuiDir.Down, true);
                ImGui.SameLine();

                this.CheckBox(field.Show, x => field.Show = x, $"#{i + 1:D2} {fieldName}");
            }
        }

        private void FloorSetTime()
        {
            var config = this.Configuration.FloorSetTime;
            this.CheckBox(config.Lock, x => config.Lock = x, "Lock");
            ImGui.SameLine();
            this.CheckBox(config.Show, x => config.Show = x, "Show");
            WindowEx.Tooltip("You need to be either inside a Deep Dungeon or in the area outside to get into it.");
            ImGui.SameLine();
            this.CheckBox(config.ShowInBetweenFloors, x => config.ShowInBetweenFloors = x, "Show in between floors");
            ImGui.SameLine();
            this.CheckBox(config.ShowTitle, x => config.ShowTitle = x, "Show title");
            this.CheckBox(config.ShowFloorTime, x => config.ShowFloorTime = x, "Show floor time");
            this.DragFloat(config.Scale, x => config.Scale = x, "Scale", 0.01f, 0.25f, 2.0f, "%.2f");
            this.ColorEdit4(config.PreviousFloorTimeColor, x => config.PreviousFloorTimeColor = x, "Previous Floor Time");
            ImGui.SameLine();
            this.ColorEdit4(config.CurrentFloorTimeColor, x => config.CurrentFloorTimeColor = x, "Current Floor Time");
            this.ColorEdit4(config.AverageTimeColor, x => config.AverageTimeColor = x, "Average Time");
            ImGui.SameLine();
            this.ColorEdit4(config.RespawnTimeColor, x => config.RespawnTimeColor = x, "Respawn Time");
        }

        private void Score()
        {
            var config = this.Configuration.Score;
            this.CheckBox(config.Lock, x => config.Lock = x, "Lock");
            ImGui.SameLine();
            this.CheckBox(config.Show, x => config.Show = x, "Show");
            WindowEx.Tooltip("You need to be either inside a Deep Dungeon or in the area outside to get into it.");
            ImGui.SameLine();
            this.CheckBox(config.ShowInBetweenFloors, x => config.ShowInBetweenFloors = x, "Show in between floors");
            ImGui.SameLine();
            this.CheckBox(config.ShowTitle, x => config.ShowTitle = x, "Show title");
            this.CheckBox(config.IncludeFloorCompletion, x => config.IncludeFloorCompletion = x, "Include floor completion");
            WindowEx.Tooltip("Include all floor completion-related score up to the floor where it shows the next score result window.");
            this.DragFloat(config.Scale, x => config.Scale = x, "Scale", 0.01f, 0.25f, 2.0f, "%.2f");
            this.CheckBox(config.IsFlyTextScoreVisible, x => config.IsFlyTextScoreVisible = x, "##IsFlyTextScoreVisible");
            WindowEx.Tooltip("When the score changes, a Fly Text will be shown.");
            ImGui.SameLine();
            this.ColorEdit4(config.FlyTextScoreColor, x => config.FlyTextScoreColor = x, "Fly Text Score");
        }

        private void Statistics()
        {
            var config = this.Configuration.Statistics;
            this.DragFloat(config.Scale, x => config.Scale = x, "Scale", 0.01f, 0.25f, 2.0f, "%.2f");
            this.ColorEdit4(config.FloorTimeColor, x => config.FloorTimeColor = x, "Floor Time");
            ImGui.SameLine();
            this.ColorEdit4(config.ScoreColor, x => config.ScoreColor = x, "Score");
            ImGui.NewLine();

            var saveSlotSelection = this.Data.Common.SaveSlotSelection.Data;
            if (saveSlotSelection?.Count > 0)
            {
                var statistics = this.Data.Statistics;
                this.ArrowButton(statistics.FloorSetStatisticsPrevious, "##Up", ImGuiDir.Up, false);
                ImGui.SameLine();

                this.ArrowButton(statistics.FloorSetStatisticsNext, "##Down", ImGuiDir.Down, false);
                ImGui.SameLine();

                if (this.Combo(statistics.FloorSetStatistics, x => statistics.FloorSetStatistics = x, "##FloorSetStatistics").Item1)
                    statistics.DataUpdate();

                foreach (var key in saveSlotSelection.Keys)
                {
                    ImGui.NewLine();
                    ImGui.Text($"{key.Replace("-", "@")}");
                    saveSlotSelection.TryGetValue(key, out var SaveSlotSelectionData);
                    foreach (var deepDungeon in Enum.GetValues<DeepDungeon>())
                    {
                        if (deepDungeon == DeepDungeon.None)
                            continue;

                        ImGui.Text($"{deepDungeon.GetDescription()}:");
                        for (var saveSlotNumber = 1; saveSlotNumber <= 2; saveSlotNumber++)
                        {
                            ImGui.SameLine();
                            var fileName = DataCommon.GetSaveSlotFileName(key, new(deepDungeon, saveSlotNumber));
                            var saveSlotFileExists = LocalStream.Exists(ServiceUtility.ConfigDirectory, fileName);
                            if ((!this.Data.IsInsideDeepDungeon && saveSlotFileExists) || (this.Data.IsInsideDeepDungeon && this.Data.Common.GetSaveSlotFileName(SaveSlotSelectionData ?? new()) == fileName))
                            {
                                this.SmallButton(() =>
                                {
                                    if (!this.Data.IsInsideDeepDungeon)
                                        this.Data.Common.LoadDeepDungeonData(key, new(deepDungeon, saveSlotNumber));

                                    statistics.Load(this.Data.Common.CurrentSaveSlot);
                                }, $"Save Slot {saveSlotNumber}##{fileName}", false);
                            }
                            else
                                ImGui.TextColored((this.Data.IsInsideDeepDungeon && saveSlotFileExists) ? Color.Red : Color.Gray, $"Save Slot {saveSlotNumber}");
                        }
                    }
                }
            }
            else
                ImGui.TextColored(Color.Gray, "No save slots!");
        }

        private void Information()
        {
            ImGui.TextColored(Color.Green, "Kills:");
            ImGui.TextWrapped(
                "All enemies killed from a distance of more than two rooms cannot be counted. " +
                "If you use magicite, do so in the center of the floor, covering all enemies killed (as much as possible). " +
                "\nKilling an enemy takes almost two seconds to be counted. If you kill a boss, don't leave as soon as possible, or you could miss it.");
            ImGui.TextColored(Color.Green, "Cairn of Passage Kills:");
            ImGui.TextWrapped("Keep your map menu open for the Cairn of Passage key status check. Killing too many enemies at the same time can be inaccurate.");
            ImGui.TextColored(Color.Green, "Maps:");
            ImGui.TextWrapped("Keep your map menu open to count the map reveal.");
            ImGui.TextColored(Color.Green, "Potsherds:");
            ImGui.TextWrapped("Only Potsherds obtained from bronze coffers will be counted.");
            ImGui.TextColored(Color.Green, "Score:");
            ImGui.TextWrapped("The number shown in the Score Window is the Duty Complete value.\nThe score will be zero if you track it from an ongoing save file.");
            if (ImGui.CollapsingHeader("OpCodes"))
            {
                ImGui.TextWrapped("Values in this section should not be zero.");
                ImGui.TextWrapped($"{JsonSerializer.Serialize(this.Configuration.OpCodes, new JsonSerializerOptions() { WriteIndented = true, })}");
            }
        }
    }
}