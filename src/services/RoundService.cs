// src/services/RoundService.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using SimpleZombieMode.Configs;

namespace SimpleZombieMode.Services;

public enum RoundPhase
{
    Idle,
    Countdown,
    Active,
    Ended
}

public enum GameEnd
{
    HumansWin,
    ZombieWin,
    Canceled,
    Draw
}

public class RoundService
{
    private readonly MainConfig _config;
    private readonly PlayerService _playerService;
    private readonly Func<float, Action, TimerFlags?, CounterStrikeSharp.API.Modules.Timers.Timer> _addTimer;
    public int TimeLeft { get; private set; } = 0;
    public RoundPhase Phase { get; private set; } = RoundPhase.Ended;
    public string RoundWinners { get; private set; } = string.Empty;
    private CounterStrikeSharp.API.Modules.Timers.Timer? Timer;
    private bool _lastSurvivorBoosted = false;

    public RoundService(MainConfig config, PlayerService playerService, Func<float, Action, TimerFlags?, CounterStrikeSharp.API.Modules.Timers.Timer> addTimer)
    {
        _config = config;
        _playerService = playerService;
        _addTimer = addTimer;
    }

    // Events -->>
    internal void OnPlayerConnect(CCSPlayerController? player)
    {
        if(player is null || !player.IsValid || Phase is not RoundPhase.Idle) return;

        _addTimer(0.2f, () => player.Respawn(), null);
        List<CCSPlayerController> players = Utilities.GetPlayers().Where(p => p.IsValid).ToList();

        if(players.Count >= _config.MinPlayers) StartRound();
    }
    internal void OnPlayerDisconnect(CCSPlayerController? player)
    {
        if(player is null || !player.IsValid) return;

        List<CCSPlayerController> players = Utilities.GetPlayers().Where(p => p.IsValid && p != player).ToList();

        switch(Phase)
        {
            case RoundPhase.Countdown:
                if(players.Count < _config.MinPlayers)
                    StopGame(GameEnd.Canceled);
                break;

            case RoundPhase.Active:
                int humans = players.Count(p => p.Team == CsTeam.CounterTerrorist);
                int zombies = players.Count(p => p.Team == CsTeam.Terrorist);
                if(humans <= 0 || zombies <= 0)
                    StopGame(GameEnd.Canceled);
                break;
        }
    }

    internal void StartRound()
    {
        _lastSurvivorBoosted = false;
        List<CCSPlayerController> players = Utilities.GetPlayers().Where(p => p.IsValid && p.PawnIsAlive).ToList();

        if(players.Count < _config.MinPlayers)
        {
            Phase = RoundPhase.Idle;
            return;
        }

        foreach(CCSPlayerController player in players)
        {
            player.SwitchTeam(CsTeam.CounterTerrorist);
            _addTimer(0.2f, () =>
            {
                player.Respawn();

                CCSPlayerPawn pawn = player.PlayerPawn.Value!;
                pawn.Health = _config.HumanHealth;
                pawn.VelocityModifier = _config.HumanSpeed;
            }, null);
        }

        Phase = RoundPhase.Countdown;
        StartTimer(_config.TimerStartInfection, () => StartInfection(), (timeLeft) =>
        {
            if(timeLeft <= 5 && timeLeft > 0)
                Server.PrintToChatAll($" {ChatColors.Red}[SZM] {ChatColors.Default}Infection in {ChatColors.Red}{timeLeft}{ChatColors.Default}...");
        });
    }

