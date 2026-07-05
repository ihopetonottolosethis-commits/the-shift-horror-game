# The Shift - Mobile Horror Game

**The Shift** is a premium first-person horror game developed for Android using Unity 6 and Universal Render Pipeline (URP).

## Project Overview

- **Engine**: Unity 6 (Latest Stable)
- **Graphics**: Universal Render Pipeline (URP)
- **Target Platform**: Android (Mobile Optimized)
- **Game Duration**: 12 minutes (Work shift from 12 AM to 6 AM)
- **Genre**: Psychological Horror / First-Person

## Game Features

### Gameplay
- First-person perspective with walk, sprint, and crouch mechanics
- 12-minute game session structured as a 6-hour night shift (each game hour = 2 real minutes)
- Sanity meter that decreases with horror events
- Flashlight with battery system
- Interactive tasks: mop floors, lock doors, check cameras, replace fuses, sort files, empty trash, restart generator

### Environment
- Single-floor museum setting
- Realistic URP lighting with volumetric fog
- Dynamic flashlight shadows
- Multiple rooms: Main Hallway, Security Office, Storage Rooms, Archive Room, Janitor Closet
- Central sarcophagus display in main hallway

### Horror System (Rules-Based)

**Rule 1 - The Watcher**: If sarcophagus rattles after 1 AM, player must not look at it for more than 1.5 seconds or sanity drains rapidly.

**Rule 2 - Silence**: If children laughing is heard, player must stop moving immediately or face consequences.

**Rule 3 - Mirror**: If player's reflection stays still while they move, they must turn off flashlight and crouch for 5 seconds.

### Enemy AI
- One supernatural entity with intelligent behavior
- States: Idle, Patrol, Investigate, Chase
- Becomes more aggressive as player sanity decreases
- Adaptive difficulty based on player performance

### Audio
- 3D spatial audio
- Dynamic footsteps based on surface and movement
- Ambient whispers and building creaks
- Random supernatural sounds
- Dynamic music system

### Graphics & Performance
- Target: 60 FPS on flagship Android phones
- Minimum: 30 FPS on mid-range Android devices
- Volumetric fog and dynamic shadows
- Post-processing: Film grain, bloom, ambient occlusion
- Optimized for modern Android hardware (Snapdragon 888+, Exynos 2100+)

## Quick Start

1. Clone this repository
2. Open with Unity 6.0 or later
3. Navigate to `Assets/Scenes/MainMenu.unity`
4. Press Play to test
5. See `ProjectSettings.md` for build configuration

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/                 # Core game systems
│   ├── Player/               # Player controller and mechanics
│   ├── Enemy/                # Enemy AI and behavior
│   ├── UI/                   # UI controllers and managers
│   ├── Audio/                # Audio system and managers
│   ├── Environment/          # Interactive objects and room logic
│   ├── Systems/              # Time, rules, sanity, inventory
│   └── Managers/             # Game, input, and state managers
├── Scenes/
│   ├── MainMenu.unity
│   ├── GameplayMuseum.unity
│   ├── PauseMenu.unity
│   ├── GameOver.unity
│   └── Victory.unity
├── Prefabs/
│   ├── Player/
│   ├── Enemy/
│   ├── Environment/
│   └── UI/
├── Materials/
├── Textures/
├── Audio/
│   ├── Music/
│   ├── SFX/
│   └── Ambience/
├── Settings/
│   └── URP Settings
└── Resources/
    └── Data configs
```

## File Organization

### Core Systems
- `TimeManager.cs` - Manages game time (12 AM to 6 AM)
- `GameManager.cs` - Main game controller
- `RuleManager.cs` - Manages the 3 horror rules
- `SanityManager.cs` - Player sanity system
- `InventoryManager.cs` - Task tracking and inventory

### Player
- `PlayerController.cs` - Movement and interaction
- `PlayerCamera.cs` - First-person camera and look mechanics
- `Flashlight.cs` - Flashlight system with battery
- `PlayerInput.cs` - Mobile input handling
- `PlayerFootsteps.cs` - Dynamic footstep audio

### Enemy AI
- `EnemyAI.cs` - Main enemy controller
- `EnemyState.cs` - State machine implementation
- `EnemyPatrol.cs` - Patrol behavior
- `EnemyChase.cs` - Chase behavior
- `EnemyDetection.cs` - Detection system

### UI
- `MainMenuUI.cs` - Main menu controller
- `PauseMenuUI.cs` - Pause menu
- `HUD.cs` - In-game HUD display
- `SanityBar.cs` - Sanity meter UI
- `TaskDisplay.cs` - Current task display

### Audio
- `AudioManager.cs` - Master audio controller
- `SpatialAudioSource.cs` - 3D audio positioning
- `AmbiencePlayer.cs` - Ambient sound management
- `DynamicMusicSystem.cs` - Music based on game state

### Systems
- `RuleMonitor.cs` - Monitors rule violations
- `BatterySystem.cs` - Flashlight battery management
- `TaskSystem.cs` - Task tracking and completion
- `CheckpointSystem.cs` - Save/load system

## Build Configuration

See `ProjectSettings.md` for:
- Android build settings
- Quality presets
- Performance targets
- URP configuration
- Memory optimization

## Performance Targets

**Flagship Phones (Snapdragon 888+)**
- Target: 60 FPS
- Resolution: 1440p (adaptive)
- Shadows: Dynamic, 2048 resolution
- Post-processing: Full

**Mid-Range Phones (Snapdragon 778G+)**
- Target: 30 FPS
- Resolution: 1080p
- Shadows: Baked + limited dynamic
- Post-processing: Reduced

**Budget Phones (Snapdragon 480)**
- Target: 30 FPS minimum
- Resolution: 720p
- Shadows: Baked only
- Post-processing: Minimal

## Mobile Controls

```
Left Joystick: Movement (WASD)
Right Joystick: Look Around
L1: Crouch
R1: Sprint
X: Interact
Y: Toggle Flashlight
Triangle: Pause
Square: Look Away
```

## Development

### Requirements
- Unity 6.0 LTS or newer
- Android SDK API 24+
- Target API 34
- 8GB+ RAM for development
- SSD recommended

### IDE Setup
- Visual Studio or Rider recommended
- Enable code analysis
- Use provided .editorconfig

## Installation & Testing

1. **Clone Repository**
   ```bash
   git clone https://github.com/ihopetonottolosethis-commits/the-shift-horror-game.git
   cd the-shift-horror-game
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Add Project from Disk"
   - Select project folder
   - Open with Unity 6.0+

3. **Configure Android**
   - Edit → Project Settings → Player
   - Set company name and product name
   - Configure signing for APK/AAB builds

4. **Build & Deploy**
   - File → Build Settings
   - Add scenes in order: MainMenu, GameplayMuseum, etc.
   - Select Android platform
   - Click "Build and Run"

## Code Standards

- **Naming**: PascalCase for classes/methods, camelCase for variables
- **Documentation**: XML comments on public methods
- **Error Handling**: Try-catch for critical systems
- **Performance**: Object pooling for frequently created objects
- **Mobile**: No GC allocations in update loops

## License

All code and systems are proprietary © The Shift Team

## Support

For issues or questions, see project documentation in `docs/` folder.
