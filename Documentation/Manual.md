# LOGIYGames Effects System — полное руководство

## 1. Назначение и границы пакета

`LOGIYGames Effects System` предоставляет базовые типы для конфигурации и реализации игровых эффектов: статусы, баффы, дебаффы, мгновенные воздействия и эффекты с длительностью. Пакет содержит `EffectDefinition`, `EffectFactory` и `RuntimeEffect`.

Управление коллекцией активных эффектов находится в необязательном примере `Samples~/Examples/EffectsController.cs`. Это пример реализации, а не обязательный runtime-компонент ядра. В своём проекте можно использовать этот контроллер или реализовать собственный.

Core namespace: `LOGIYGames.EffectSystem`. Версия Unity из `package.json`: `2022.3`. Assembly: `LOGIYGames.Effects`.

| Тип | Назначение |
|---|---|
| `EffectDefinition` | ScriptableObject с авторскими настройками эффекта |
| `EffectFactory` | Абстрактная фабрика, создающая новый runtime-экземпляр |
| `RuntimeEffect` | Базовый класс runtime-эффекта, его hooks и lifetime |
| `EffectsController` (sample) | Пример применения, стаковки, обновления и удаления эффектов |
| `DamageEffectDefinition`, `ScalingEffectDefinition` (samples) | Примеры специализированных definition assets |
| `InstantDamageEffect`, `DamageOverTimeEffect`, `ChangeScaleEffect` (samples) | Примеры эффектов разных типов жизненного цикла |

Для агентов и инструментов предусмотрен структурированный контракт [API.json](API.json): типы, поля, сигнатуры, lifecycle, правила стаковки и примеры.

## 2. Установка и настройка

1. Установите пакет в Unity-проект.
2. Убедитесь, что проект использует Unity 2022.3 или совместимую версию.
3. Для примеров импортируйте sample `Examples` через Package Manager.
4. Используйте `LOGIYGames.EffectSystem` для базовых типов и `LOGIYGames.EffectSystem.Examples` для большинства sample-классов.

Core assembly definition минимальна и не объявляет внешних ссылок. Sample assembly ссылается на дополнительные проектные зависимости (включая таймеры, R3 и UI/TMP), которые должны быть подключены для компиляции всех примеров.

## 3. Архитектура и жизненный цикл

Последовательность применения эффекта в sample-контроллере:

1. `EffectFactory.Create()` создаёт новый runtime object. Конструктор `RuntimeEffect` клонирует исходный `EffectDefinition` через `Object.Instantiate`.
2. `EffectsController.AddEffect()` вызывает `Initialize(owner)` для любого типа эффекта, в том числе `Instant`.
3. Для `Countdown` и `Passive` эффект добавляется в активный список. Для `Instant` список не используется.
4. Контроллер вызывает `OnApply()`.
5. Каждый кадр активный continuous-эффект получает `OnUpdate(Time.deltaTime)`.
6. При `IsFinished == true` или явном `RemoveEffect` контроллер вызывает `Dispose()`.
7. `Dispose()` вызывает `OnRemove()` и уничтожает runtime-клон `Data`.

`RuntimeEffect` сам по себе не управляет обновлением, стеком, VFX или SFX. Он предоставляет контракт. Контроллер определяет расписание и политику применения.

### Категории жизненного цикла

| `EffectLifeType` | Поведение |
|---|---|
| `Instant` | Применяется один раз и сразу освобождается; не хранится в активном списке sample-контроллера |
| `Countdown` | Хранится в списке, обновляется, затем удаляется при `IsFinished` |
| `Passive` | Хранится, пока не удалён вручную или не помечен завершённым |

## 4. Создание EffectDefinition

`EffectDefinition` — ScriptableObject, который хранит настройки и presentation assets эффекта.

| Поле | Описание |
|---|---|
| `Value` | Runtime-значение для конкретного экземпляра. Изменения должны касаться клона `RuntimeEffect.Data`, а не исходного asset |
| `MaxStackSize` | Максимальное число активных экземпляров одного concrete effect type в sample-контроллере; минимум 1 |
| `Levels` | Максимальный уровень эффекта; базовый конструктор ограничивает начальный `Level` этим значением |
| `VFX` | Необязательный prefab визуального эффекта. Ядро не создаёт его автоматически |
| `SFX` | Необязательный AudioClip. Ядро не проигрывает его автоматически |
| `Icon` | Необязательная иконка для UI |

Чтобы добавить настройки, создайте наследника:

