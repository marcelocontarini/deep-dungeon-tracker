﻿using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace DeepDungeonTracker;

public class BossStatusTimerData
{
    [JsonInclude]
    public BossStatusTimerItem Combat { get; private set; } = new(BossStatusTimer.Combat);

    [JsonIgnore]
    public Collection<BossStatusTimerItem> Medicated { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("Medicated")]
    public Collection<BossStatusTimerItem>? SerializationMedicated { get => this.Medicated?.Count > 0 ? this.Medicated : null; private set => this.Medicated = value ?? new(); }

    [JsonIgnore]
    public Collection<BossStatusTimerItem> AccursedPox { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("AccursedPox")]
    public Collection<BossStatusTimerItem>? SerializationAccursedPox { get => this.AccursedPox?.Count > 0 ? this.AccursedPox : null; private set => this.AccursedPox = value ?? new(); }

    [JsonIgnore]
    public Collection<BossStatusTimerItem> Weakness { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("Weakness")]
    public Collection<BossStatusTimerItem>? SerializationWeakness { get => this.Weakness?.Count > 0 ? this.Weakness : null; private set => this.Weakness = value ?? new(); }

    [JsonIgnore]
    public Collection<BossStatusTimerItem> BrinkOfDeath { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("BrinkOfDeath")]
    public Collection<BossStatusTimerItem>? SerializationBrinkOfDeath { get => this.BrinkOfDeath?.Count > 0 ? this.BrinkOfDeath : null; private set => this.BrinkOfDeath = value ?? new(); }

    [JsonIgnore]
    public Collection<BossStatusTimerItem> DamageUp { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("DamageUp")]
    public Collection<BossStatusTimerItem>? SerializationDamageUp { get => this.DamageUp?.Count > 0 ? this.DamageUp : null; private set => this.DamageUp = value ?? new(); }

    [JsonIgnore]
    public Collection<BossStatusTimerItem> VulnerabilityDown { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("VulnerabilityDown")]
    public Collection<BossStatusTimerItem>? SerializationVulnerabilityDown { get => this.VulnerabilityDown?.Count > 0 ? this.VulnerabilityDown : null; private set => this.VulnerabilityDown = value ?? new(); }

    [JsonIgnore]
    public Collection<BossStatusTimerItem> VulnerabilityUp { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("VulnerabilityUp")]
    public Collection<BossStatusTimerItem>? SerializationVulnerabilityUp { get => this.VulnerabilityUp?.Count > 0 ? this.VulnerabilityUp : null; private set => this.VulnerabilityUp = value ?? new(); }

    [JsonIgnore]
    public Collection<BossStatusTimerItem> Enervation { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("Enervation")]
    public Collection<BossStatusTimerItem>? SerializationEnervation { get => this.Enervation?.Count > 0 ? this.Enervation : null; private set => this.Enervation = value ?? new(); }

    [JsonIgnore]
    public Collection<BossStatusTimerItem> DamageUpEurekaOrthos { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("DamageUpEurekaOrthos")]
    public Collection<BossStatusTimerItem>? SerializationDamageUpEurekaOrthos { get => this.DamageUpEurekaOrthos?.Count > 0 ? this.DamageUpEurekaOrthos : null; private set => this.DamageUpEurekaOrthos = value ?? new(); }

    [JsonIgnore]
    public Collection<BossStatusTimerItem> VulnerabilityDownEurekaOrthos { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("VulnerabilityDownEurekaOrthos")]
    public Collection<BossStatusTimerItem>? SerializationVulnerabilityDownEurekaOrthos { get => this.VulnerabilityDownEurekaOrthos?.Count > 0 ? this.VulnerabilityDownEurekaOrthos : null; private set => this.VulnerabilityDownEurekaOrthos = value ?? new(); }

    public void Update(BattleChara? enemy)
    {
        var vulnerabilityUp = this.VulnerabilityUp.LastOrDefault();
        if (vulnerabilityUp != null)
        {
            var stacks = enemy?.StatusList.FirstOrDefault(x => x.StatusId == 714)?.StackCount ?? 0;
            vulnerabilityUp.StacksUpdate(Math.Max(vulnerabilityUp.Stacks, stacks));
        }
    }

    public int TimersCount()
    {
        return 1 +
            this.Medicated.DistinctBy(x => x.BossStatusTimer).Count() +
            this.AccursedPox.DistinctBy(x => x.BossStatusTimer).Count() +
            this.Weakness.DistinctBy(x => x.BossStatusTimer).Count() +
            this.BrinkOfDeath.DistinctBy(x => x.BossStatusTimer).Count() +
            this.DamageUp.DistinctBy(x => x.BossStatusTimer).Count() +
            this.VulnerabilityDown.DistinctBy(x => x.BossStatusTimer).Count() +
            (this.VulnerabilityUp?.DistinctBy(x => x.Stacks).Count() ?? 0) +
            this.Enervation.DistinctBy(x => x.BossStatusTimer).Count() +
            this.DamageUpEurekaOrthos.DistinctBy(x => x.BossStatusTimer).Count() +
            this.VulnerabilityDownEurekaOrthos.DistinctBy(x => x.BossStatusTimer).Count();
    }

    public void TimerEnd()
    {
        if (this.Combat.HasEnded())
            return;

        foreach (var item in this.Medicated)
            item.TimerEnd();

        foreach (var item in this.AccursedPox)
            item.TimerEnd();

        foreach (var item in this.Weakness)
            item.TimerEnd();

        foreach (var item in this.BrinkOfDeath)
            item.TimerEnd();

        foreach (var item in this.DamageUp)
            item.TimerEnd();

        foreach (var item in this.VulnerabilityDown)
            item.TimerEnd();

        foreach (var item in this.VulnerabilityUp)
            item.TimerEnd();

        foreach (var item in this.Enervation)
            item.TimerEnd();

        foreach (var item in this.DamageUpEurekaOrthos)
            item.TimerEnd();

        foreach (var item in this.VulnerabilityDownEurekaOrthos)
            item.TimerEnd();

        this.Combat.TimerEnd();
    }
}