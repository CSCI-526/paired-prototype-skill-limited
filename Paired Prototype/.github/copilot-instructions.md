# Paired Prototype - Unity Card Game Project

## Project Overview
Unity 2022.3.14f1 card-based combat game with turn-based mechanics. Key systems: deck building, card effects, enemy spawning/leveling, and UI selection patterns.

## Architecture & Team Organization

### Folder Structure by Developer
- **`Assets/Bucky/`** - Card system core (deck service, hand spawning, card effects, reward flow)
- **`Assets/Chace/`** - Scene management, enemy wave system, level progression
- **`Assets/Rachel/`** - Combat mechanics, health/damage, selection systems, UI actions

### Core Singleton Pattern
Essential singletons persist across scenes via `DontDestroyOnLoad`:
- `DeckService` - Manages runtime deck (`List<CardInstance>`), upgrades, curses
- `TurnManager` - Handles turn flow, block resets, play locking
- `SelectManager` / `HandSelectionManager` - Track enemy/card selection states

## Card System Architecture

### Data Flow
1. **Deck Building**: `DeckBootstrap.startingCards` → `DeckService.BuildStartingDeck()` → persistent `CardInstance` list
2. **Hand Display**: `CardHandSpawner.SpawnSpecific()` renders 3 random cards from deck
3. **Selection**: Cards get `CardSelectable` + yellow `Outline`, enemies get `EnemySelectable` + yellow highlight
4. **Play**: `ButtonActions.OnPlayCardClick()` executes effects, destroys card UI
5. **Turn End**: Clear hand UI, draw new random hand, reset blocks

### Key Components
- **CardData (SO)**: Blueprint with `baseEffects[]` and metadata
- **CardInstance**: Runtime copy with dynamic effect list, supports upgrades/curses
- **CardEffect**: Abstract base with `Execute(Health player, Health target)`, `RequiresTarget` flag
- **Card.prefab**: `CardDisplay` (renders effects with green/red colors) + `CardSelectable` (outline selection)

### Scene Wiring Requirements
```
Systems GameObject:
  - DeckService, TurnManager, DeckBootstrap (assign startingCards[])

UI Setup:
  - HandContainer (UI parent) → CardHandSpawner (assign container + cardPrefab)
  - HandSelectionManager (scene singleton)
  - SelectManager (for enemy targeting)

Button Wiring:
  - Play Card: ButtonActions.OnPlayCardClick() + PlayButtonInteractableBinder
  - End Turn: ButtonActions.OnEndTurnClick() + PlayButtonInteractableBinder(alwaysEnable=true)
```

## Combat & Enemy Systems

### Health Component Pattern
All combatants use `Health` with:
- `currentHealth`, `maxHealth`, `currentBlock` (temporary shield), `power` (damage modifier)
- `player` flag determines death behavior (game over vs destroy GameObject)
- Block absorbed before health damage, resets each turn

### Enemy Spawning (Chace's System)
`EnemyWaveManager` (misnamed file `EnemyLeveling.cs`):
- Loads prefabs from Resources folder or Inspector array
- Applies progressive scaling: `hpMult`, `atkMult`, `currentCount` based on random upgrades
- Modifies spawned instances' Health and EnemyAttack components

### Selection Visual Patterns
- **Cards**: Yellow `Outline` component on GameObject
- **Enemies**: `SpriteRenderer.color` changed to `highlightColor` (yellow)
- Both use singleton managers to enforce single selection

## Development Patterns

### Effect Creation Workflow
1. Create ScriptableObject class extending `CardEffect`
2. Implement `Execute(Health player, Health target)`
3. Set `effectType` (Positive/Negative/Neutral) for color coding
4. Override `RequiresTarget` if targeting needed
5. Use editor tool: `Cards/Create Demo Cards` for batch creation

### Debugging & Testing
- Set `ButtonActions.testMode = true` to skip enemy attacks
- Console logging pattern: `[Play]`, `[Post]`, `[Hand]` prefixes for game state
- Effects show green (positive) or red (negative) in card descriptions

### Cross-Scene Persistence
- `DeckService.Deck` and `TurnManager` state survive scene loads
- `DeckBootstrap` only rebuilds deck if empty (allows scene reloading)
- Player progression stored in `PlayerPrefs` for run statistics

## Common Integration Points
- Target validation: Check `SelectManager.Instance.Current` for enemy selection
- Card validation: Check `HandSelectionManager.Instance.Selected` for card selection  
- Effect requirements: Use `CardEffect.RequiresTarget` to determine if enemy must be selected
- UI state binding: `PlayButtonInteractableBinder` handles button enable/disable logic based on selections

## File Naming Conventions
- Effects: `[EffectName]Effect.cs` in `Assets/Bucky/Scripts/Cards/Effects/`
- Runtime systems: `Assets/Bucky/Scripts/Runtime/` 
- Scene-specific: Organized by developer folder (`Chace/`, `Rachel/`, `Bucky/`)