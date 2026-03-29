# 🧟 SZM — SimpleZombieMode

[![GitHub release](https://img.shields.io/github/v/release/simple-cs2/SimpleZombieMode)](https://github.com/simple-cs2/SimpleZombieMode/releases)
[![GitHub stars](https://img.shields.io/github/stars/simple-cs2/SimpleZombieMode)](https://github.com/simple-cs2/SimpleZombieMode/stargazers)
[![GitHub issues](https://img.shields.io/github/issues/simple-cs2/SimpleZombieMode)](https://github.com/simple-cs2/SimpleZombieMode/issues)
[![License](https://img.shields.io/github/license/simple-cs2/SimpleZombieMode)](LICENSE)
[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-1.0.363+-blue)](https://github.com/roflmuffin/CounterStrikeSharp)

> A simple and clean Zombie Mode for CS2 — humans vs zombies, infection system, respawn lives, last survivor bonus and more.

---

## 🎮 Gameplay

- **CT = Humans** — survive the round or kill all zombies
- **TT = Zombies** — infect all humans before the time runs out
- Each zombie has a limited number of lives — after running out they wait as spectator
- Humans have infinite ammo
- Last survivor receives a speed and health bonus
- Zombies regenerate HP on kill

---

## 📦 Dependencies

- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) `v1.0.363+`

---

## ⚙️ Installation

1. Download the latest release from [Releases](https://github.com/simple-cs2/SimpleZombieMode/releases)
2. Extract `SimpleZombieMode` folder to:
```
csgo/addons/counterstrikesharp/plugins/
```
3. Configure `SimpleZombieMode.json` (see Configuration)
4. Restart your server

---

## 🔧 Configuration

`csgo/addons/counterstrikesharp/configs/plugins/SimpleZombieMode/SimpleZombieMode.json`
```json
{
  "TimerStartInfection": 15,
  "TimerRound": 900,
  "TimerRestartGame": 3,
  "ZombieHealth": 999,
  "ZombieSpeed": 1.3,
  "ZombieLives": 3,
  "ZombieRespawnDelay": 0.5,
  "ZombieHealOnKill": 50,
  "HumanHealth": 100,
  "HumanSpeed": 1.0,
  "SurvivorHealth": 500,
  "SurvivorSpeed": 1.25,
  "MinPlayers": 4
}
```

| Field | Description |
|---|---|
| `TimerStartInfection` | Countdown before first zombie is chosen (seconds) |
| `TimerRound` | Round duration (seconds) |
| `TimerRestartGame` | Delay before game restart after round ends (seconds) |
| `ZombieHealth` | Zombie HP |
| `ZombieSpeed` | Zombie speed multiplier (`1.0` = default) |
| `ZombieLives` | Number of respawn lives per zombie |
| `ZombieRespawnDelay` | Delay before zombie respawn (seconds) |
| `ZombieHealOnKill` | HP restored to zombie on kill |
| `HumanHealth` | Human HP at round start |
| `HumanSpeed` | Human speed multiplier |
| `SurvivorHealth` | HP bonus for last survivor |
| `SurvivorSpeed` | Speed bonus for last survivor |
| `MinPlayers` | Minimum players required to start |

---

## 👤 Author

Made with ❤️ by [kotyarakryt](https://t.me/kotyarakryt)