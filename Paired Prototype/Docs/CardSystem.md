## Card & Turn System - Concise Overview

### High-level Flow
- Deck is built once at run start from `DeckBootstrap.startingCards` into `DeckService.Deck` (runtime `CardInstance`s, persists across scenes).
- At scene start, `CardHandSpawner` shows an initial hand (3 by default). Each End Turn draws a fresh random hand (3) using `DeckService.PickRandomFromDeck` and `CardHandSpawner.SpawnSpecific`.
- Player selects a card in hand (yellow outline). If any effect requires a target, an enemy must be selected.
- Press Play Card → effects run in order → logs are printed → the played card’s UI is destroyed.
- Press End Turn → enemies act (skipped if `testMode`) → `TurnManager.NextTurn()` resets blocks → hand UI clears → new hand spawns and is logged.

### Key Runtime Objects
- `DeckService` (singleton, DontDestroyOnLoad)
  - `Deck: List<CardInstance>`
  - Build: `BuildStartingDeck(CardData[])`
  - Add/Upgrade/Curse helpers; `PickRandomFromDeck(int)` for offers/hands.

- `TurnManager` (singleton, DontDestroyOnLoad)
  - `StartTurn()` resets `IsPlayLocked=false` and clears all `Health.currentBlock`.
  - `NextTurn()` = End + Start.

- `DeckBootstrap` (scene component)
  - On `Start()`: `StartTurn()`; if deck empty → `BuildStartingDeck(startingCards)`.

- `CardHandSpawner`
  - `container` (UI) + `cardPrefab` (`Assets/Bucky/Prefabs/Cards/Card.prefab`).
  - `Start()` shows initial hand; `SpawnSpecific(List<CardInstance>)` renders a provided list.

- `HandSelectionManager` (scene singleton)
  - Tracks selected `CardSelectable`; exposes `Selected` and `Clear()`.

- `ButtonActions`
  - `testMode` (bool) and `cardsPerHand` (int, default 3).
  - `OnPlayCardClick()`: plays selected `CardInstance`, logs post-state, removes card UI.
  - `OnEndTurnClick()`: enemies attack (unless `testMode`), `NextTurn()`, clear spawned cards, draw/log new hand, spawn it.

- `PlayButtonInteractableBinder` (UI helper)
  - For Play Card button: enables only if a card is selected, and if any effect `RequiresTarget`, an enemy is selected.
  - For End Turn button: set `alwaysEnable=true` to keep it clickable.

### Card Data Model
- `CardData` (SO): name + `baseEffects: CardEffect[]` + optional description.
- `CardInstance`: runtime copy; `Play(player, target)` executes effects.
- `CardEffect` (abstract): `description`, `effectType`, `RequiresTarget` (virtual, default false), `Execute(player, target)`.
  - Implemented effects: `DealDamageEffect` (single target, +power, min 0), `DealDamageAllEnemiesEffect` (+power, min 0), `GainBlockEffect`, `HealEffect`, `GainPowerEffect`, `LosePowerEffect` (power can be negative), `SelfDamageEffect`, `EndTurnLockEffect`.

### UI Prefab
- `Card.prefab`: `CardDisplay` renders name and effect lines (green positive, red negative). `CardSelectable` adds `Button` and `Outline` for click/selection.

### Scene Wiring (minimal)
1) `Systems` GameObject: add `DeckService`, `TurnManager`, `DeckBootstrap` (assign `startingCards`).
2) UI: `HandContainer` under Canvas; add `CardHandSpawner` (anywhere, assign `container` + `cardPrefab`).
3) Selection: `HandSelectionManager` in scene; enemies use `EnemySelectable` + `SelectManager` exists.
4) Buttons:
   - Play Card: OnClick → `ButtonActions.OnPlayCardClick()`. Add `PlayButtonInteractableBinder` (default settings).
   - End Turn: OnClick → `ButtonActions.OnEndTurnClick()`. Add `PlayButtonInteractableBinder` with `alwaysEnable=true`.

### Persistence Across Scenes
- `DeckService` and `TurnManager` are DontDestroyOnLoad; `DeckBootstrap` only builds when the deck is empty, so reloading scenes keeps the deck and upgrades/curses.

### Testing Tips
- Set `ButtonActions.testMode=true` to skip enemy attacks and focus on effect correctness.
- Watch Console: [Play] lines show chosen card/effects; [Post] lines show player/enemy HP/Block/Power; [Hand] lines show the new hand at turn start.

### Next Steps (optional)
- Draw/discard piles with real consumption, shuffling, and hand size limits.
- Energy/cost system on `CardData`.
- Effect previews on card hover (compute with current power and clamp rules).


