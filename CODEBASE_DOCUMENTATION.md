# Dream Guardians — Codebase Documentation

Documents design decisions and behavioral contracts for all systems in the project.
This is the **why** file — the **what** is in the code; the **test checklist** is in PRODUCT_BACKLOG.md.

**Engine:** Unity 2022 LTS · C# · URP  
**Last updated:** 2026-05-12  

---

## System Index

| System | Key Script(s) | Section |
|--------|--------------|---------|
| Game State Machine | `GameManager.cs` | §1 |
| Currency Economy | `CurrencyManager.cs` | §2 |
| Wave Spawning | `WaveManager.cs` | §3 |
| Enemy Base Behavior | `Enemy.cs` | §4 |
| Enemy Registry | `EnemyTracker.cs` | §5 |
| Enemy Concrete Type | `NightmareEnemy.cs` | §6 |
| Tower Base & Targeting | `Tower.cs` | §7 |
| Tower Concrete Types | `TeddyBearTower.cs`, `ToySoldierTower.cs`, `FairyGodmotherTower.cs` | §8 |
| Projectile | `Projectile.cs` | §9 |
| Tower Placement | `TowerPlacement.cs` | §10 |
| Waypoint Path | `Waypoints.cs` | §11 |
| UI | `DreamMeter.cs`, `CurrencyDisplay.cs` | §12 |

---

## §1 · Game State Machine

**File:** `Assets/Scripts/Managers/GameManager.cs`

**Architecture in one sentence:** A singleton MonoBehaviour holds a `GameState` enum and routes state transitions to the appropriate managers — nothing outside `GameManager` should call `Time.timeScale` or trigger scene-level changes directly.

### Game States

| State | Meaning | Time.timeScale |
|-------|---------|---------------|
| `MainMenu` | Pre-game, main menu visible | 1 |
| `Playing` | Active wave in progress | 1 |
| `Paused` | Game paused by player | 0 |
| `WaveClear` | Wave ended, between waves | 1 |
| `GameOver` | Dream Meter reached zero | 0 |
| `Victory` | All waves defeated | 0 |

### State Transitions

```
MainMenu
    │ (Play pressed)
    ▼
Playing ──────────────────────────────────────────────┐
    │                                                  │
    │ (Dream Meter = 0)         (all enemies killed,   │
    ▼                            no waves remaining)   │
GameOver                             Victory           │
    │ (Retry)                                          │
    └──────────────────────────────────────────────────┘
    │
    │ (all enemies killed in wave, waves remain)
    ▼
WaveClear
    │ (player ready / timer)
    ▼
Playing (next wave)
```

### Inspector Fields

| Field | Type | Purpose |
|-------|------|---------|
| `WaveManager` | Reference | Calls `StartNextWave()` on wave clear |
| `CurrencyManager` | Reference | Available for state-change resets |
| `DreamMeter` | Reference | Listens for zero-health event |
| `CurrencyDisplay` | Reference | Available for UI updates |

### Public API

| Method | Called by | Effect |
|--------|-----------|--------|
| `ChangeState(GameState)` | Internal + WaveManager | State transition + timeScale |
| `TriggerGameOver()` | DreamMeter | → `GameOver` state |
| `TriggerVictory()` | WaveManager | → `Victory` state |
| `OnWaveCleared()` | WaveManager | → `WaveClear` state |

### Known Gaps

| Gap | Description | Fix |
|-----|-------------|-----|
| Main Menu scene not built | `GameState.MainMenu` exists but no scene or UI exists to enter it | Build Main Menu scene (US-15) |
| Game Over / Victory screens not built | State changes correctly but no UI appears | Build screens (US-16, US-17) |
| Pause input not wired | `GameState.Paused` logic exists but no key binding calls it | Add Escape-key pause handler |

---

## §2 · Currency Economy

**File:** `Assets/Scripts/Managers/CurrencyManager.cs`

**Architecture in one sentence:** A singleton holds the integer coin balance; all reads and writes go through it, and it fires a `UnityEvent<int>` so UI updates without polling.

### Design Decision: Event-Driven UI