    internal void OnPlayerDeath(CCSPlayerController? victim, CCSPlayerController? killer)
    {
        if(victim is null || killer is null || !victim.IsValid || !killer.IsValid || Phase is not RoundPhase.Active) return;

        if(victim.Team is CsTeam.Terrorist)
        {
            int lives = _playerService.RemoveLive(victim.SteamID);
            if(lives > 0) _addTimer(_config.ZombieRespawnDelay, () => _playerService.InfectPlayer(victim, null, false), null);
            else if(lives == 1) victim.PrintToChat($" {ChatColors.Red}[SZM] {ChatColors.Default}Last chance! {ChatColors.Red}1 life {ChatColors.Default}remaining!");
        }

        if(victim.Team == CsTeam.CounterTerrorist && killer.Team == CsTeam.Terrorist)
        {
            _addTimer(0.1f, () =>
            {
                if(!victim.IsValid) return;
                _playerService.InfectPlayer(victim, killer?.IsValid == true ? killer : null, true);
            }, null);

            CCSPlayerPawn? killerPawn = killer.PlayerPawn.Value;
            if(killerPawn is not null)
                killerPawn.Health = Math.Min(killerPawn.Health + _config.ZombieHealOnKill, _config.ZombieHealth);
        }

        _addTimer(1.0f, () =>
        {
            List<CCSPlayerController> players = Utilities.GetPlayers().Where(p => p.IsValid && p.PawnIsAlive).ToList();

            int humans = 0;
            int zombies = 0;

            foreach(CCSPlayerController player in players)
                if(player.Team is CsTeam.CounterTerrorist) humans++;
                else if(player.Team is CsTeam.Terrorist) zombies++;
            
            if(humans is 0 && zombies is not 0) StopGame(GameEnd.ZombieWin);
            else if(humans is not 0 && zombies is 0) StopGame(GameEnd.HumansWin);
            else if(humans is 1 && zombies is not 0 && !_lastSurvivorBoosted)
            {
                _lastSurvivorBoosted = true;

                CCSPlayerController? human = Utilities.GetPlayers().Where(p => p.IsValid && p.PawnIsAlive).FirstOrDefault(p => p.Team == CsTeam.CounterTerrorist);
                CCSPlayerPawn? pawn = human?.PlayerPawn.Value;

                if(human is null || pawn is null) return;

                pawn.Health = _config.SurvivorHealth;
                pawn.VelocityModifier = _config.SurvivorSpeed;

                Server.PrintToChatAll($" {ChatColors.Red}[SZM] {ChatColors.Gold}{human.PlayerName} {ChatColors.Default}is the {ChatColors.Lime}last survivor{ChatColors.Default}! Received a bonus!");
            }
        }, null);
    }

    // Functions -->>
    private void StartTimer(int seconds, Action callback, Action<int>? OnTick = null)
    {
        Timer?.Kill();
        TimeLeft = seconds;

        Timer = _addTimer(1.0f, () =>
        {
            TimeLeft--;
            OnTick?.Invoke(TimeLeft);
            if(TimeLeft <= 0)
            {
                StopTimer();
                callback();
            }
        }, TimerFlags.REPEAT);
    }

    private void StartInfection()
    {
        List<CCSPlayerController> players = Utilities.GetPlayers().Where(p => p.IsValid && p.PawnIsAlive).ToList();

        if(players.Count < _config.MinPlayers)
        {
            Phase = RoundPhase.Idle;
            return;
        }

        var random = new Random();
        CCSPlayerController zombie = players[random.Next(players.Count)];

        _playerService.InfectPlayer(zombie, null, true);

        Phase = RoundPhase.Active;
        StartTimer(_config.TimerRound, () => StopGame(GameEnd.Draw));
        Server.ExecuteCommand("mp_ignore_round_win_conditions 1");
    }

    private void StopTimer()
    {
        Timer?.Kill();
        TimeLeft = 0;
    }

    internal void StopGame(GameEnd whoWins = GameEnd.Draw)
    {
        Phase = RoundPhase.Ended;

        StopTimer();
        _playerService.ResetLives();
        Server.ExecuteCommand($"mp_restartgame {_config.TimerRestartGame}");

        (RoundWinners, string chatColor) = whoWins switch
        {
            GameEnd.HumansWin   => ("Humans win!", $"{ChatColors.Lime}"),
            GameEnd.ZombieWin   => ("Zombies win!", $"{ChatColors.DarkRed}"),
            GameEnd.Canceled    => ("The game was cancelled due to a lack of players.", $"{ChatColors.Default}"),
            _                   => ("Round ended!", $"{ChatColors.Default}")
        };

        Server.PrintToChatAll($" {ChatColors.Red}[SZM] {chatColor}{RoundWinners}");
    }
}