// SimpleZombieMode.Events.cs
using CounterStrikeSharp.API.Core;

namespace SimpleZombieMode;

public partial class SimpleZombieMode
{
    // Event handlers -->>
    private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        _roundService.OnPlayerConnect(@event.Userid);
        return HookResult.Continue;
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        _roundService.OnPlayerDisconnect(@event.Userid);
        return HookResult.Continue;
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        _roundService.StartRound();
        return HookResult.Continue;
    }

    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        _roundService.StopGame();
        return HookResult.Continue;
    }

    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        _roundService.OnPlayerDeath(@event.Userid, @event.Attacker);
        return HookResult.Continue;
    }

    private HookResult OnWeaponFire(EventWeaponFire @event, GameEventInfo info)
    {
        _weaponService.OnWeaponFire(@event.Userid);
        return HookResult.Continue;
    }
    
    private HookResult OnItemPickup(EventItemPickup @event, GameEventInfo info)
    {
        _playerService.OnItemPickup(@event.Userid, @event.Item);
        return HookResult.Continue;
    }
}