`OnCoinsChanged` (UnityEvent<int>) fires on every balance change. `CurrencyDisplay` subscribes in `Start()` and unsubscribes in `OnDestroy()`. This means the display never polls — it only updates when something actually changes. No tight coupling between economy and UI.

### Inspector Fields

| Field | Type | Default | Purpose |
|-------|------|---------|---------|
| `startingCoins` | int | 150 | Balance at game start |

### Public API

| Method | Returns | Behavior |
|--------|---------|---------|
| `TrySpend(int amount)` | bool | Deducts if affordable; returns false and does nothing if not |
| `AddCoins(int amount)` | void | Increases balance; rejects negative values |

### Validation

- `TrySpend` rejects negative amounts (guard clause at top).
- `AddCoins` rejects negative amounts (guard clause at top).
- Both fire `OnCoinsChanged` only if the balance actually changes.

---

## §3 · Wave Spawning System

**File:** `Assets/Scripts/Managers/WaveManager.cs`

**Architecture in one sentence:** `WaveManager` holds a serialized list of `Wave` configs; it spawns enemies via coroutine, counts active enemies, and notifies `GameManager` when all waves are cleared.

### Data Structures

```
WaveManager
├── List<Wave> waves
│     └── Wave
│           ├── string waveName
│           ├── List<EnemySpawnEntry> spawnEntries
│           │     └── EnemySpawnEntry
│           │           ├── GameObject enemyPrefab
│           │           └── int count
│           └── float spawnInterval
└── float firstWaveDelay (default: 3s)
```

### Spawn Flow

```
GameManager.OnWaveCleared()
      │
      ▼
WaveManager.StartNextWave()
      │  Checks: all waves finished? → TriggerVictory
      │  Otherwise: increments CurrentWaveIndex
      │
      ▼
Coroutine: SpawnWave(Wave)
      │  activeEnemies = total enemies in this wave
      │  spawningInProgress = true
      │  For each EnemySpawnEntry:
      │    For each count:
      │      Instantiate(enemyPrefab, spawnPoint)
      │      yield WaitForSeconds(spawnInterval)
      │  spawningInProgress = false
      │
      ▼
Enemies alive in scene...
      │
      ▼ (each enemy dies → Enemy calls WaveManager.OnEnemyRemoved())
WaveManager.OnEnemyRemoved()
      │  activeEnemies--
      │  if (activeEnemies == 0 && !spawningInProgress)
      │      GameManager.OnWaveCleared()
```

### Design Decision: Spawning vs Completion Tracking

`spawningInProgress` and `activeEnemies` are tracked independently. This prevents a false wave-complete trigger during the spawn interval (if the first few enemies die before the last ones have spawned). A wave is only considered complete when spawning has finished AND all enemies are dead.

### Inspector Fields

| Field | Type | Default | Purpose |
|-------|------|---------|---------|
| `waves` | List\<Wave\> | — | Configure via Unity inspector |
| `firstWaveDelay` | float | 3 | Seconds before wave 1 begins |
| `spawnPoint` | Transform | — | World position where enemies appear |

---

## §4 · Enemy Base Behavior

**File:** `Assets/Scripts/Enemies/Enemy.cs`

**Architecture in one sentence:** `Enemy` follows an ordered array of `Transform` waypoints, responds to damage and slow effects, and notifies `DreamMeter`/`WaveManager` on death.

### Waypoint Movement

```
Start()
  → cache Waypoints[] from Waypoints.GetWaypoints()
  → cache DreamMeter and WaveManager refs (avoids per-frame FindObjectOfType)

Update()
  → move toward waypoints[waypointIndex] at moveSpeed
  → if distance < 0.1 units: waypointIndex++
  → if waypointIndex >= waypoints.Length:
      DreamMeter.TakeDamage(dreamDamage)
      Destroy(gameObject)
```

### Design Decision: Cached Scene References

All scene references (`DreamMeter`, `WaveManager`, `Waypoints`) are cached in `Start()` rather than fetched each frame. `FindObjectOfType` is expensive — doing it once at spawn vs once per frame is the correct pattern for a tower defense with potentially many live enemies.

### PathProgress Property

`PathProgress` returns a normalized 0→1 value representing how far along the path the enemy is. Calculated as `waypointIndex / (float)waypoints.Length`. Used by `Tower.cs` for targeting priority (see §7).

