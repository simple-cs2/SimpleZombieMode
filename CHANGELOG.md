# Changelog

All notable changes to this project will be documented in this file.

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