# The Shift - Scene Setup Guide

## Scene Structure

The game uses 5 main scenes:

### 1. MainMenu.unity
**Purpose**: Game entry point and main menu

**Setup Instructions**:
1. Create new scene, name it "MainMenu"
2. Create Canvas with:
   - Background Image (dark, semi-transparent)
   - Title Text: "THE SHIFT" (large, centered)
   - Start Button
   - Settings Button
   - Credits Button
   - Quit Button
3. Add AudioListener component
4. Add MainMenuUI script to Canvas
5. Save scene

**Hierarchy**:
```
MainMenu
├── Canvas
│   ├── Background
│   ├── TitleText
│   ├── StartButton
│   ├── SettingsButton
│   ├── CreditsButton
│   └── QuitButton
├── AudioListener
└── EventSystem
```

### 2. GameplayMuseum.unity
**Purpose**: Main gameplay scene - the museum

**Setup Instructions**:

#### Lighting
1. Set Ambient Light: RGB(80, 80, 90) - Subtle blue tone
2. Create Directional Light (simulates moonlight):
   - Intensity: 0.6
   - Color: RGB(200, 200, 220) - Cool tone
   - Rotation: X=45, Y=-45
   - Bake shadows
3. Add point lights in key areas:
   - Emergency lighting: Red tint, 0.3 intensity
   - Exit signs: Green tint, 0.2 intensity

#### Environment Setup
1. Create Ground plane (museum floor)
2. Create main hallway with walls
3. Add central sarcophagus display
4. Create rooms:
   - Security Office
   - Storage Room (2x)
   - Archive Room
   - Janitor Closet
   - Bathroom

#### Player Setup
1. Create empty GameObject: "Player"
2. Add CharacterController component
3. Add PlayerController script
4. Create child object: "CameraHolder"
   - Add Camera component
   - Set FOV to 60
   - Position: (0, 0.6, 0) relative to Player
5. Add child object: "FlashlightObject"
   - Add Light component (Spotlight)
   - Intensity: 2.0
   - Range: 50
   - Spot Angle: 30
   - Add Flashlight script
6. Add child object: "CameraVisuals"
   - Add PlayerCamera script to CameraHolder
7. Set Player position in spawning area

#### Enemy Setup
1. Create enemy model/capsule: "Enemy"
2. Add CharacterController component
3. Add EnemyAI script
4. Assign patrol points (waypoints):
   - Main hallway center
   - Security office
   - Archive room
   - Storage room
   - Back hallway
5. Set EnemyAI references

#### Interactive Objects
For each interactive item, create GameObject with:
- Collider (trigger)
- InteractiveObject script
- Set InteractionType (Task, Sarcophagus, Mirror, etc.)
- Set taskId if applicable

**Example - Sarcophagus**:
```
Sarcophagus
├── Model (visual)
├── Collider (trigger, size adjusted)
└── InteractiveObject script
    ├── type: Sarcophagus
    ├── prompt: "The sarcophagus... don't look too long"
```

#### Audio Setup
1. Create empty GameObject: "AudioManager"
2. Add AudioManager script
3. Create Resources/Audio folders:
   ```
   Assets/Resources/Audio/
   ├── SFX/
   │   ├── footstep_stone_1.wav
   │   ├── footstep_stone_2.wav
   │   ├── whisper_1.wav
   │   ├── creak_1.wav
   │   ├── children_laughing.wav
   │   └── sarcophagus_rattle.wav
   └── Music/
       ├── ambient_midnight.ogg
       ├── ambient_early_night.ogg
       ├── ambient_deep_night.ogg
       └── ambient_late_night.ogg
   ```

#### UI Setup
1. Create Canvas: "HUD"
2. Add UI elements:
   - TimeDisplay (top center)
   - SanityBar (left side)
   - BatteryBar (left side)
   - TaskDisplay (right side)
   - InteractionPrompt (bottom center)
   - WarningIndicators (center)
3. Add HUD script to Canvas