### Slow Effect

```
ApplySlow(float multiplier, float duration)
  → if already slowed: ignore (no stacking)
  → Start coroutine:
      originalSpeed = moveSpeed
      moveSpeed *= multiplier
      yield WaitForSeconds(duration)
      moveSpeed = originalSpeed
      slowed = false
```

**Design Decision:** Slow does not stack. A second `ApplySlow` call while the enemy is already slowed is silently ignored. This keeps behavior predictable — a Fairy Godmother can't perma-freeze an enemy by firing repeatedly.

### Protected Inspector Fields

| Field | Type | Default | Purpose |
|-------|------|---------|---------|
| `enemyName` | string | "Nightmare" | Display / debug label |
| `maxHealth` | int | 50 | Starting health |
| `moveSpeed` | float | 2 | Units per second |
| `dreamDamage` | int | 10 | Damage to Dream Meter on breakthrough |
| `coinReward` | int | 20 | Dream Coins awarded on death |
| `healthBar` | UI.Slider | null | Optional; set in inspector for visual health bar |

### Known Gaps

| Gap | Description | Fix |
|-----|-------------|-----|
| Slow stacking not possible | A faster enemy can never be slowed below the slowest applied multiplier | If stacking is desired, track all active slows and apply the lowest multiplier |
| `healthBar` not auto-found | Must be manually assigned in inspector per prefab | Could be found via `GetComponentInChildren<Slider>` in `Start()` |

---

## §5 · Enemy Registry

**File:** `Assets/Scripts/Enemies/EnemyTracker.cs`

**Architecture in one sentence:** A static class maintains a `HashSet<Enemy>` so towers can query all living enemies in O(1) without any scene graph searches or physics overlaps.

### Design Decision: Static Class vs. Singleton MonoBehaviour

`EnemyTracker` is a plain static class (not a MonoBehaviour). It has no update loop, no Unity lifecycle, and no scene dependency. This is intentional — it exists purely as a fast in-memory index. The trade-off: it must be manually cleared on scene load (`EnemyTracker.Clear()`), because static state persists across scene transitions in Unity.

### Public API

| Method | Called by | Effect |
|--------|-----------|--------|
| `Register(Enemy)` | `Enemy.Start()` | Adds to HashSet |
| `Unregister(Enemy)` | `Enemy.OnDestroy()` | Removes from HashSet |
| `Clear()` | Scene transition code | Empties HashSet |
| `ActiveEnemies` | `Tower.Update()` | Read-only IReadOnlyCollection view |

### Known Gaps

| Gap | Description | Fix |
|-----|-------------|-----|
| `Clear()` not called anywhere yet | No scene transition code exists; static state will carry over if the scene is reloaded (Retry button) | Call `EnemyTracker.Clear()` in GameManager before loading a new scene |

---

## §6 · Concrete Enemy — Shadow Nightmare

**File:** `Assets/Scripts/Enemies/NightmareEnemy.cs`

**Architecture in one sentence:** A minimal subclass of `Enemy` that sets its stats in `Start()` and overrides `Die()` to spawn an optional particle effect.

### Stats (set in Start, override base defaults)

| Stat | Value |
|------|-------|
| Name | "Shadow Nightmare" |
| Health | 50 |
| Move Speed | 2 |
| Dream Damage | 10 |
| Coin Reward | 20 |

### Inspector Fields

| Field | Type | Purpose |
|-------|------|---------|
| `deathVFXPrefab` | GameObject | Optional particle effect spawned at death position |

### Template for New Enemy Types

`NightmareEnemy` is the template pattern for all future enemies. To add a new type:
1. Create a new class inheriting `Enemy`
2. Override `Start()` to set stats
3. Override `Die()` if custom death behavior is needed
4. Create a prefab and assign it to `WaveManager.waves[n].spawnEntries`

---

## §7 · Tower Base Class & Targeting

**File:** `Assets/Scripts/Towers/Tower.cs`

**Architecture in one sentence:** An abstract base class handles target acquisition from `EnemyTracker`, rotation of a head sprite, fire-rate timing, and upgrade scaling — subclasses only need to implement `Shoot()`.

### Targeting Algorithm

