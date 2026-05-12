# Dream Guardians — Product Backlog

**Project:** Dream Guardians — 2D Tower Defense  
**Engine:** Unity 2022 LTS · C# · Universal Render Pipeline (URP)  
**Methodology:** Scrum  
**Document version:** 1.0 — 2026-05-12  

---

## Scrum Roles

| Role | Person | Responsibility |
|------|--------|---------------|
| **Product Owner** | Tomer Levi | Defines what the product is and validates completion |
| **Scrum Master** | Tomer Levi | Ensures the Scrum process is followed; facilitates team communication |
| **Dev Team** | Tomer Levi, Hagi Debby, Alex Politsan | Builds the product (3 members) |

> **Note:** In this project Tomer Levi holds both the Product Owner and Scrum Master roles. In a larger team these would be separate people. As Scrum Master, Tomer is responsible for keeping the backlog current and running sprint ceremonies. As Product Owner, Tomer defines and validates acceptance criteria.

---

## Definition of Done

A user story is **Done** when all of the following are true:

- [ ] All test cases for the story pass when verified by clicking/playing the game in the Unity editor
- [ ] The feature is visible and verifiable without opening any code or inspector windows
- [ ] No new errors appear in the Unity Console as a result of the change
- [ ] The code is committed to the `main` branch on GitHub
- [ ] Another team member has verified the test cases independently

---

## Story Completion Protocol

When a user story is finished, follow these steps — whether you're doing it manually or with AI assistance.

### Step 1 — Verify the Definition of Done

Go through every test case in the story and confirm each one passes in the Unity editor Play mode. Do not mark a story done if any test case fails.

### Step 2 — Update this document

Change the story's `**Status:**` line from `Not started` to `Done ✅ — YYYY-MM-DD`.

Example:
```
**Priority:** P1 · **Sprint:** 1 · **Status:** Done ✅ — 2026-05-20
```

### Step 3 — Update CODEBASE_DOCUMENTATION.md

For the system the story belongs to, update (or add) the relevant section in `CODEBASE_DOCUMENTATION.md`:
- Fill in the "Architecture in one sentence" if it was missing
- Add or confirm the Inspector Fields table
- Add any new design decisions made during implementation (the **why**, not just the what)
- Move any resolved items out of the Known Gaps table
- If the story introduced new gaps or workarounds, add them to Known Gaps

### Step 4 — Commit to GitHub

Commit the working code with a message that references the story ID. Example:
```
git commit -m "US-14: Tower selection UI — buttons wire to TowerPlacement.SelectTower()"
```

### Step 5 — Update the Summary Table

In the Full Backlog Summary at the bottom of this file, change the story's Status cell to `Done ✅`.

---

### Using AI (Claude) to Complete Step 3

When a story is done, open Claude Code and use this prompt:

```
Story US-XX "[story title]" is now complete.
Here is what was built: [brief description of what you implemented and any
design decisions you made — e.g. "used a CanvasGroup to grey out buttons
instead of disabling them, so the click area still exists for hover tooltips"].
Please update CODEBASE_DOCUMENTATION.md for the relevant section:
- confirm or fill in the architecture summary
- update the inspector fields table
- add the design decision I described
- remove it from Known Gaps if it was listed there
```

Claude will read the current documentation and the code, then update only the relevant section — it won't rewrite the whole file.

---

## How This Backlog Was Built

1. **Codebase audit** — All 15 C# scripts were read and analyzed to map every implemented system.
2. **Feature extraction** — Each distinct, player-visible or system-level capability was isolated as a candidate user story.
3. **Story writing** — Each feature was converted to standard Scrum format: statement + test cases + dependencies.
4. **Priority ranking** — Stories were ordered by dependency chain and player-facing value: foundational systems first, UI and polish last.
5. **Sprint grouping** — Stories were grouped into four logical sprints, each delivering a testable vertical slice of the game.

---

## Priority Legend

