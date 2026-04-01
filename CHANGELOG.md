# Changelog

All notable changes to this project will be documented in this file.

## [0.0.4] - 2026-04-01
### Added
- Localization system with `IStringLocalizer`
- `en.json` and `ru.json` language files
- Round winner display in HUD on `Ended` phase
- Infection countdown messages in chat (5..1)
- Notify zombie when last life remaining
- Notify last survivor in chat with bonus message

### Fixed
- Last life warning now shows before respawn instead of after lives run out

### Changed
- Refactored `StopGame` using key-based localizer
- Optimized player counting in HUD with single-pass foreach
- Cached `GetRoundPhase()` result to avoid multiple calls
- Reused existing players list for last survivor check
- Renamed `Timer` to `_timer` to follow naming conventions
- Renamed `RemoveLive` to `RemoveLife` for correct grammar
- Renamed `IsInitialize` to `isInitialize` to follow C# conventions
- Simplified `RemoveLife` using `Math.Max` and `GetValueOrDefault`
- Replaced `Where().Count()` with `Count()` predicate
- Used pattern matching for `PlayerPawn` null checks
- Release archive now includes `lang/` folder with translations

## [0.0.3] - 2026-03-29
### Removed
- Unused `GetLives` method from `PlayerService`
- Unused `_hudTimer` field from `InterfaceService`
- Unused import in `WeaponService`
- Debug console log from `Load()`

## [0.0.2] - 2026-03-29
### Removed
- Debug command "css_test"
- Temporary test flag (_roundService.test)

### Changed
- Simplified player selection logic (removed bot filtering condition)
- Players list is no longer nullable and is initialized directly

## [0.0.1] - 2026-03-29
### Added
- Initial project setup