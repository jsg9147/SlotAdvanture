# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Slot Adventure** is a probability-based roguelike RPG built in Unity with C#. All combat is determined by a slot machine-inspired random system (1000-point scale where 700 = 70% success). The game features 3-character party management, turn-based combat, equipment systems, and gambling mini-games.

- Platform: PC (Steam)
- Company: MSGStudio
- Language: Korean primary, with English/Chinese localization

## Build Commands

This is a Unity project. Use Unity Editor for building:
- Open project in Unity Hub
- Build via File > Build Settings > Build
- Main scene: `00_Scenes/MAIN`

No command-line build scripts are configured. For automated builds, use Unity's batch mode:
```bash
Unity.exe -batchmode -projectPath "path/to/project" -buildTarget StandaloneWindows64 -executeMethod BuildScript.Build -quit
```

## Architecture

### Manager Hierarchy (Singleton Pattern)

All managers use singleton pattern with `DontDestroyOnLoad`:

**Static Managers** (`01_Scripts/StaticManager/`):
- `GameManager` - Central game state, unit data, stage tracking
- `PrefabManager` - Unit prefab pooling and instantiation
- `EffectManager` - Visual effects and particle pooling
- `LoadingManager` - Async scene loading
- `SlotMachineManager` - Probability calculations
- `LocalizationManager` - Multi-language CSV support

**Gameplay Managers** (scene-specific):
- `BattleManager` - Turn order, action queue, probability resolution
- `ItemManager` / `EquipmentManager` - Inventory and stat calculations
- `SkillManager` - Skill learning from skill books
- `SanctuaryManager` - Revival buff assignment
- `StoreManager` / `TreasureManager` / `GambleManager`

### Core Data Flow

```
Unit.cs (runtime instance)
├── UnitData.cs (persistent stats)
├── StatData.cs (HP, AD, AP, DEF, MR, SPD, ACC)
├── Equipment.cs (equipped items)
├── List<SkillObject> (learned skills with PP)
└── List<StatusEffect> (active buffs/debuffs)
```

### Scene Flow

```
MAIN → LOBBY (Select Units) → MAP (Navigate Dungeon)
  → BATTLE (Fight) → TREASURE (Rewards)
  → STORE/SANCTUARY/GAMBLE (Optional) → Next Stage
```

Build scenes (9 total): MAIN, LOBBY, MAP, BATTLE, STORE, SANCTUARY, TREASURE, GAMBLE, LOADING

### Data-Driven Design

All game content is defined via ScriptableObjects in `04_ScriptableObject/`:
- `Enemy/` - Monster definitions by stage (Tutorial, Fire, Forest, Snow, Cave, Boss, Final)
- `Item/` - Equipment and consumables with stat effects
- `Skill/` - Player skills by type (Melee, Range, Buff, Debuff, MultiShot)
- `Monster Skill/` - Enemy ability pools
- `SantuaryBuff/` - Revival buff definitions

### Localization System

CSV-based localization in `LocalizationCSV/` with columns: `Key | English | Korean | Chinese`
- Categories: UI, Items, Skills, Unit Names, Monster Names
- Runtime lookup via `LocalizationManager`

## Third-Party Dependencies

- **DOTween** - Animation/tweening for combat and UI
- **Master Audio** (DarkTonic) - Audio mixer, playlists (Battle, Gamble, Room, StageBGM, Ending)
- **Steamworks.NET** - Steam achievements (12 total) and cloud saves
- **Easy Save 3** - Data serialization
- **TextMesh Pro** - UI text rendering

## Key Patterns

**Probability System**: All actions use 1000-point scale
```csharp
// Hit occurs if: random(0-1000) - (ACC * 10) <= SlotResult
```

**Turn Resolution**: Units sorted by SPD stat via TurnManager, sequential execution with DOTween animations

**Status Effects**: Track duration in turns, apply stat modifications, handle revival flags separately

## Extending the Codebase

**New Skill**: Create SkillObject in `04_ScriptableObject/Skill/`, add SkillParticle prefab, update LocalizationCSV

**New Enemy**: Create MonsterData in `04_ScriptableObject/Enemy/`, link to prefab in `03_Prefabs/Monster/`, add to MapGenerateData enemy pool

**New Item**: Create ItemData in `04_ScriptableObject/Item/`, add icon to `02_Sprites/ItemIcon/`, update LocalizationCSV