| Priority | Meaning |
|----------|---------|
| P1 | Must have — game cannot function without it |
| P2 | Core experience — game is playable but incomplete without it |
| P3 | Player-facing polish — game feels unfinished without it |
| P4 | Nice to have — enhances quality and atmosphere |

---

## Sprint Overview

| Sprint | Theme | Goal | Stories |
|--------|-------|------|---------|
| Sprint 1 | Core Systems | Enemies walk a path; a meter tracks dream health; the player has a currency balance | US-01 → US-06 |
| Sprint 2 | Towers, Enemies & Waves | Three towers fire at enemies; waves spawn in sequence; the full game loop is winnable | US-07 → US-13 |
| Sprint 3 | Game Flow & Player UI | Player can navigate from main menu through placement UI, upgrades, and end screens | US-14 → US-18 |
| Sprint 4 | Polish & Advanced Content | Game looks and sounds finished; advanced enemies add variety | US-19 → US-23 |

---
---

# Sprint 1 — Core Systems Foundation

---

## US-01 · Enemy Path System

**Priority:** P1 · **Sprint:** 1 · **Status:** Not started

> As a **player**, I want enemies to follow a defined path through the dreamworld so that the game has a clear threat route I can defend against.

### Test Cases
- [ ] Does a path exist in the scene made of ordered waypoints?
- [ ] Does an enemy move from the first waypoint to the last in sequence?
- [ ] Does the path display as visible lines in the Unity editor (gizmos) so the level designer can verify it?
- [ ] Does the enemy stop moving when it reaches the final waypoint?

### Dependencies
- None (foundational system)

---

## US-02 · Enemy Base Behavior

**Priority:** P1 · **Sprint:** 1 · **Status:** Not started

> As a **player**, I want nightmare enemies to have health, move speed, and a coin reward value so that the core loop of damaging and defeating enemies is possible.

### Test Cases
- [ ] Does an enemy have a visible health bar that decreases when it takes damage?
- [ ] Does the enemy die and disappear when its health reaches zero?
- [ ] Does the enemy award Dream Coins to the player when it dies?
- [ ] Does the enemy deal damage to the Dream Meter if it reaches the end of the path?
- [ ] Can the enemy be slowed by an external effect, reducing its movement speed?
- [ ] Does the slow effect expire after a set duration and return the enemy to normal speed?

### Dependencies
- US-01 (Enemy Path System)
- US-05 (Dream Meter)
- US-06 (Currency System)

---

## US-03 · Enemy Registration & Targeting Registry

**Priority:** P1 · **Sprint:** 1 · **Status:** Not started

> As a **developer**, I want all active enemies to be registered in a central tracker so that towers can query enemy positions efficiently without running physics overlap checks every frame.

### Test Cases
- [ ] Is an enemy added to the tracker automatically when it spawns?
- [ ] Is an enemy removed from the tracker automatically when it dies?
- [ ] Can a tower read the list of active enemies from the tracker at any time?
- [ ] Does the tracker return an empty list when no enemies are alive?

### Dependencies
- US-02 (Enemy Base Behavior)

---

## US-04 · Tower Placement on Grid

**Priority:** P1 · **Sprint:** 1 · **Status:** Not started

> As a **player**, I want to place towers on valid tiles by clicking the map so that I can position my defenses strategically.

### Test Cases
- [ ] Does a ghost/preview of the tower follow the mouse cursor while in placement mode?
- [ ] Does the ghost snap to a grid so towers align cleanly on tiles?
- [ ] Can the player place a tower only on tiles marked as buildable?
- [ ] Is placement blocked on non-buildable tiles (path, walls)?
- [ ] Is placement blocked if the player cannot afford the tower's cost?
- [ ] Does the tower appear permanently on the map after a successful placement?
- [ ] Does pressing Escape cancel placement and remove the ghost?

### Dependencies
- US-06 (Currency System)

---

## US-05 · Dream Meter

**Priority:** P1 · **Sprint:** 1 · **Status:** Not started

> As a **player**, I want a Dream Meter that decreases when enemies breach the defenses so that I have a clear indicator of how close I am to losing.

