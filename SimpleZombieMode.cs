// SimpleZombieMode.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using SimpleZombieMode.Configs;
using SimpleZombieMode.Services;

namespace SimpleZombieMode;

public partial class SimpleZombieMode : BasePlugin, IPluginConfig<MainConfig>
{
	public override string ModuleName => "SimpleZombieMode";
	public override string ModuleVersion => "0.0.2";
	public override string ModuleAuthor => "t.me/kotyarakryt";
    public override string ModuleDescription => "A simple and clean Zombie Mode for CS2 — infection system, respawn lives, last survivor bonus and more. Built with CounterStrikeSharp.";

	public MainConfig Config { get; set; } = null!;

	private RoundService _roundService = null!;
	private PlayerService _playerService = null!;
	private InterfaceService _interfaceService = null!;
	private WeaponService _weaponService = null!;

	public override void Load(bool hotReload)
	{
		Console.WriteLine("Plugin loaded successfully!");

		// Events -->>
		RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
		RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
		RegisterEventHandler<EventRoundStart>(OnRoundStart);
		RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
		RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
		RegisterEventHandler<EventWeaponFire>(OnWeaponFire);
		RegisterEventHandler<EventItemPickup>(OnItemPickup);

		// Listeners -->>
		RegisterListener<Listeners.OnTick>(() => _interfaceService.OnTick());
	}

	public void OnConfigParsed(MainConfig cfg)
	{
		Config = cfg;
		_playerService = new PlayerService(Config, () => _roundService.Phase);
		_roundService = new RoundService(Config, _playerService, AddTimer);
		_interfaceService = new InterfaceService(Config, () => _roundService.TimeLeft, () => _roundService.Phase, () => Utilities.GetPlayers().Where(p => p.IsValid && p.PawnIsAlive).Count(), AddTimer);
		_weaponService = new WeaponService(() => _roundService.Phase);

		_interfaceService.StartHud();
	}
}