```csharp
using LOGIYGames.EffectSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Poison")]
public sealed class PoisonEffectDefinition : EffectDefinition
{
    [Min(0f)] public float DamagePerTick = 2f;
    [Min(0.01f)] public float TickInterval = 1f;
}
```

`MaxStackSize` и `Levels` имеют атрибуты `[Min(1)]` и нормализуются в Editor через `OnValidate`. Значения, созданные кодом, также нужно валидировать в конструкторах и фабриках.

## 5. Реализация RuntimeEffect

Каждый эффект должен реализовать `LifecicleType` (оригинальное имя API сохранено), конструктор и `OnApply`. При необходимости переопределите `OnUpdate` и `OnRemove`.

```csharp
using LOGIYGames.EffectSystem;
using System;
using UnityEngine;

[Serializable]
public sealed class ShieldEffect : RuntimeEffect
{
    private readonly float shieldAmount;

    public override EffectLifeType LifecicleType => EffectLifeType.Passive;

    public ShieldEffect(ShieldEffectDefinition definition) : base(definition)
    {
        shieldAmount = definition.Amount;
    }

    public override void OnApply()
    {
        ThrowIfDisposed();
        if (!IsInitialized || Owner == null)
            throw new InvalidOperationException("Initialize must be called before OnApply.");

        Owner.GetComponent<Shield>()?.AddShield(shieldAmount);
    }

    public override void OnRemove()
    {
        if (Owner != null)
            Owner.GetComponent<Shield>()?.RemoveShield(shieldAmount);
    }
}
```

`Owner` защищён и заполняется через `Initialize`. Не обращайтесь к нему в конструкторе или до Initialize. `OnApply` и `OnRemove` должны быть парными по игровому состоянию: если эффект меняет скорость, масштаб, броню, подписки или timers, удаление должно отменить эти изменения.

Базовый класс клонирует definition. `Data` — публичное поле для совместимости с Unity `SerializeReference`; его не следует заменять или присваивать исходный asset. Допускается менять поля клона (например `Data.Value`) для отображения текущего состояния. Клон уничтожается при `Dispose`.

Не переиспользуйте один RuntimeEffect для нескольких целей или повторных применений. Создавайте новый экземпляр для каждой активации.

## 6. EffectFactory

`EffectFactory` — сериализуемый абстрактный класс с одним методом:

```csharp
public abstract RuntimeEffect Create();
```

Фабрика должна возвращать новый объект при каждом вызове. Sample использует `[SerializeReference] EffectFactory`, чтобы выбрать конкретную фабрику в Inspector. Фабрики хранят ссылку на definition и дополнительные настройки длительности или интервала.

```csharp
[Serializable]
public sealed class ShieldEffectFactory : EffectFactory
{
    public ShieldEffectDefinition definition;
    public override RuntimeEffect Create() => new ShieldEffect(definition);
}
```

Если factory может быть не настроена, валидируйте definition до создания эффекта и выдавайте диагностическое сообщение.

## 7. Применение через sample EffectsController

`EffectsController` — MonoBehaviour из `Samples~/Examples`. Он хранит continuous эффекты в `[SerializeReference] List<RuntimeEffect>`, предоставляет `IReadOnlyList<RuntimeEffect> Effects` и событие `OnContinuousEffectsChanged`.

```csharp
using LOGIYGames.EffectSystem.Examples;
using UnityEngine;

public sealed class ApplyShieldButton : MonoBehaviour
{
    [SerializeField] private EffectsController controller;
    [SerializeReference] private ShieldEffectFactory factory;

    public void Apply()
    {
        if (controller == null || factory == null) return;
        controller.AddEffect(factory.Create());
    }
}
```

Для мгновенного эффекта `AddEffect` сначала вызывает `Initialize`, затем `OnApply`, а после завершения освобождает экземпляр. Для `Countdown` и `Passive` эффект остаётся в списке.

### Политика стаковки sample-контроллера

- Сравнение выполняется по `effect.GetType()`, то есть по конкретному классу runtime-эффекта, а не по asset.
- Если `IsStackable == false`, эффективный лимит равен 1: новый экземпляр заменяет старейший того же типа.
- Если `IsStackable == true`, лимит берётся из `Data.MaxStackSize`.
- Если лимит уже достигнут, старейший matching effect удаляется до добавления нового.
- Другие типы эффектов не конкурируют за один stack limit.

Это лишь пример политики. Для игры может быть правильнее сравнивать по ID эффекта, owner, source caster, tag, category или уникальному asset identifier.

Контроллер обновляет snapshot активных эффектов, чтобы callback, меняющий список во время update, не ломал итерацию. На завершении или удалении он вызывает `Dispose`.

## 8. Примеры из Samples

