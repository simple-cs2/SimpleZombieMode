// src/services/PlayerService.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;
using SimpleZombieMode.Configs;

namespace SimpleZombieMode.Services;

public class PlayerService
{
    private Dictionary<ulong, int> _playerLives = new();
    private readonly MainConfig _config;
    private readonly Func<RoundPhase> _getRoundPhase;
    private readonly IStringLocalizer _localizer;

    public PlayerService(MainConfig config, Func<RoundPhase> getRoundPhase, IStringLocalizer localizer)
    {
        _config = config;
        _getRoundPhase = getRoundPhase;
        _localizer = localizer;
    }

    // Zombie management -->>
    internal void InfectPlayer(CCSPlayerController player, CCSPlayerController? infectedBy, bool isInitialize = true)
    {
        if(player is null || !player.IsValid) return;

        if(isInitialize)
        {
            player.ChangeTeam(CsTeam.Terrorist);
            _playerLives[player.SteamID] = _config.ZombieLives;
            if(infectedBy is null) Server.PrintToChatAll(_localizer["szm.zombie.now", _localizer["szm.prefix"], player.PlayerName]);
            else Server.PrintToChatAll(_localizer["szm.zombie.infected_by", _localizer["szm.prefix"], player.PlayerName, infectedBy.PlayerName]);
        }

        player.Respawn();

        // modify player health and speed
        if(player.PlayerPawn.Value is not CCSPlayerPawn pawn)
            return;

        pawn.Health = _config.ZombieHealth;
        pawn.VelocityModifier = _config.ZombieSpeed;

        player.RemoveWeapons();
        player.GiveNamedItem("weapon_knife");
    }

    // Lives management -->>
    internal int RemoveLife(ulong steamId)
    {
        if(_playerLives.TryGetValue(steamId, out int lives)) _playerLives[steamId] = lives - 1;
        else _playerLives[steamId] = 0;

        return _playerLives[steamId];
    }
    
    internal void ResetLives()
    {
        _playerLives.Clear();
    }

    // Events -->>
    internal void OnItemPickup(CCSPlayerController? player, string itemName)
    {
        if(player is null || !player.IsValid) return;

        if(player.Team is CsTeam.Terrorist && _getRoundPhase() is RoundPhase.Active)
        {
            if(itemName is "knife") return;

            Server.NextFrame(() =>
            {
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
            });
        }
    }
}