```
Update() — runs every frame
  1. fireCooldown -= Time.deltaTime
  2. Scan EnemyTracker.ActiveEnemies (O(n)):
       → skip if distance > range
       → track enemy with highest PathProgress (furthest along path)
  3. If target found:
       → rotate rotatingHead toward target
       → if fireCooldown <= 0: Shoot(); fireCooldown = 1/fireRate
  4. If no target in range: currentTarget = null
```

**Design Decision: Target-the-furthest.** Towers always prioritize the enemy closest to breaking through, not the nearest enemy. This is the standard tower defense heuristic — it minimizes Dream Meter damage per shot because it prevents the most advanced threat from reaching the end.

**Design Decision: EnemyTracker over Physics.** Using the `HashSet<Enemy>` registry instead of `Physics2D.OverlapCircle` avoids allocating a results array every frame and avoids the overhead of the physics broadphase for targeting. For a 2D tower defense with dozens of towers firing every second, this is significant.

### `CircleCollider2D` Requirement

`Tower` has `[RequireComponent(typeof(CircleCollider2D))]`. The collider radius is set to `range` in `Start()`. This is purely for Unity Gizmo drawing and potential future range-indicator use — targeting itself uses the distance check, not trigger callbacks.

### Inspector Fields

| Field | Type | Default | Purpose |
|-------|------|---------|---------|
| `towerName` | string | — | Display label |
| `cost` | int | 100 | Dream Coins to place |
| `upgradeCost` | int | 75 | Dream Coins to upgrade |
| `range` | float | 3 | Attack radius in world units |
| `fireRate` | float | 1 | Shots per second |
| `damage` | int | 10 | Damage per projectile hit |
| `rotatingHead` | Transform | — | Child sprite that rotates toward target |

### Upgrade Scaling

| Stat | Per-level multiplier |
|------|---------------------|
| `damage` | ×1.25 (25% per level) |
| `range` | ×1.10 (10% per level) |

`Upgrade()` is a virtual method — subclasses can override to add type-specific upgrade behavior.

### Known Gaps

| Gap | Description | Fix |
|-----|-------------|-----|
| No upgrade UI | `Upgrade()` logic is complete but nothing calls it from a player interaction | Build Tower Upgrade UI (US-18) |
| No max upgrade level | A tower can be upgraded indefinitely | Add `maxUpgradeLevel` field and guard in `Upgrade()` |
| No tower sell mechanic | Misplaced towers cannot be removed | Add `Sell()` method returning partial refund |

---

## §8 · Concrete Tower Types

**Files:** `TeddyBearTower.cs`, `ToySoldierTower.cs`, `FairyGodmotherTower.cs`

All three inherit `Tower`, set their stats in `Start()`, and implement `Shoot()` by instantiating a `Projectile` prefab from a `firePoint` transform.

### Tower Comparison

| Tower | Cost | Range | Fire Rate | Damage | Special |
|-------|------|-------|-----------|--------|---------|
| Teddy Bear | 75 | 2.5 | 1.5/sec | 8 | None |
| Toy Soldier | 100 | 4.0 | 1.0/sec | 15 | None |
| Fairy Godmother | 150 | 5.0 | 0.8/sec | 10 | Slows target |

### Fairy Godmother — Slow Application

```
Shoot()
  → Instantiate(sparkleProjectilePrefab, firePoint)
  → proj.Initialise(currentTarget, damage)
  → proj.ApplySlow(slowMultiplier, slowDuration)
```

`ApplySlow()` marks the projectile. On hit, the projectile calls `target.ApplySlow()` before destroying itself.

### Shared Inspector Fields (all three)

| Field | Type | Purpose |
|-------|------|---------|
| `projectilePrefab` / `sparkleProjectilePrefab` | GameObject | Projectile to spawn on `Shoot()` |
| `firePoint` | Transform | World position to spawn projectile (optional — falls back to `transform.position`) |

### Fairy Godmother Additional Fields

| Field | Type | Default | Purpose |
|-------|------|---------|---------|
| `slowMultiplier` | float [0.1–0.9] | 0.5 | Speed fraction during slow (0.5 = 50% speed) |
| `slowDuration` | float | 2 | Seconds slow lasts |