### InstantDamageEffect

Тип `Instant`. При `OnApply` ищет `Health` на owner, наносит `BaseDamage` и устанавливает `isFinished = true`. `EffectsController` не держит этот эффект в активном списке и освобождает его после применения.

### DamageOverTimeEffect

Тип `Countdown`. Создаёт `IntervalTimer(duration, interval)`. Каждое событие интервала вызывает `Health.TakeDamage(BaseDamage)`. При остановке таймера эффект становится завершённым. В `OnRemove` таймер освобождается.

### ChangeScaleEffect

Тип `Countdown`. Сохраняет исходный `transform.localScale`, задаёт масштаб `Vector3.one * ScaleFactor` и восстанавливает исходный размер в `OnRemove`. Конструктор проверяет duration. `Timer` запускается в `OnApply`.

### Definition assets

- `DamageEffectDefinition` добавляет `BaseDamage`.
- `ScalingEffectDefinition` добавляет `ScaleFactor`.

### UI sample

`PlayerEffectsUIBootstrap` подписывается на `OnContinuousEffectsChanged`, создаёт prefab для каждого активного эффекта с иконкой и удаляет views при обновлении/выключении компонента. Prefab должен иметь `PlayerEffectView`, а также назначенные `Image` и `TextMeshProUGUI`. `PlayerEffectPresenter` связывает view и runtime effect.

## 9. Удаление и освобождение

При удалении используйте `EffectsController.RemoveEffect(effect)` либо вызовите `effect.Dispose()` в собственной системе. Не ограничивайтесь одним `OnRemove`, поскольку `Dispose` дополнительно уничтожает runtime clone definition.

`Dispose` идемпотентен: повторный вызов не выполняет очистку повторно. Если `OnRemove` выбрасывает исключение, уничтожение `Data` и очистка `Owner` выполняются в `finally`.

Вызывайте `Dispose` когда активный эффект заканчивается, снимается вручную, владелец уничтожается или система эффектов очищается. Отписывайте effect events и останавливайте coroutine/timer в `OnRemove`.

## 10. Runtime Definition и presentation

`VFX`, `SFX`, `Icon` хранятся в `EffectDefinition`, однако core package не предполагает конкретный способ их использования. Игра сама решает, когда создавать визуальный prefab, где его размещать, кто владеет AudioSource и как UI показывает иконку/Value.

`Value` у runtime-клона удобен для отображения обратного отсчёта, величины щита, уровня заряда или числа стаков. Он не является универсальным источником истины: игровая механика должна хранить специализированное состояние в полях конкретного RuntimeEffect.

## 11. Ошибки и диагностика

| Симптом | Что проверить |
|---|---|
| `Owner` равен null в `OnApply` | `Initialize(owner)` должен быть вызван до `OnApply`; sample controller делает это автоматически |
| Мгновенный эффект не работает | Не пропускайте Initialize для Instant; `AddEffect` в обновлённом sample выполняет его до OnApply |
| Значение в asset меняется от активного эффекта | Изменяйте `RuntimeEffect.Data.Value` — это per-instance clone |
| Эффект остаётся в списке после завершения | Проверьте `LifecicleType` и установку `isFinished`/`IsFinished` |
| Эффект не отменяет своё изменение при удалении | Реализуйте симметричную очистку в `OnRemove` и удаляйте через `Dispose` |
| UI view пустой | Назначьте prefab с `PlayerEffectView`, Image и TextMeshProUGUI; проверьте Icon definition |
| Не компилируются Samples | Подключите sample dependencies: timers, R3, TMPro, UI и project GUID assemblies |
| Стак не ограничивается | Sample группирует эффекты по `GetType()` и читает `MaxStackSize` из новой definition |

## 12. Ограничения и совместимость

- Unity package manifest указывает Unity `2022.3`.
- Core namespace — `LOGIYGames.EffectSystem`; sample namespace — преимущественно `LOGIYGames.EffectSystem.Examples`.
- Оригинальное свойство `LifecicleType` сохранено ради совместимости; добавлен корректно написанный alias `LifecycleType`.
- `RuntimeEffect.Data` сохранено публичным полем, поскольку сериализация `[SerializeReference]` в Unity опирается на поля.
- `EffectsController` и примеры не входят в core runtime API.
- Core asmdef и project references не изменялись.
- `.meta` файлы не изменялись.

Изменение runtime поведения требует проверки в целевой Unity версии. Перед выпуском проверьте: создание assets, сериализацию SerializeReference после перезапуска Editor, Instant/Countdown/Passive, стаковку, повторное удаление, уничтожение owner, UI и Player build.
