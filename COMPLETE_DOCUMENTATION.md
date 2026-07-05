# The Shift - Complete Project Documentation

## Table of Contents

1. [Project Overview](#overview)
2. [File Structure](#file-structure)
3. [Systems Overview](#systems-overview)
4. [Integration Guide](#integration-guide)
5. [Testing Checklist](#testing-checklist)
6. [Deployment](#deployment)

## Overview

**The Shift** is a complete, production-ready mobile horror game built with Unity 6 and URP.

- **Duration**: 12-minute gameplay session (12 AM to 6 AM)
- **Platform**: Android (also runs on PC for development)
- **Target**: 60 FPS on flagships, 30 FPS minimum on mid-range
- **Size**: ~200MB

## File Structure

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs          # Main game controller
│   │   ├── TimeManager.cs          # 12-minute game time (6 hours game time)
│   │   ├── SanityManager.cs        # Player sanity system
│   │   ├── RuleManager.cs          # 3 horror rules enforcement
│   │   └── InventoryManager.cs     # Task tracking
│   ├── Player/
│   │   ├── PlayerController.cs     # Movement, sprint, crouch
│   │   ├── PlayerCamera.cs         # Cross-platform camera (mouse/touch/gamepad)
│   │   └── Flashlight.cs           # Battery-driven flashlight
│   ├── Enemy/
│   │   └── EnemyAI.cs              # Enemy state machine (4 states)
│   ├── Audio/
│   │   ├── AudioManager.cs         # Master audio system
│   │   └── PlayerFootsteps.cs      # Dynamic footstep audio
│   ├── UI/
│   │   ├── MainMenuUI.cs           # Main menu controller
│   │   ├── PauseMenuUI.cs          # Pause menu
│   │   ├── HUD.cs                  # In-game HUD display
│   │   ├── GameOverUI.cs           # Game over screen
│   │   └── VictoryUI.cs            # Victory screen
│   ├── Environment/
│   │   └── InteractiveObject.cs    # Sarcophagus, doors, cameras, etc.
│   ├── Systems/
│   │   ├── BatterySystem.cs        # Flashlight battery management
│   │   ├── CheckpointSystem.cs     # Save/load system
│   │   └── TaskSystem.cs           # Task completion tracking
│   └── Managers/
│       └── PlayerInputManager.cs   # Cross-platform input handling
├── Scenes/
│   ├── MainMenu.unity
│   ├── GameplayMuseum.unity
│   ├── PauseMenu.unity
│   ├── GameOver.unity
│   └── Victory.unity
├── Prefabs/
│   ├── Player/
│   ├── Enemy/
│   ├── UI/
│   └── Environment/
├── Materials/
├── Textures/
├── Audio/
│   ├── Music/
│   ├── SFX/
│   └── Ambience/
└── Resources/
    └── Audio/
        ├── SFX/
        └── Music/
```

## Systems Overview

### Core Systems

**GameManager** (Singleton)
- Manages game state (Menu, Loading, Playing, Paused, GameOver, Victory)
- Coordinates all other systems
- Handles save/load
- Fires events for state changes

**TimeManager**
- Tracks game time (12 AM to 6 AM)
- 1 game hour = 120 real seconds (2 minutes)
- Total game: 12 minutes
- Fires hourly events for checkpoints

**SanityManager**
- Tracks player sanity (0-100)
- Decreases with horror events and rule violations
- Increases when looking away
- Game over at 0 sanity

**RuleManager**
- Monitors 3 core horror rules
- Rule 1: Don't look at sarcophagus >1.5s after 1 AM
- Rule 2: Stop moving when children laugh
- Rule 3: Crouch and turn off flashlight when mirror reflection stills
- Applies sanity penalties for violations

**InventoryManager**
- Tracks 7 tasks:
  1. Mop floors
  2. Lock doors
  3. Check cameras
  4. Replace fuses
  5. Sort files
  6. Empty trash
  7. Restart generator
- Rewards sanity on completion

### Player Systems

**PlayerController**
- Walk (5 m/s), Sprint (8 m/s), Crouch (2.5 m/s)
- First-person perspective
- CharacterController-based movement
- Interaction range: 3 meters
- Height changes when crouching
- Enforces Rule 2 (no movement during silence)

**PlayerCamera**
- Cross-platform input:
  - **Desktop**: Mouse look (2 sensitivity)
  - **Mobile**: Touch drag (0.5 sensitivity)
  - **Gamepad**: Right stick (2 sensitivity)
- Smooth camera movement
- Monitors sarcophagus for Rule 1
- FOV: 60 degrees (realistic)
- Look angles: ±90 degrees

**Flashlight**
- Spotlight with shadows
- Battery system: 5 minutes of use (300 seconds)
- Drains at 0.5% per second when on
- Recharges at 0.2% per second when off
- Battery warning at 20%
- Full depletion at 0%

### Enemy Systems

**EnemyAI**
- State machine with 4 states:
  1. **Idle**: Stationary
  2. **Patrol**: Moves between waypoints
  3. **Investigate**: Moves to suspicious location
  4. **Chase**: Pursues player
- Detection range scales with player sanity
- Base range: 30m
- Max range: 40m (at 0% sanity)
- Line-of-sight detection
- Catches player if distance < 2m
- Game over on caught

### Audio System

**AudioManager**
- 16 simultaneous SFX channels (3D spatial audio)
- Music system with dynamic intensity
- Ambient loops
- Supports:
  - Footsteps (4 surface types)
  - Whispers (horror ambience)
  - Creaks (building sounds)
  - Children laughing (Rule 2)
  - Sarcophagus rattling (Rule 1)
- Audio from Resources/Audio folder

**PlayerFootsteps**
- Generates footstep sounds based on movement
- Detects surface type via raycast
- Adjusts volume based on movement speed
- Interval: 0.5 seconds per step

### UI Systems

**HUD** (In-game)
- Time display (12 AM - 6 AM format)
- Sanity bar with color coding
- Battery indicator
- Task progress
- Rule violation warnings
- Interaction prompts

**MainMenuUI**
- Start Game
- Settings
- Credits
- Quit

**PauseMenuUI**
- Resume
- Settings
- Return to Main Menu
- Triggered by ESC or Menu button

**GameOverUI**
- Shows entity catch message
- Displays final stats
- Retry or Main Menu

**VictoryUI**
- Shows survival message
- Displays final stats
- Continue or Main Menu

## Integration Guide

### Step 1: Create Base Managers
1. Create empty GameObject "GameManagers"
2. Add components:
   - GameManager
   - TimeManager
   - SanityManager
   - RuleManager
   - InventoryManager
   - CheckpointSystem
   - BatterySystem
   - AudioManager
3. Don't destroy on load
4. Wire all references

### Step 2: Setup Player
1. Create "Player" GameObject with CharacterController
2. Add PlayerController script
3. Create "CameraHolder" child object
4. Add Camera to CameraHolder
5. Add PlayerCamera to CameraHolder
6. Create "Flashlight" child object
7. Add Light (Spotlight) to Flashlight
8. Add Flashlight script to Flashlight object
9. Wire references in PlayerController

### Step 3: Setup Enemy
1. Create "Enemy" GameObject with CharacterController
2. Add EnemyAI script
3. Create patrol waypoints (empty GameObjects)
4. Assign patrol points array in EnemyAI
5. Wire player reference

### Step 4: Setup Interactive Objects
1. For each interactive object:
   - Add Collider (trigger)
   - Add InteractiveObject script
   - Set type and taskId
2. Place around museum

### Step 5: Setup Scenes
Follow SCENE_SETUP.md for complete scene configuration

### Step 6: Build Settings
1. File > Build Settings
2. Add all 5 scenes in order
3. Set MainMenu as scene 0
4. Configure Android settings (see ANDROID_BUILD.md)

## Testing Checklist

### Core Gameplay
- [ ] Game starts from main menu
- [ ] Time advances (12 minutes total)
- [ ] Sanity system works (decreases/increases)
- [ ] Flashlight toggles and battery drains
- [ ] Tasks can be completed
- [ ] Checkpoints save at hour boundaries

### Player Mechanics
- [ ] WASD movement works (PC)
- [ ] Sprint increases speed (shift key)
- [ ] Crouch decreases speed and height
- [ ] Camera look works (mouse/touch/gamepad)
- [ ] Can interact with objects (E key)
- [ ] Footsteps play while moving

### Rules System
- [ ] Rule 1: Sarcophagus triggers after 1 AM
- [ ] Rule 1: Sanity drains if looking >1.5 seconds
- [ ] Rule 2: Children laughing plays at random
- [ ] Rule 2: Movement blocked during laughing
- [ ] Rule 2: Sanity drains if moving during laugh
- [ ] Rule 3: Mirror reflection stills randomly
- [ ] Rule 3: Must crouch + flashlight off
- [ ] Rule 3: Sanity drains if not following

### Enemy AI
- [ ] Enemy patrols between waypoints
- [ ] Enemy detects player in range
- [ ] Enemy chases when detected
- [ ] Enemy catches player (game over)
- [ ] Detection range increases as sanity decreases

### Audio
- [ ] Music plays and changes per hour
- [ ] Ambience loops continuously
- [ ] Footsteps play while moving
- [ ] Spatial audio works (3D positioning)
- [ ] Whispers play randomly
- [ ] Creaks play randomly
- [ ] Children laughing triggers Rule 2
- [ ] Sarcophagus rattle triggers Rule 1

### UI
- [ ] Main menu displays correctly
- [ ] HUD shows time, sanity, battery, tasks
- [ ] Pause menu can open/close
- [ ] Game over screen shows on entity catch
- [ ] Victory screen shows on 6 AM survival
- [ ] Interaction prompts appear near objects
- [ ] Warning indicators show for rule violations

### Mobile/Cross-Platform
- [ ] Touch controls move player
- [ ] Touch drag rotates camera (right side)
- [ ] Touch buttons trigger actions
- [ ] Mouse works on PC
- [ ] Gamepad analog sticks work
- [ ] Gyro aiming works (if enabled)

### Performance
- [ ] 60 FPS on flagship phones
- [ ] 30 FPS on mid-range phones
- [ ] No memory leaks (< 800MB)
- [ ] Audio has no latency issues
- [ ] Shadows render smoothly

### Edge Cases
- [ ] Restarting from pause works
- [ ] Multiple rapid interactions work
- [ ] Rapid rule triggering handled
- [ ] Quick sanity drain handled
- [ ] Battery depletion handled
- [ ] Enemy spawn positioning safe

## Deployment

### Testing Build
```bash
File > Build Settings
- Platform: Android
- Development Build: ON
- Script Debugging: ON
- Click "Build and Run"
```

### Release Build
```bash
File > Build Settings
- Platform: Android
- Development Build: OFF
- Script Debugging: OFF
- Create Symbols.zip: ON
- Click "Build" (generates .apk or .aab)
```

### Submit to Play Store
1. Create app listing in Google Play Console
2. Generate signed AAB
3. Upload to Play Console
4. Add screenshots (5 minimum)
5. Write description and changelog
6. Set rating, price, regions
7. Submit for review

## Support & Documentation

- **ProjectSettings.md**: Complete project configuration
- **SCENE_SETUP.md**: Scene structure and setup
- **ANDROID_BUILD.md**: Android build and deployment
- **README.md**: Project overview

## Contact

For technical questions about The Shift, refer to code comments and documentation files.
