# 💥 Shotgun Recoil

<div align="center">

![Godot Engine](https://img.shields.io/badge/Godot-4.7_Mono-478cbf?style=for-the-badge&logo=godotengine&logoColor=white)
![C#](https://img.shields.io/badge/C%23-.NET_8.0-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
[![Latest Release](https://img.shields.io/github/v/release/ZENOX01/shotgun-recoil?style=for-the-badge&color=blueviolet)](https://github.com/ZENOX01/shotgun-recoil/releases/latest)
![License](https://img.shields.io/badge/License-MIT-orange?style=for-the-badge)

**A fast-paced 2D retro physics platformer where your only propulsion is the violent kickback of a 12-gauge shotgun.**

*Blast to fly. Aim to survive.*

<p align="center">
  <a href="https://github.com/ZENOX01/shotgun-recoil/releases/latest">
    <img src="https://img.shields.io/badge/⬇️_Download_Playable_Game-v1.0.0_(Win_&_Linux)-success?style=for-the-badge" alt="Download Game" />
  </a>
</p>

</div>

---

## 📖 Overview

**Shotgun Recoil** turns traditional platformer movement on its head: you have **no walk keys**. The only way to traverse deadly spike pits, launch across chasms, shatter barriers, and reach the exit portal is by aiming your shotgun and firing. Every shot unleashes physics recoil that launches your cloaked hero backwards through the air!

---

## 🕹️ Controls

| Action | Input | Description |
| :--- | :--- | :--- |
| **Aim** | `Mouse Cursor` | Rotates the shotgun and points your line of fire |
| **Shoot / Propel** | `Left Mouse Button` / `Space` | Fires buckshot pellets and propels you in the opposite direction |
| **Reload** | *Touch the Floor* | Automatically restores your shotgun shells upon landing |

---

## ✨ Features

- **💥 Kinetic Recoil Locomotion**: Pure physics-driven movement. Fire down to launch into the air, fire forward to dash backward, or chain rapid blasts for precision aerial maneuvers.
- **🎨 Handcrafted Retro Pixel Art**: Featuring a charismatic yellow-headed, blue-cloaked protagonist with smooth 4-frame breathing idle and dynamic wind-fluttering flight animations.
- **🧱 Destructible Environments**: Blast through breakable walls and obstacles to carve paths through hazardous chambers.
- **⚡ Interactive Hazards & Tools**:
  - **Spikes & Moving Hazards**: Lethal obstacles demanding aerial dexterity.
  - **Jump Pads**: Launchpads providing massive upward kinetic boosts.
  - **Ammo Refill Crystals**: Mid-air pickups replenishing shotgun ammo during complex flight sequences.
- **🎯 5 Distinct Campaign Levels**: Progressively challenging stages testing timing, recoil control, and precision shooting.
- **🖥️ Interactive Main Menu**:
  - Full campaign level selector modal.
  - "How to Play" tutorial modal.
  - Interactive background diorama with live shotgun firing and physics kickback.
- **💥 Visual & Game Feel Juice**: Custom squash-and-stretch on weapon discharge, landing impact cushions, dynamic airborne leaning, screen shake, muzzle flash, and particle impacts.

---

## 📁 Project Structure

```plaintext
shotgun-recoil/
├── Assets/                # Pixel art sprites, tilesets, and fonts
│   ├── idle.png           # 4-frame character breathing idle animation
│   ├── jump.png           # 4-frame dynamic airborne cape animation
│   ├── shotgun.png        # Shotgun weapon sprite
│   ├── tilemap.png        # Environmental platform & scenery tiles
│   └── fonts/             # Retro arcade typography (PressStart2P, Silkscreen)
├── scenes/                # Godot scene files
│   ├── MainMenu.tscn      # Interactive title screen & level select
│   ├── player.tscn        # Player character, shotgun pivot & camera
│   ├── level_1.tscn       # Level 1 stage
│   ├── level_2.tscn       # Level 2 stage
│   ├── level_3.tscn       # Level 3 stage
│   ├── level_4.tscn       # Level 4 stage
│   ├── level_5.tscn       # Level 5 stage
│   ├── Goal.tscn          # Stage completion flag & level transition
│   ├── AmmoHUD.tscn       # Dynamic on-screen shell counter
│   ├── BreakableWall.tscn # Destructible barricades
│   ├── JumpPad.tscn       # Kinetic bounce pads
│   └── spikes.tscn        # Static & moving hazards
├── scripts/               # C# logic & physics scripts
│   ├── Player.cs          # Recoil impulse, air physics, state transitions
│   ├── Bullets.cs         # Pellet trajectory, spread & impact FX
│   ├── BreakableWall.cs   # Wall damage & shatter physics
│   ├── JumpPad.cs         # Launchpad bounce velocity
│   ├── MainMenu.cs        # Menu UI controller & level routing
│   ├── MenuBackground.cs  # Interactive menu diorama physics
│   ├── AmmoHUD.cs         # Ammo UI rendering
│   └── Goal.cs            # Level completion handling
├── project.godot          # Godot project settings & configuration
└── Shotgun recoil.csproj  # C# .NET project configuration
```

---

## 🚀 Getting Started

### Prerequisites

- [Godot Engine 4.x (.NET / Mono version)](https://godotengine.org/download)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)

### Installation & Running

1. **Clone the repository**:
   ```bash
   git clone https://github.com/ZENOX01/shotgun-recoil.git
   cd shotgun-recoil
   ```

2. **Build the C# solution**:
   ```bash
   dotnet build
   ```

3. **Open and Run in Godot**:
   - Open the Godot Engine (Mono version).
   - Click **Import** and select the `project.godot` file in this directory.
   - Press **F5** (or click the **Play** button in the top right) to launch the game!

---

## 🛠️ Built With

- **Engine**: [Godot Engine 4](https://godotengine.org/)
- **Language**: C# (.NET 8)
- **Art & Animation**: Hand-crafted Pixel Art (Krita / Aseprite)
- **Fonts**: Press Start 2P & Silkscreen

---

## 👤 Author

Developed with ❤️ by **Zeno** ([@ZENOX01](https://github.com/ZENOX01)).