### Test Cases
- [ ] Is a Dream Meter slider visible on the HUD at the start of a game session?
- [ ] Does the Dream Meter decrease when an enemy reaches the end of the path?
- [ ] Does the amount of decrease match the enemy's dream damage value?
- [ ] Does the game trigger a Game Over state when the Dream Meter reaches zero?
- [ ] Is the Dream Meter value clamped so it never goes below zero or above its maximum?

### Dependencies
- None (foundational UI system)

---

## US-06 · Currency System (Dream Coins)

**Priority:** P1 · **Sprint:** 1 · **Status:** Not started

> As a **player**, I want a Dream Coin balance that I earn by defeating enemies and spend on towers so that tower placement involves meaningful economic decisions.

### Test Cases
- [ ] Does the player start each game session with a set number of Dream Coins?
- [ ] Does the player's balance increase when an enemy is killed?
- [ ] Does spending coins on a tower reduce the balance by the tower's exact cost?
- [ ] Is the player prevented from placing a tower they cannot afford?
- [ ] Is the coin balance always visible on the HUD?

### Dependencies
- None (foundational system)

---
---

# Sprint 2 — Towers, Enemies & Waves

---

## US-07 · Teddy Bear Tower

**Priority:** P1 · **Sprint:** 2 · **Status:** Not started

> As a **player**, I want to place a Teddy Bear Tower that fires rapidly at close-range enemies so that I have an affordable option for defending tight chokepoints.

### Test Cases
- [ ] Does the Teddy Bear Tower cost 75 Dream Coins to place?
- [ ] Does the tower fire projectiles at enemies within its range?
- [ ] Does the tower's head rotate to face its current target?
- [ ] Does each projectile reduce the target enemy's health on hit?
- [ ] Does the tower stop firing when no enemies are in range?
- [ ] Does the tower target the enemy that has progressed furthest along the path?

### Dependencies
- US-02 (Enemy Base Behavior)
- US-03 (Enemy Targeting Registry)
- US-04 (Tower Placement on Grid)
- US-09 (Projectile Behavior)

---

## US-08 · Toy Soldier Tower

**Priority:** P1 · **Sprint:** 2 · **Status:** Not started

> As a **player**, I want to place a Toy Soldier Tower with balanced range and damage so that I have a versatile all-purpose defender.

### Test Cases
- [ ] Does the Toy Soldier Tower cost 100 Dream Coins to place?
- [ ] Does the tower fire at a rate of one projectile per second?
- [ ] Does the tower engage enemies within a range of 4 units?
- [ ] Does each hit deal 15 damage to the target enemy?
- [ ] Does the tower's head rotate to face the current target?
- [ ] Does the tower target the enemy furthest along the path?

### Dependencies
- US-02 (Enemy Base Behavior)
- US-03 (Enemy Targeting Registry)
- US-04 (Tower Placement on Grid)
- US-09 (Projectile Behavior)

---

## US-09 · Projectile Behavior

**Priority:** P1 · **Sprint:** 2 · **Status:** Not started

> As a **player**, I want tower projectiles to home in on their target and deal damage on contact so that attacks feel responsive and intentional.

### Test Cases
- [ ] Does a projectile move toward its assigned target enemy each frame?
- [ ] Does the projectile rotate to face the direction it is traveling?
- [ ] Does the projectile deal its assigned damage to the enemy when it gets close enough?
- [ ] Does the projectile destroy itself after hitting its target?
- [ ] Does the projectile destroy itself if it has not hit anything after 5 seconds?
- [ ] If the target enemy dies before the projectile arrives, does the projectile destroy itself without causing a console error?

### Dependencies
- US-02 (Enemy Base Behavior)

---

## US-10 · Fairy Godmother Tower

**Priority:** P2 · **Sprint:** 2 · **Status:** Not started

> As a **player**, I want to place a Fairy Godmother Tower that slows enemies from long range so that I have a strategic support option to pair with high-damage towers.

