// src/services/InterfaceService.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using SimpleZombieMode.Configs;

namespace SimpleZombieMode.Services;

public class InterfaceService
{
    private readonly MainConfig _config;
    private readonly Func<int> _getTimeLeft;
    private readonly Func<RoundPhase> _getRoundPhase;
    private readonly Func<int> _getPlayersCount;
    private readonly Func<string> _getRoundWinner;
    private readonly Func<float, Action, TimerFlags?, CounterStrikeSharp.API.Modules.Timers.Timer> _addTimer;
    private string _currentHudText = string.Empty;
    private List<CCSPlayerController> _activePlayers = new();

    public InterfaceService(MainConfig config, Func<int> getTimeLeft, Func<RoundPhase> getRoundPhase, Func<int> getPlayersCount, Func<string> getRoundWinner, Func<float, Action, TimerFlags?, CounterStrikeSharp.API.Modules.Timers.Timer> addTimer)
    {
        _config = config;
        _getTimeLeft = getTimeLeft;
        _getRoundPhase = getRoundPhase;
        _getPlayersCount = getPlayersCount;
        _getRoundWinner = getRoundWinner;
        _addTimer = addTimer;
    }

    internal void StartHud()
    {
        _addTimer(1.0f, () =>
        {
            string formattedTime = string.Empty;

            if(_getRoundPhase() is not RoundPhase.Idle)
            {
                TimeSpan time = TimeSpan.FromSeconds(_getTimeLeft());
                formattedTime = time.TotalMinutes >= 1 ? $"{(int)time.TotalMinutes}:{time.Seconds:D2}" : $"{time.Seconds}";
            }

            _activePlayers = Utilities.GetPlayers().Where(p => p.IsValid).ToList();

            switch(_getRoundPhase())
            {
                case RoundPhase.Idle:
                    _currentHudText = $"<center>Waiting for players... <b>{_getPlayersCount()}/{_config.MinPlayers}</b></center>";
                    break;

                case RoundPhase.Countdown:
                    _currentHudText = $"<center>Started infection within <b color='red'>{formattedTime}</b></center>";
                    break;

                case RoundPhase.Active:
                    int humans = _activePlayers.Count(p => p.Team is CsTeam.CounterTerrorist);
                    int humansAlive = _activePlayers.Count(p => p.Team is CsTeam.CounterTerrorist && p.PawnIsAlive);
                    int zombies = _activePlayers.Count(p => p.Team is CsTeam.Terrorist);
                    int zombiesAlive = _activePlayers.Count(p => p.Team is CsTeam.Terrorist && p.PawnIsAlive);

                    _currentHudText = $"<center>Time left: <b color='orange'>{formattedTime}</b><br><font color='green'>Humans: <b>{humansAlive}/{humans}</b></font> | <font color='red'>Zombies: <b>{zombiesAlive}/{zombies}</b></font></center>";
                    break;

                case RoundPhase.Ended:
                    _currentHudText = $"<center>{_getRoundWinner()}</center>";
                    break;
            }
        }, TimerFlags.REPEAT);
    }

    internal void OnTick()
    {
        if(string.IsNullOrEmpty(_currentHudText)) return;
        // @TODO: If the player opens the menu, we should not display the HUD.

        foreach(CCSPlayerController player in _activePlayers)
            if(player is not null && player.IsValid)
                player.PrintToCenterHtml(_currentHudText);
    }
}