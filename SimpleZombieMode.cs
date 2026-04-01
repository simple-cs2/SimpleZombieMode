// SimpleZombieMode.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Localization;
using SimpleZombieMode.Configs;
using SimpleZombieMode.Services;

namespace SimpleZombieMode;

public partial class SimpleZombieMode : BasePlugin, IPluginConfig<MainConfig>
{
	public override string ModuleName => "SimpleZombieMode";
	public override string ModuleVersion => "0.0.3";
	public override string ModuleAuthor => "t.me/kotyarakryt";
    public override string ModuleDescription => "A simple and clean Zombie Mode for CS2 — infection system, respawn lives, last survivor bonus and more. Built with CounterStrikeSharp.";

	public MainConfig Config { get; set; } = null!;

	private RoundService _roundService = null!;
	private PlayerService _playerService = null!;
	private InterfaceService _interfaceService = null!;
	private WeaponService _weaponService = null!;

	public override void Load(bool hotReload)
	{
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
		_playerService = new PlayerService(Config, () => _roundService.Phase, Localizer);
		_roundService = new RoundService(Config, _playerService, Localizer, AddTimer);
		_interfaceService = new InterfaceService(
			Config,
			() => _roundService.TimeLeft,
			() => _roundService.Phase,
			() => Utilities.GetPlayers().Count(p => p.IsValid && p.PawnIsAlive),
			() => _roundService.RoundWinners,
			Localizer,
			AddTimer
		);
		_weaponService = new WeaponService(() => _roundService.Phase);

		_interfaceService.StartHud();
	}

	public SimpleZombieMode(IStringLocalizer localizer)
	{
		Localizer = localizer;
	}
}