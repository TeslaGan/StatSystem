# Core.StatSystem

Маленькая generic-система статов для C# и Unity-проектов.

## StatContainer

`StatContainer<TStat, TComponent>` — основной runtime-контейнер системы.

Статы заранее регистрировать не нужно. Стат существует, пока в контейнере есть хотя бы один source для его `TStat`.

```csharp
var stats = new StatContainer<CharacterStat, CharacterStatComponent>();
```

Если стат отсутствует:

```csharp
stats.GetValue(CharacterStat.Attack);        // 0
stats.TryGetValue(CharacterStat.Attack, _);  // false
```

## Sources

Любое влияние на стат представлено через `IStatSource<TStat, TComponent>`.

В системе есть три типа модификаторов:

```text
Add
Percent
Multiply
```

`Add` формирует базу:

```text
Base = Sum(Add)
```

`Percent` и `Multiply` группируются по компоненту:

```text
Component = (1 + Sum(Percent)) * Product(Multiply)
```

Финальное значение:

```text
Value = Base * Product(Components)
```

Например:

```text
Character     +10 Attack
Longsword      +5 Attack
Sword Mastery +20% [Equipment]
Cursed Ring   -30% [Curse]

Attack = (10 + 5) * 1.2 * 0.7 = 12.6
```

Простейший constant source можно сделать так:

```csharp
public sealed class ConstantStatSource : StatSource<CharacterStat, CharacterStatComponent>
{
    public override CharacterStat Stat { get; }
    public override CharacterStatComponent Component { get; }
    public override StatModifierKind Kind { get; }
    public override float Value { get; }

    public ConstantStatSource(
        CharacterStat stat,
        StatModifierKind kind,
        float value,
        CharacterStatComponent component = default)
    {
        Stat = stat;
        Kind = kind;
        Value = value;
        Component = component;
    }
}
```

Добавление source автоматически создаёт stat record:

```csharp
stats.AddSource(new ConstantStatSource(
    CharacterStat.Attack,
    StatModifierKind.Add,
    10f));
```

Удаление:

```csharp
stats.RemoveSource(source);
```

Когда удалён последний source этого стата, stat record тоже исчезает.

`Stat`, `Component` и `Kind` source должны оставаться стабильными, пока source зарегистрирован. Меняться может `Value`. Если значение изменилось, source вызывает `Invalidate()`.

## Components

Компонент — математическая группа, а не gameplay-система.

Один компонент может независимо влиять на разные статы. Например `Burger` одновременно усиливает Attack и уменьшает Agility:

```text
Attack:
    Hamburger    x3   [Burger]
    Burger Sauce +50% [Burger]

Burger = (1 + 0.5) * 3 = 4.5
```

```text
Agility:
    Hamburger     x0.8 [Burger]
    Burger Grease -10% [Burger]

Burger = (1 - 0.1) * 0.8 = 0.72
```

## Dynamic sources

`Value` может вычисляться динамически, в том числе через другой stat:

```csharp
public override float Value =>
    _stats.GetValue(CharacterStat.Agility) * 0.1f;
```

Таким source можно выразить, например:

```text
MoveSpeed = 2 + Agility * 0.1
```

При `Invalidated` контейнер помечает все stat records dirty и пересчитывает нужное значение лениво при следующем чтении.

Циклическая зависимость обнаруживается во время evaluation и приводит к `InvalidOperationException` вместо бесконечной рекурсии.

## Breakdown

Для UI, debug и tooltip можно получить snapshot полного расчёта:

```csharp
if(stats.TryGetBreakdown(CharacterStat.Attack, out var breakdown))
{
    // breakdown.BaseValue
    // breakdown.AddSources
    // breakdown.Components
    // breakdown.Value
}
```

Snapshot хранит значения sources на момент расчёта, поэтому динамический source не перечитывается позже при отображении.

Пример форматированного вывода:

```text
Attack: 56.7

Base: 15
  Character                     +10
  Longsword                      +5

Burger: x4.5
  Burger Sauce                  +50%
  Hamburger                       x3

Curse: x0.7
  Cursed Ring                   -30%

Equipment: x1.2
  Sword Mastery                 +20%

Result: 56.7
```

Для `Base` выводится абсолютное базовое значение без `x`, а компоненты выводятся как итоговые множители.

## Example

Полный пример находится в папке `Example`. Он показывает Equipment, Curse, Burger, красивый breakdown и зависимость `MoveSpeed` от `Agility`.
