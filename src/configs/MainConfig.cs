// src/configs/MainConfig.cs
using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace SimpleZombieMode.Configs;

public class MainConfig : BasePluginConfig
{
	// Timers -->>
	[JsonPropertyName("TimerStartInfection")]
	public int      TimerStartInfection     { get; set; } = 15;

	[JsonPropertyName("TimerRound")]
	public int      TimerRound              { get; set; } = 900;

	[JsonPropertyName("TimerRestartGame")]
	public int      TimerRestartGame        { get; set; } = 3;


	// Zombies -->>
	[JsonPropertyName("ZombieHealth")]
	public int      ZombieHealth            { get; set; } = 999;

	[JsonPropertyName("ZombieSpeed")]
	public float    ZombieSpeed             { get; set; } = 1.3f;

	[JsonPropertyName("ZombieLives")]
	public int      ZombieLives             { get; set; } = 3;

	[JsonPropertyName("ZombieRespawnDelay")]
	public float    ZombieRespawnDelay      { get; set; } = 0.5f;

	[JsonPropertyName("ZombieHealOnKill")]
	public int		ZombieHealOnKill		{ get; set; } = 50;


	// Humans -->>
	[JsonPropertyName("HumanHealth")]
	public int      HumanHealth             { get; set; } = 100;

	[JsonPropertyName("HumanSpeed")]
	public float    HumanSpeed              { get; set; } = 1.0f;


	// Survivor -->>
	[JsonPropertyName("SurvivorHealth")]
	public int      SurvivorHealth          { get; set; } = 500;

	[JsonPropertyName("SurvivorSpeed")]
	public float    SurvivorSpeed           { get; set; } = 1.25f;


	// Misc
	[JsonPropertyName("MinPlayers")]
	public int      MinPlayers              { get; set; } = 4;
}