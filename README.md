# LOGIYGames Effects System

Базовая система авторских определений и runtime-экземпляров игровых эффектов для Unity 2022.3. Core содержит `EffectDefinition`, `EffectFactory` и `RuntimeEffect`; управление активными эффектами и демонстрационные типы поставляются в optional sample `Samples~/Examples`.

## Quick start

1. Создайте `EffectDefinition` asset или наследника.
2. Реализуйте `RuntimeEffect` subclass: `LifecicleType`, `OnApply`, при необходимости `OnUpdate` и `OnRemove`.
3. Реализуйте `EffectFactory.Create()`, который возвращает новый объект.
4. Вызовите `EffectsController.AddEffect(factory.Create())` или передайте эффект собственной системе.
5. При удалении вызовите `Dispose()`: он вызывает `OnRemove` и уничтожает runtime clone `Data`.

## Documentation

- [API.json](Documentation/API.json) — структурированный API-контракт для ИИ-агентов.
- [Manual.md](Documentation/Manual.md) — полное руководство на русском.
- [QuickStart.pdf](Documentation/QuickStart.pdf) — пример реализации и применения.

## Compatibility

Package version `0.1.0`; Unity `2022.3`; assembly `LOGIYGames.Effects`; namespace `LOGIYGames.EffectSystem`. Core assembly не требует дополнительных references. Optional sample зависит от проектных assemblies, Timer package, R3 и Unity UI/TMP.