---

## §9 · Projectile

**File:** `Assets/Scripts/Projectiles/Projectile.cs`

**Architecture in one sentence:** A homing projectile moves toward a cached `Enemy` reference each frame, deals damage and optional slow on proximity contact, and self-destructs on hit or timeout.

### Movement & Hit Detection

```
Update()
  → if target == null or target destroyed: Destroy(self)
  → move toward target.transform.position at speed
  → rotate to face direction of travel
  → if distance to target < 0.15 units:
      target.TakeDamage(damage)
      if hasSlow: target.ApplySlow(slowMultiplier, slowDuration)
      Destroy(self)
```

**Design Decision: Distance-based collision, not colliders.** Using a 0.15-unit proximity threshold instead of a trigger collider avoids the overhead of physics collision callbacks for potentially many simultaneous projectiles. At the scales and speeds used (speed=8, projectile lifespan≤5s), this is visually indistinguishable from a collider-based approach.

**Design Decision: No object pooling.** Projectiles are instantiated per shot and destroyed on hit. For the expected enemy/tower counts in a 2D tower defense, this is lightweight enough. If profiling shows GC pressure from frequent instantiation/destruction, a `ProjectilePool` could be introduced — but premature pooling adds complexity with no current benefit.

### Public API

| Method | Called by | Purpose |
|--------|-----------|---------|
| `Initialise(Enemy, int)` | Tower's `Shoot()` | Sets target and damage after Instantiate |
| `ApplySlow(float, float)` | FairyGodmotherTower | Marks projectile to apply slow on hit |

### Inspector Fields

| Field | Type | Default | Purpose |
|-------|------|---------|---------|
| `speed` | float | 8 | Units per second toward target |
| `maxLifetime` | float | 5 | Self-destructs after this many seconds if no hit |

---

## §10 · Tower Placement

**File:** `Assets/Scripts/Gameplay/TowerPlacement.cs`

**Architecture in one sentence:** Tracks a selected prefab and a ghost instance; on left-click it validates the tile layer and currency, then instantiates the real tower; Escape cancels.

### Placement Flow

```
SelectTower(prefab) called (from UI button — not yet built)
  → isPlacing = true
  → ghostInstance = Instantiate(ghostPrefab)

Update() while isPlacing:
  → UpdateGhostPosition():
      ray = Camera.ScreenToWorldPoint(mousePos)
      hit = Physics2D.Raycast(ray, buildableTileLayer)
      if hit: ghost.position = SnapToGrid(hit.point)
  → Left click:
      TryPlaceTower():
        hit = Physics2D.Raycast(ray, buildableTileLayer)
        if !hit: return  (invalid tile)
        if !CurrencyManager.TrySpend(cost): return  (can't afford)
        Instantiate(selectedPrefab, SnapToGrid(hit.point))
        CancelPlacement()
  → Escape key:
      CancelPlacement()
```

### Grid Snapping

```csharp
Vector3 SnapToGrid(Vector3 pos)
{
    return new Vector3(
        Mathf.Round(pos.x) + 0.5f,
        Mathf.Round(pos.y) + 0.5f,
        pos.z
    );
}
```

The 0.5 offset centers towers within 1-unit grid cells.

### Inspector Fields

| Field | Type | Purpose |
|-------|------|---------|
| `buildableTileLayer` | LayerMask | Only tiles on this layer accept tower placement |
| `ghostPrefab` | GameObject | Semi-transparent preview shown while placing |

### Known Gaps

| Gap | Description | Fix |
|-----|-------------|-----|
| `SelectTower()` has no caller | Method exists but no UI buttons call it | Build Tower Selection UI (US-14) |
| No overlap check | Two towers can be placed on the same cell | Add `Physics2D.OverlapPoint` check before placing |

---

## §11 · Waypoint Path

**File:** `Assets/Scripts/Gameplay/Waypoints.cs`

**Architecture in one sentence:** A MonoBehaviour whose child GameObjects, in scene hierarchy order, define the enemy path — `GetWaypoints()` returns them as an ordered array.

### Design Decision: Children as Waypoints