### Test Cases
- [ ] Does the Fairy Godmother Tower cost 150 Dream Coins to place?
- [ ] Does the tower engage enemies at a range of 5 units?
- [ ] Does each projectile apply a slow effect to the enemy it hits?
- [ ] Does the slowed enemy move at a reduced speed for a set duration?
- [ ] Does the enemy return to its normal speed after the slow duration expires?
- [ ] Does the tower target the enemy furthest along the path?

### Dependencies
- US-02 (Enemy Base Behavior)
- US-03 (Enemy Targeting Registry)
- US-04 (Tower Placement on Grid)
- US-09 (Projectile Behavior)

---

## US-11 · Shadow Nightmare Enemy

**Priority:** P1 · **Sprint:** 2 · **Status:** Not started

> As a **player**, I want to face Shadow Nightmare enemies that walk the path and threaten my Dream Meter so that there is a concrete threat to defend against.

### Test Cases
- [ ] Does the Shadow Nightmare spawn at the start of the enemy path?
- [ ] Does it move along the path at its defined speed?
- [ ] Does it have 50 health points?
- [ ] Does it deal 10 damage to the Dream Meter if it reaches the end?
- [ ] Does it award 20 Dream Coins on death?
- [ ] Does it play a death visual effect (VFX) when killed (if a VFX prefab is assigned)?

### Dependencies
- US-01 (Enemy Path System)
- US-02 (Enemy Base Behavior)
- US-05 (Dream Meter)
- US-06 (Currency System)

---

## US-12 · Wave Spawning System

**Priority:** P1 · **Sprint:** 2 · **Status:** Not started

> As a **player**, I want enemies to spawn in structured waves so that the game has escalating challenge and a clear sense of progression.

### Test Cases
- [ ] Does the first wave begin automatically after a short delay when gameplay starts?
- [ ] Are enemies from a wave spawned one at a time at set intervals from the spawn point?
- [ ] Does the game detect when all enemies in a wave have been defeated?
- [ ] Does a new wave start only after the previous wave is fully cleared?
- [ ] Does the game trigger a Victory state after all configured waves are defeated?

### Dependencies
- US-02 (Enemy Base Behavior)
- US-03 (Enemy Targeting Registry)
- US-11 (Shadow Nightmare Enemy)

---

## US-13 · Currency HUD Display

**Priority:** P2 · **Sprint:** 2 · **Status:** Not started

> As a **player**, I want to see my current Dream Coin balance on screen at all times so that I can make informed decisions about when to place towers.

### Test Cases
- [ ] Is the Dream Coin balance visible on the HUD during gameplay?
- [ ] Does the displayed value update immediately when coins are earned?
- [ ] Does the displayed value update immediately when coins are spent?
- [ ] Does the display include a label so the player knows what the number represents?

### Dependencies
- US-06 (Currency System)

---
---

# Sprint 3 — Game Flow & Player Interface

---

## US-14 · Tower Selection UI

**Priority:** P2 · **Sprint:** 3 · **Status:** Not started

> As a **player**, I want to select which tower to place from a UI panel so that I can choose between tower types without using the Unity editor.

### Test Cases
- [ ] Is a tower selection panel visible during gameplay?
- [ ] Does each button in the panel display the tower's name and cost?
- [ ] Does clicking a tower button enter placement mode for that tower?
- [ ] Is a tower button visually disabled (greyed out) when the player cannot afford it?
- [ ] Does the panel update when coins are gained, re-enabling buttons the player can now afford?
- [ ] Can only one tower type be in placement mode at a time?

### Dependencies
- US-04 (Tower Placement on Grid)
- US-06 (Currency System)
- US-07 (Teddy Bear Tower)
- US-08 (Toy Soldier Tower)
- US-10 (Fairy Godmother Tower)

---

## US-15 · Main Menu Scene

**Priority:** P2 · **Sprint:** 3 · **Status:** Not started

> As a **player**, I want a main menu when I launch the game so that I have a clear entry point and do not drop directly into gameplay.

