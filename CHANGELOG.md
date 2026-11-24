# Changelog

All notable changes are documented here.

## [Unreleased]

### Added
- XML documentation for core effect types and key sample classes.
- Structured API contract for AI agents.
- Full Russian manual and Quick Start PDF.
- Correctly spelled `LifecycleType` alias while retaining `LifecicleType`.

### Changed
- Runtime effects now implement `IDisposable` and clean up their per-instance `EffectDefinition` clone.
- Sample EffectsController initializes both Instant and continuous effects before OnApply.
- Sample stack policy enforces the configured cap by removing oldest matching effect instances.
- Sample update loop iterates over a snapshot and safely handles effects removed by callbacks.
- Effect UI sample validates references, handles missing icons/components, and unsubscribes when disabled.

### Fixed
- Instant effects no longer run OnApply with a null Owner in the sample controller.
- Runtime cloned ScriptableObjects are released on removal.
- ChangeScaleEffect restores the owner's original local scale, not an assumed Vector3.one.
- UI does not leave stale bindings after view cleanup.

## [0.1.0] - 2026-11-23

### Added
- Initial release of LOGIYGames Effects System.