Using child Transform objects instead of a serialized array means the path is defined visually in the Unity scene hierarchy and editor viewport (the Gizmo lines make it immediately visible). New waypoints are added by creating child objects — no code changes needed.

### Editor Gizmos

- Red lines between consecutive waypoints
- Red spheres at each waypoint position
- Yellow sphere at the final waypoint (destination)

### Known Gaps

| Gap | Description | Fix |
|-----|-------------|-----|
| No minimum validation | `GetWaypoints()` returns whatever children exist; zero children causes enemies to break | Add a guard: if `childCount < 2`, log a warning in the editor |

---

## §12 · UI Systems

### DreamMeter

**File:** `Assets/Scripts/UI/DreamMeter.cs`

Tracks the player's dream health (0–100). Calls `GameManager.TriggerGameOver()` when it reaches zero.

| Field | Type | Default | Purpose |
|-------|------|---------|---------|
| `maxDreamPower` | int | 100 | Starting / max value |
| `dreamSlider` | UI.Slider | — | Visual health bar |
| `dreamValueText` | UI.Text | null | Optional numeric label |

**Public API:**

| Method | Called by | Effect |
|--------|-----------|--------|
| `TakeDamage(int)` | `Enemy.Update()` (on path end) | Reduces dream power; triggers GameOver at 0 |
| `Restore(int)` | (intended: wave clear bonus) | Increases dream power up to max |

---

### CurrencyDisplay

**File:** `Assets/Scripts/UI/CurrencyDisplay.cs`

Listens to `CurrencyManager.OnCoinsChanged` and updates a UI Text label. Subscribes in `Start()`, unsubscribes in `OnDestroy()`.

| Field | Type | Default | Purpose |
|-------|------|---------|---------|
| `coinText` | UI.Text | — | Text component to update |
| `prefix` | string | "Dream Coins: " | Label shown before the number |

---

## Known Gaps — Project-Wide

| Gap | Affects | Priority | Notes |
|-----|---------|----------|-------|
| EnemyTracker not cleared on scene reload | All towers, all waves | P1 | Call `EnemyTracker.Clear()` before reloading scene |
| No tower placement UI | Player cannot place towers without code | P2 | US-14 |
| No main menu scene | Game has no entry point for players | P2 | US-15 |
| No game over / victory screens | Game state changes but nothing renders | P2 | US-16, US-17 |
| No upgrade UI | `Tower.Upgrade()` unreachable by player | P3 | US-18 |
| No audio system | No audio files, no audio scripts | P3 | US-19, US-20 |
| No VFX prefabs | `NightmareEnemy.deathVFXPrefab` always null | P4 | US-21 |
| Single enemy type | Only `NightmareEnemy` exists | P3 | US-22 |
| Hard-coded wave content | Difficulty does not scale between waves | P3 | US-23 |
| Tower overlap not prevented | Two towers can stack on same cell | P2 | `TowerPlacement.TryPlaceTower()` |
| Tower max upgrade level uncapped | Towers can be upgraded indefinitely | P3 | Add `maxUpgradeLevel` to `Tower.cs` |

---

## Architecture Decisions Summary

| Decision | Where | Reason |
|----------|-------|--------|
| EnemyTracker (static HashSet) instead of Physics overlap | `Tower.cs` targeting | Avoids per-frame allocation and broadphase overhead |
| Target furthest-along enemy (highest PathProgress) | `Tower.cs` targeting | Minimizes Dream Meter damage per shot |
| Slow does not stack | `Enemy.ApplySlow()` | Keeps behavior predictable; prevents degenerate freeze from Fairy Godmother spam |
| Scene refs cached in Enemy.Start() | `Enemy.cs` | `FindObjectOfType` is expensive at scale |
| CurrencyDisplay event-driven (UnityEvent) | `CurrencyDisplay.cs` | No polling; display only updates on actual change |
| Projectile distance-check (not collider) | `Projectile.cs` | Avoids physics callback overhead for many simultaneous projectiles |
| Waypoints as child GameObjects | `Waypoints.cs` | Path is visible and editable directly in scene viewport |
| Spawning vs completion tracked separately | `WaveManager.cs` | Prevents false wave-complete during spawn interval |

---

*Dream Guardians Codebase Documentation — v1.0 — 2026-05-12*