### Test Cases
- [ ] Does the game open to a main menu screen on launch?
- [ ] Is a "Play" button visible on the main menu?
- [ ] Does clicking "Play" load the gameplay scene?
- [ ] Is the game's title ("Dream Guardians") visible on the main menu?
- [ ] Is a "Quit" button visible that closes the application?

### Dependencies
- US-12 (Wave Spawning System — gameplay scene must exist to load into)

---

## US-16 · Game Over Screen

**Priority:** P2 · **Sprint:** 3 · **Status:** Not started

> As a **player**, I want a Game Over screen when my Dream Meter is depleted so that I know the game has ended and I can choose what to do next.

### Test Cases
- [ ] Does a Game Over screen appear when the Dream Meter reaches zero?
- [ ] Is a "Game Over" message displayed on screen?
- [ ] Is a "Retry" button visible that restarts the gameplay scene?
- [ ] Is a "Main Menu" button visible that returns to the main menu?
- [ ] Does gameplay pause (time stop) when the Game Over screen is shown?

### Dependencies
- US-05 (Dream Meter)
- US-15 (Main Menu Scene)

---

## US-17 · Victory Screen

**Priority:** P2 · **Sprint:** 3 · **Status:** Not started

> As a **player**, I want a Victory screen after surviving all waves so that my success is clearly acknowledged and I can choose to play again.

### Test Cases
- [ ] Does a Victory screen appear after all waves are defeated?
- [ ] Is a congratulations message visible on screen?
- [ ] Is a "Play Again" button visible that restarts the gameplay scene?
- [ ] Is a "Main Menu" button visible?
- [ ] Does gameplay pause when the Victory screen is shown?

### Dependencies
- US-12 (Wave Spawning System)
- US-15 (Main Menu Scene)

---

## US-18 · Tower Upgrade UI

**Priority:** P3 · **Sprint:** 3 · **Status:** Not started

> As a **player**, I want to upgrade a placed tower by clicking it and paying coins so that I can invest in specific defenses as the waves get harder.

### Test Cases
- [ ] Does clicking a placed tower open an upgrade panel showing the upgrade cost?
- [ ] Does the "Upgrade" button deduct the correct cost in Dream Coins?
- [ ] Is the "Upgrade" button disabled when the player cannot afford it?
- [ ] Does upgrading a tower increase its damage?
- [ ] Does upgrading a tower increase its range?
- [ ] Does the upgrade panel show the tower's current upgrade level?
- [ ] Does closing or clicking away from the panel dismiss it?

### Dependencies
- US-06 (Currency System)
- US-07 (Teddy Bear Tower)
- US-08 (Toy Soldier Tower)
- US-10 (Fairy Godmother Tower)
- US-14 (Tower Selection UI — UI framework must exist)

---
---

# Sprint 4 — Polish & Advanced Content

---

## US-19 · Sound Effects (SFX)

**Priority:** P3 · **Sprint:** 4 · **Status:** Not started

> As a **player**, I want sound effects for tower attacks, enemy deaths, and UI interactions so that the game world feels alive and responsive.

### Test Cases
- [ ] Does a sound play when a tower fires a projectile?
- [ ] Does a sound play when an enemy is killed?
- [ ] Does a sound play when the player places a tower?
- [ ] Does a sound play when the Dream Meter takes damage?
- [ ] Does a sound play when the player clicks a UI button?

### Dependencies
- US-07, US-08, US-10 (All tower types)
- US-11 (Shadow Nightmare Enemy)
- US-05 (Dream Meter)
- US-14 (Tower Selection UI)

---

## US-20 · Background Music

**Priority:** P3 · **Sprint:** 4 · **Status:** Not started

> As a **player**, I want background music during gameplay and on the main menu so that the dreamworld atmosphere is reinforced throughout the experience.

### Test Cases
- [ ] Does music play automatically when the main menu loads?
- [ ] Does gameplay music start when the gameplay scene loads?
- [ ] Does the music loop without an audible gap?
- [ ] Is the music volume at a level that does not overpower sound effects?

### Dependencies
- US-15 (Main Menu Scene)

---

## US-21 · Enemy Death Visual Effect (VFX)

