// src/services/WeaponService.cs
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleZombieMode.Services;

public class WeaponService
{
    private readonly Func<RoundPhase> _getRoundPhase;

    public WeaponService(Func<RoundPhase> getRoundPhase)
    {
        _getRoundPhase = getRoundPhase;
    }

    public void OnWeaponFire(CCSPlayerController? player)
    {
        if(player is null || !player.IsValid) return;

        if(player.Team is not CsTeam.CounterTerrorist || _getRoundPhase() is not RoundPhase.Active) return;

        CBasePlayerWeapon? weapon = player.PlayerPawn.Value?.WeaponServices?.ActiveWeapon?.Value;

        if(weapon is null || weapon.VData is null) return;

        weapon.Clip1 = weapon.VData.MaxClip1;
    }
}