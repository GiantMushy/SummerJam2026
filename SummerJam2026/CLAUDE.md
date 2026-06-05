# SummerJam2026 — Codebase Guide

## Architecture

This is a top-down 2D car game built in Unity. `transform.up` is the forward direction. Physics run on `Rigidbody2D`.

### Three-Tier Manager Hierarchy

Scripts are split across three layers:

```
Totaly Not Interfaces/   ← Base classes (abstract-ish MonoBehaviours)
Player/                  ← Player-specific overrides
AI/                      ← AI-specific overrides
```

Each layer mirrors the same set of manager types:

| Base                      | Player                      | AI                      |
|---------------------------|-----------------------------|-------------------------|
| `CharacterManager`        | `PlayerCharacterManager`    | `AiCharacterManager`    |
| `LocomotionManager`       | `PlayerLocomotionManager`   | `AiLocomotionManager`   |
| `StatsManager`            | `PlayerStatsManager`        | `AiStatsManager`        |
| `CombatManager`           | `PlayerCombatManager`       | `AiCombatManager`       |
| `AnimatorManager`         | `PlayerAnimatorManager`     | `AiAnimatorManager`     |
| `EquipmentManager`        | `PlayerEquipmentManager`    | `AiEquipmentManager`    |
| `InventoryManager`        | `PlayerInventoryManager`    | `AiInventoryManager`    |
| `InteractionManager`      | `PlayerInteractionManager`  | `AiInteractionManager`  |
| `CharacterEffectsManager` | `PlayerEffectsManager`      | `AiCharacterEffectsManager` |
| `UiManager`               | `PlayerUiManager`           | `AiUiManager`           |
| `CharacterSoundEffectManager` | (shared)               | (shared)                |

### Component Composition

A character is a single GameObject with all its Manager components attached. `CharacterManager` (or its subclass) is the root — it caches references to all sibling managers via `GetComponent<>` in `Awake()`. Subclasses call `base.Awake()` then cache their own typed references.

### Stats Pattern

Stats have three fields each: a `[SerializeField] float defaultX` (Inspector-tweakable), a private runtime `float x`, and a set of `GetX()` / `IncreaseX()` / `DecreaseX()` methods. `SetInitialStats()` is called in `Start()` to copy defaults into runtime values.

### Global Singletons

- `GameManager` — scene-level state (score, game time, player reference).
- `InputManager` — `DontDestroyOnLoad`, reads Unity's new Input System and exposes `horizontalInput` / `verticalInput`. Player managers read from `InputManager.instance`.
- `CameraManager` — camera behaviour.

### Interfaces

`IDamageable` lives in `Assets/Scripts/Interfaces/`.

---

## Rules

1. **Ask before assuming.** When anything is unclear — design intent, scope of a change, expected behaviour — ask rather than guess.

2. **Follow the existing architecture.** Do not introduce new architectural patterns. New managers, systems, or structural changes require explicit discussion first. Code should slot into the patterns already in place.

3. **Keep functions small and focused.** Break logic into clearly named helper methods. Avoid long functions — if a method is getting complex, split it up.