**Priority:** P4 · **Sprint:** 4 · **Status:** Not started

> As a **player**, I want a visual effect to play when a nightmare enemy is destroyed so that kills feel satisfying and impactful.

### Test Cases
- [ ] Does a particle effect appear at the enemy's position when it dies?
- [ ] Does the particle effect disappear on its own after playing?
- [ ] Does the enemy GameObject get destroyed even if a VFX effect is playing?
- [ ] Do multiple enemies dying at the same time each produce their own independent effect?

### Dependencies
- US-11 (Shadow Nightmare Enemy)

---

## US-22 · Second Enemy Type

**Priority:** P3 · **Sprint:** 4 · **Status:** Not started

> As a **player**, I want to face a second, distinct nightmare enemy in later waves so that the game requires adapting strategy beyond the early stages.

### Test Cases
- [ ] Does a second enemy type exist with different stats from the Shadow Nightmare?
- [ ] Is this enemy visually distinguishable from the Shadow Nightmare?
- [ ] Does it follow the same waypoint path as other enemies?
- [ ] Does it interact with the slow effect from the Fairy Godmother Tower?
- [ ] Does it award a different Dream Coin reward than the Shadow Nightmare?
- [ ] Does it appear only in later waves (not wave 1)?

### Dependencies
- US-01 (Enemy Path System)
- US-02 (Enemy Base Behavior)
- US-12 (Wave Spawning System)

---

## US-23 · Wave Difficulty Scaling

**Priority:** P3 · **Sprint:** 4 · **Status:** Not started

> As a **player**, I want each wave to be harder than the last so that the game presents escalating challenge that keeps me engaged throughout.

### Test Cases
- [ ] Does each wave contain more enemies than the previous wave?
- [ ] Do later waves include enemies with higher health or speed than earlier waves?
- [ ] Does at least one wave include the second enemy type (US-22)?
- [ ] Does the spawn interval decrease in later waves, increasing enemy pressure?
- [ ] Is the final wave visibly harder than wave 1?

### Dependencies
- US-11 (Shadow Nightmare Enemy)
- US-12 (Wave Spawning System)
- US-22 (Second Enemy Type)

---
---

## Full Backlog Summary

| ID | Title | Priority | Sprint | Status |
|----|-------|----------|--------|--------|
| US-01 | Enemy Path System | P1 | 1 | Not started |
| US-02 | Enemy Base Behavior | P1 | 1 | Not started |
| US-03 | Enemy Registration & Targeting Registry | P1 | 1 | Not started |
| US-04 | Tower Placement on Grid | P1 | 1 | Not started |
| US-05 | Dream Meter | P1 | 1 | Not started |
| US-06 | Currency System | P1 | 1 | Not started |
| US-07 | Teddy Bear Tower | P1 | 2 | Not started |
| US-08 | Toy Soldier Tower | P1 | 2 | Not started |
| US-09 | Projectile Behavior | P1 | 2 | Not started |
| US-10 | Fairy Godmother Tower | P2 | 2 | Not started |
| US-11 | Shadow Nightmare Enemy | P1 | 2 | Not started |
| US-12 | Wave Spawning System | P1 | 2 | Not started |
| US-13 | Currency HUD Display | P2 | 2 | Not started |
| US-14 | Tower Selection UI | P2 | 3 | Not started |
| US-15 | Main Menu Scene | P2 | 3 | Not started |
| US-16 | Game Over Screen | P2 | 3 | Not started |
| US-17 | Victory Screen | P2 | 3 | Not started |
| US-18 | Tower Upgrade UI | P3 | 3 | Not started |
| US-19 | Sound Effects | P3 | 4 | Not started |
| US-20 | Background Music | P3 | 4 | Not started |
| US-21 | Enemy Death VFX | P4 | 4 | Not started |
| US-22 | Second Enemy Type | P3 | 4 | Not started |
| US-23 | Wave Difficulty Scaling | P3 | 4 | Not started |

---

*Dream Guardians Product Backlog — v1.0 — 2026-05-12*
