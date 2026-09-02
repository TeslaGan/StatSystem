# StatSystem

Простая generic-система статов для C# без зависимости от Unity.

`StatContainer<TStat, TComponent>` хранит runtime-набор статов сущности. Стат появляется автоматически, когда в контейнер добавляется первый `IStatSource` для этого `TStat`, и исчезает после удаления последнего source.

Core не валидирует, какие значения `TStat` и `TComponent` допустимы в конкретной игре. Если проекту нужна такая проверка, она делается на внешней границе (например, через Registry).

## Математика

Все источники имеют один из трёх типов:

```text
Add
Percent
Multiply
```

`Add` формирует абсолютную базу стата:

```text
Base = Sum(Add)
```

`Percent` и `Multiply` группируются по `TComponent`:

```text
Component = (1 + Sum(Percent)) * Product(Multiply)
```

Итог:

```text
Value = Base * Product(Components)
```

Поэтому при `Base == 0` любые процентные и multiplicative-компоненты всё равно дают `0`.

Компонент — это математическая группа влияния, а не gameplay-система. Один и тот же компонент может влиять на разные статы независимо.

## Базовый пример

Определим ключи статов и компонентов обычными enum:

```csharp
public enum CharacterStat
{
    Attack,
    Agility,
    MoveSpeed
}

public enum CharacterStatComponent
{
    Equipment,
    Curse,
    Burger
}
```

Создадим контейнер:

```csharp
using var stats =
    new StatContainer<CharacterStat, CharacterStatComponent>();
```

Статы отдельно регистрировать не нужно. Чтобы добавить `Attack = 10`, достаточно добавить source:

```csharp
var baseAttack = new ConstantStatSource(
    "Character",
    CharacterStat.Attack,
    StatModifierKind.Add,
    10f);

stats.AddSource(baseAttack);
```

Теперь:

```csharp
stats.GetValue(CharacterStat.Attack); // 10
```

Если стат отсутствует:

```csharp
stats.GetValue(CharacterStat.Agility);       // 0
stats.TryGetValue(CharacterStat.Agility, _); // false
```

После появления первого source:

```csharp
var baseAgility = new ConstantStatSource(
    "Character",
    CharacterStat.Agility,
    StatModifierKind.Add,
    10f);

stats.AddSource(baseAgility);

stats.GetValue(CharacterStat.Agility);       // 10
stats.TryGetValue(CharacterStat.Agility, _); // true
```

## Компоненты

Например меч даёт `Attack +5`, мастерство усиливает Equipment на `20%`, а проклятое кольцо даёт отдельный `Curse -30%`:

```csharp
stats.AddSource(new ConstantStatSource(
    "Longsword",
    CharacterStat.Attack,
    StatModifierKind.Add,
    5f));

stats.AddSource(new ConstantStatSource(
    "Sword Mastery",
    CharacterStat.Attack,
    StatModifierKind.Percent,
    0.2f,
    CharacterStatComponent.Equipment));

stats.AddSource(new ConstantStatSource(
    "Cursed Ring",
    CharacterStat.Attack,
    StatModifierKind.Percent,
    -0.3f,
    CharacterStatComponent.Curse));
```

Расчёт:

```text
Base      = 10 + 5 = 15
Equipment = 1 + 0.2 = 1.2
Curse     = 1 - 0.3 = 0.7

Attack = 15 * 1.2 * 0.7 = 12.6
```

## Один компонент влияет на несколько статов

`Burger` может усиливать атаку и одновременно снижать ловкость:

```csharp
stats.AddSource(new ConstantStatSource(
    "Hamburger",
    CharacterStat.Attack,
    StatModifierKind.Multiply,
    3f,
    CharacterStatComponent.Burger));

stats.AddSource(new ConstantStatSource(
    "Burger Sauce",
    CharacterStat.Attack,
    StatModifierKind.Percent,
    0.5f,
    CharacterStatComponent.Burger));

stats.AddSource(new ConstantStatSource(
    "Hamburger",
    CharacterStat.Agility,
    StatModifierKind.Multiply,
    0.8f,
    CharacterStatComponent.Burger));

stats.AddSource(new ConstantStatSource(
    "Burger Grease",
    CharacterStat.Agility,
    StatModifierKind.Percent,
    -0.1f,
    CharacterStatComponent.Burger));
```

Для атаки:

```text
Burger = (1 + 0.5) * 3 = 4.5
```

Для ловкости:

```text
Burger = (1 - 0.1) * 0.8 = 0.72
```

Это один `CharacterStatComponent.Burger`, но разные records для `Attack` и `Agility`.

## Динамические sources

`Value` может вычисляться динамически. Если источник потенциально изменил значение, он вызывает `Invalidate()` из базового `StatSource`.

Контейнер в текущей реализации инвалидирует кэш всех статов, но пересчитывает их лениво только при следующем чтении.

## Зависимость одного стата от другого

Core не содержит отдельного dependency graph. Это можно выразить обычным source:

```csharp
public override float Value =>
    _stats.GetValue(CharacterStat.Agility) * 0.1f;
```

Например `MoveSpeed = 2 + Agility * 0.1`.

Если зависимости образуют цикл, контейнер выбросит `InvalidOperationException` вместо ухода в бесконечную рекурсию.

## Просмотр влияний

`TryGetBreakdown` возвращает snapshot расчёта со всеми sources, base и компонентами:

```csharp
if(stats.TryGetBreakdown(CharacterStat.Attack, out var breakdown))
    StatBreakdownPrinter.Print(breakdown);
```

Пример вывода:

```text
Attack: 56.7

Base
  Character                    +10
  Longsword                     +5
  Total                         15

[Burger] x4.5
  Burger Sauce                 +50%
  Hamburger                      x3

[Curse] x0.7
  Cursed Ring                  -30%

[Equipment] x1.2
  Sword Mastery                +20%

Result: 56.7
```

Полный пример находится в папке `Example`.
