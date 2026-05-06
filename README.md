# Dream Guardians 🌙

**Dream Guardians** is a 2D tower defense game where players protect a sleeping child's dreamworld from waves of nightmare creatures trying to break in.

---

## 🎮 Gameplay Overview

1. **Place Towers** – Deploy dream guardians — Teddy Bears, Toy Soldiers, Fairy Godmothers and more — on the map to defend key positions.
2. **Survive Waves** – Stop nightmares from reaching the child before the Dream Meter empties.
3. **Upgrade & Adapt** – Earn Dream Coins to upgrade towers and unlock new defenders between waves.

---

## 🧩 Project Structure

```
Assets/
├── Scenes/             # Unity scene files
├── Scripts/
│   ├── Managers/       # GameManager, WaveManager, CurrencyManager
│   ├── Towers/         # Tower base class + TeddyBear, ToySoldier, FairyGodmother
│   ├── Enemies/        # Enemy base class + NightmareEnemy
│   ├── Projectiles/    # Projectile behaviour
│   ├── Gameplay/       # TowerPlacement, EnemySpawner, Waypoints / Path
│   └── UI/             # DreamMeter, CurrencyDisplay
├── Prefabs/            # Reusable Unity prefabs
├── Sprites/            # 2D artwork
└── Audio/              # Sound effects & music
```

---

## 🛠 Tech Stack

| Tool | Purpose |
|------|---------|
| Unity 2022 LTS | Game engine |
| C# | Scripting language |
| Universal Render Pipeline (URP) | 2D rendering |

---

## 👥 The Team

| Name | Role |
|------|------|
| **Tomer Levi** | Game Designer & Developer |
| **Hagi Debby** | Programming & Game Development |
| **Alex Politsan** | UI, Audio & Integration |

---

## 🚀 Getting Started

1. Clone the repository.
2. Open the project in **Unity 2022 LTS** (or later).
3. Open `Assets/Scenes/MainMenu.unity` to start.
4. Press **Play** to run the game in the editor.

---

## 📜 License

This project is the intellectual property of the Dream Guardians team. All rights reserved.