#### Scene Managers
1. Create empty GameObject: "GameManagers"
2. Add components:
   - GameManager
   - TimeManager
   - SanityManager
   - RuleManager
   - InventoryManager
   - CheckpointSystem
   - BatterySystem
3. Wire up all references in Inspector

**Final Hierarchy**:
```
GameplayMuseum
├── Lighting
│   ├── Directional Light (Moon)
│   └── Point Lights (Emergency)
├── Environment
│   ├── Ground
│   ├── Walls/Hallways
│   ├── Sarcophagus
│   ├── Doors
│   ├── Furniture
│   └── InteractiveObjects
├── Player
│   ├── CameraHolder
│   │   └── Camera
│   ├── FlashlightObject
│   │   └── Light (Spotlight)
│   └── Collider
├── Enemy
│   ├── Model
│   └── Collider
├── UI
│   └── HUD Canvas
├── GameManagers
│   ├── GameManager
│   ├── TimeManager
│   ├── SanityManager
│   ├── RuleManager
│   ├── InventoryManager
│   └── AudioManager
├── AudioListener
└── EventSystem
```

### 3. PauseMenu.unity
**Purpose**: Pause menu scene (overlaid during gameplay)

**Setup Instructions**:
1. Create new scene
2. Create Canvas with:
   - Pause Panel (dark overlay)
   - "PAUSED" Title
   - Resume Button
   - Settings Button
   - Main Menu Button
3. Add PauseMenuUI script
4. Save scene

### 4. GameOver.unity
**Purpose**: Game over screen when player is caught

**Setup Instructions**:
1. Create Canvas with:
   - Background (dark red tint)
   - "ENTITY CLAIMED YOU" message
   - Stats display (playtime, sanity, tasks)
   - Retry Button
   - Main Menu Button
2. Add GameOverUI script
3. Save scene

### 5. Victory.unity
**Purpose**: Victory screen when player survives until 6 AM

**Setup Instructions**:
1. Create Canvas with:
   - Background (bright, celebratory)
   - "YOU SURVIVED THE NIGHT" message
   - Stats display
   - Continue Button
   - Main Menu Button
2. Add VictoryUI script
3. Save scene

## Scene Loading Order

In Build Settings (File > Build Settings):
```
0. MainMenu.unity
1. GameplayMuseum.unity
2. PauseMenu.unity
3. GameOver.unity
4. Victory.unity
```

## Lighting Setup for "Not Too Dark" Museum

### Ambient Lighting
- Use gradient ambient light
- Sky color: RGB(60, 70, 100) - Cool blue night
- Equator: RGB(100, 80, 60) - Warm horizon
- Ground: RGB(40, 40, 50) - Dark ground

### Post-Processing
Add Post-Processing Volume to GameplayMuseum:
1. Bloom:
   - Intensity: 0.5
   - Threshold: 1.0
2. Film Grain:
   - Intensity: 0.2
   - Response: 0.95
3. Color Grading:
   - Temperature: 0 (neutral)
   - Tint: 0
   - Saturation: 0.8 (slightly desaturated for horror)
4. Vignette:
   - Intensity: 0.15
   - Smoothness: 0.5

## Testing the Scenes

1. Start with MainMenu scene
2. Click Start to load GameplayMuseum
3. Test movement (WASD or touch joystick)
4. Test camera look (mouse drag or touch drag)
5. Test flashlight (F key or Y button)
6. Test interaction (E key or X button)
7. Test pause (ESC key)

## Mobile Testing

For Android:
1. Connect device
2. File > Build Settings
3. Select "Android" platform
4. Click "Build and Run"
5. Test touch controls:
   - Left side: Movement joystick
   - Right side: Look around (drag)
   - Buttons: Sprint, Crouch, Interact, Flashlight

## Performance Profiling

While playing:
1. Window > Analysis > Profiler
2. Monitor:
   - CPU: Game thread should stay < 16.67ms (60 FPS)
   - GPU: Rendering < 16.67ms
   - Memory: < 800MB total
3. Look for spikes in:
   - Physics.Raycast (enemy detection)
   - Rendering (shadows)
   - Audio (spatial audio calculations)
