// src/services/PlayerService.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using SimpleZombieMode.Configs;

namespace SimpleZombieMode.Services;

public class PlayerService
{
    private readonly Dictionary<ulong, int> _playerLives = new();
    private readonly MainConfig _config;
    private readonly Func<RoundPhase> _getRoundPhase;

    public PlayerService(MainConfig config, Func<RoundPhase> getRoundPhase)
    {
        _config = config;
        _getRoundPhase = getRoundPhase;
    }

    // Zombie management -->>
    internal void InfectPlayer(CCSPlayerController player, CCSPlayerController? infectedBy, bool IsInitialize = true)
    {
        if(player is null || !player.IsValid) return;

        if(IsInitialize)
        {
            player.ChangeTeam(CsTeam.Terrorist);
            _playerLives[player.SteamID] = _config.ZombieLives;
            if(infectedBy is null) Server.PrintToChatAll($" {ChatColors.Red}[SZM] {ChatColors.Gold}{player.PlayerName} {ChatColors.Default}is now a {ChatColors.DarkRed}zombie{ChatColors.Default}!");
            else Server.PrintToChatAll($" {ChatColors.Red}[SZM] {ChatColors.Gold}{player.PlayerName} {ChatColors.Default}turning into a {ChatColors.DarkRed}zombie{ChatColors.Default}... infected by {ChatColors.Gold}{infectedBy.PlayerName}{ChatColors.Default}!");
        }

        player.Respawn();

        // modify player health and speed
        CCSPlayerPawn pawn = player.PlayerPawn.Value!;
        pawn.Health = _config.ZombieHealth;
        pawn.VelocityModifier = _config.ZombieSpeed;

        player.RemoveWeapons();
        player.GiveNamedItem("weapon_knife");
    }

    // Lives management -->>
    internal int RemoveLive(ulong steamId)
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