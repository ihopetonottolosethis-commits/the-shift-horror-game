# The Shift - Project Settings & Configuration

## Unity Version
- **Minimum**: Unity 6.0 LTS
- **Recommended**: Unity 6.0.1 or later
- **Graphics API**: Vulkan (primary), OpenGL ES 3.2 (fallback)

## Project Settings

### Graphics
- **Rendering Pipeline**: Universal Render Pipeline (URP)
- **Color Space**: Linear (required for proper PBR lighting)
- **Rendering Path**: Forward (mobile optimized)
- **Anti-aliasing**: FXAA 2x (mobile)
- **HDR**: Enabled

### Quality Settings

#### High Quality (Flagship Phones - Snapdragon 888+)
```
Shadow Distance: 100m
Shadow Resolution: 2048 (high resolution)
Texture Quality: Full Resolution
Anisotropic Filtering: Per Texture
LOD Bias: 1.0
VSync: Double Buffering
Target Frame Rate: 60 FPS
Time.fixedDeltaTime: 0.01667 (60 Hz)
```

#### Medium Quality (Mid-Range Phones - Snapdragon 778G+)
```
Shadow Distance: 50m
Shadow Resolution: 1024 (medium)
Texture Quality: 1/2 Resolution
Anisotropic Filtering: Trilinear
LOD Bias: 1.2
VSync: Double Buffering
Target Frame Rate: 30 FPS
Time.fixedDeltaTime: 0.03333 (30 Hz)
```

#### Low Quality (Budget Phones - Snapdragon 480)
```
Shadow Distance: 30m
Shadow Resolution: 512 (low)
Texture Quality: 1/4 Resolution
Anisotropic Filtering: Bilinear
LOD Bias: 2.0
VSync: Double Buffering
Target Frame Rate: 30 FPS (minimum)
Time.fixedDeltaTime: 0.03333 (30 Hz)
```

### Android Build Settings

```
Minimum API Level: 24 (Android 7.0 Nougat)
Target API Level: 34 (Android 14)
Compilation Backend: IL2CPP
Architecture: ARM64 (arm64-v8a) only
Graphics API: Vulkan preferred, OpenGL ES 3.2 fallback
Internet Required: Yes
Screen Orientation: Portrait (locked)
Default Orientation: Portrait
```

### Physics Settings
```
Gravity: (0, -9.81, 0) m/s²
Default Material Friction: 0.4
Default Material Bounciness: 0.0
Fixed Timestep: 0.02 seconds (50 Hz physics updates)
Default Solver Iterations: 6
Default Solver Velocity Iterations: 1
QueriesHitBackfaces: false
```

### Audio Settings
```
Sample Rate: 48 kHz (higher quality)
DSP Buffer Size: Best Latency
Max Virtual Voices: 32
Max Real Voices: 16
Spatial Audio: Enabled
Dolby Atmos: Enabled (if supported)
Default Speaker Mode: Stereo
```

### Input Manager

**Horizontal Axis**
- Positive Button: d
- Negative Button: a
- Alt Positive Button: Right
- Alt Negative Button: Left
- Gravity: 3
- Dead: 0.19
- Sensitivity: 1
- Snap: false
- Invert: false
- Type: Key or Mouse Button
- Axis: Joystick 1 Left Stick X

**Vertical Axis**
- Positive Button: w
- Negative Button: s
- Alt Positive Button: Up
- Alt Negative Button: Down
- Gravity: 3
- Dead: 0.19
- Sensitivity: 1
- Snap: false
- Invert: false
- Type: Key or Mouse Button
- Axis: Joystick 1 Left Stick Y

**Look Horizontal**
- Type: Joystick Axis
- Axis: Joystick 1 Right Stick X
- Gravity: 0
- Dead: 0.1
- Sensitivity: 2

**Look Vertical**
- Type: Joystick Axis
- Axis: Joystick 1 Right Stick Y
- Gravity: 0
- Dead: 0.1
- Sensitivity: 2
- Invert: true

### URP Settings

#### Asset Settings
- **High Fidelity**: 
  - Shadow Distance: 100
  - Shadow Cascade Splits: [0.067, 0.267]
  - Main Light Shadows: On
  - Shadow Resolution: 2048

- **Medium Fidelity**:
  - Shadow Distance: 50
  - Shadow Cascade Splits: [0.1, 0.3]
  - Main Light Shadows: On
  - Shadow Resolution: 1024

- **Low Fidelity**:
  - Shadow Distance: 30
  - Shadow Cascades: Single Cascade
  - Main Light Shadows: On
  - Shadow Resolution: 512

#### Forward Renderer
```
Depth Priming: Enabled
Native Render Pass: Enabled (better performance)
Opaque Layer Mask: Everything (except UI)
Transparent Layer Mask: Everything (except UI)
OpaqueTexture: Enabled
OpaqueDownsampling: 2x
```

#### Features Enabled
- Render Objects Pass (for special effects)
- Screen Space Ambient Occlusion (SSAO)
- Decals (optional)

#### Features Disabled (Performance)
- Screen Space Shadows
- Motion Vectors
- Full Screen Bloom Pass (use post-processing bloom)

### Scene Setup

#### Main Camera
```
FOV: 60 degrees (realistic human vision)
Near Clipping Plane: 0.01
Far Clipping Plane: 1000
Clear Flags: Skybox
Viewport Rect: Full Screen (0, 0, 1, 1)
Occlusion Culling: Enabled
HDR: Enabled
Dynamic Resolution: Enabled (mobile)
Target Texture: None
```

#### Lighting Setup
```
Environment Lighting:
  Source: Gradient
  Skybox Intensity: 1.0
  Ambient Light: (0.3, 0.3, 0.35, 1.0)
  
Realtime Global Illumination: Disabled (performance)
Baked Global Illumination: Enabled
  
Directional Light (Main):
  Type: Directional
  Intensity: 1.0
  Color: (1, 1, 0.95, 1) - Slightly warm
  Cast Shadows: On (baked)
  Shadow Type: Hard Shadows
```

### Post-Processing Stack

#### Bloom
```
Intensity: 0.5
Threshold: 1.0
Scatter: 0.7
Clamp: 65472.0
```

#### Film Grain
```
Type: Thin Film (perceptually accurate)
Intensity: 0.15
Response: 0.95
```

#### Ambient Occlusion
```
Mode: Scalable Ambient Obscurance (SAO)
Intensity: 1.0
Radius: 3.0
Samples: 4 (mobile optimized)
```

#### Color Grading
```
Mode: Linear
Lut Size: 32
Contribution: 1.0
Temperature: 5000K (neutral)
Tint: 0
Saturation: 1.0
Contrast: 1.0
Linear Sections: 2
```

### Memory & Performance

#### Texture Streaming
- **Enabled**: Yes
- **Budget**: 128 MB
- **Unload Unused**: Yes
- **Max Level Reduction**: 2

#### Asset Compression
- **Texture Format**: ASTC 6x6 (Android)
- **Normal Maps**: BC5 (when converted)
- **Mesh Compression**: High
- **Animation Compression**: Optimal

#### Object Pooling
- Enable pooling for:
  - Footstep particles
  - Bullet shells
  - UI elements
  - Audio sources

#### Memory Budget
- **Total Budget**: 800 MB
- **Gameplay Active**: 500-600 MB
- **UI/Menus**: 50-100 MB
- **Streaming Buffer**: 100-150 MB

### Optimization Checklist

- [x] IL2CPP backend (faster than Mono)
- [x] Managed Stripping Level: High
- [x] Bytecode Stripping: Enabled
- [x] Optimize Mesh Data: Enabled
- [x] Optimize Mesh Polygons: Enabled
- [x] Async Upload Buffer Size: 4 MB
- [x] Time.timeScale: Properly managed
- [x] Physics: Fixed timestep optimized
- [x] Shadows: Baked where possible
- [x] LOD Groups: Implemented
- [x] Occlusion Culling: Room-based

## Build Configuration

### Development Build
```
Development Build: Enabled
AutoConnect Profiler: Yes
Script Debugging: Enabled
WaitForManagedDebugger: No
Symbols: Included
```

### Production Build
```
Development Build: Disabled
AutoConnect Profiler: No
Script Debugging: Disabled
WaitForManagedDebugger: N/A
Symbols: Stripped (ProGuard/R8)
Code Stripping: Aggressive
```

### APK/AAB Configuration
```
Enable Split APKs By Target API: Yes
Enable Split APKs By Screen Size: No
Enable Split APKs By CPU Architecture: Yes (ARM64 only)
Create symbols.zip: Yes
Build App Bundle: Yes (for Play Store)
```

## Scripting Backend

### IL2CPP Settings
```
C++ Compiler Configuration: Release
Target Architecture: ARM64 only
IL2CPP Code Generation: Faster (smaller builds)
Arm64 Neon Support: Enabled
Enable Arm64 Neon SIMD: Yes
```

## Prefab Optimization

- Use blueprint mode for complex hierarchies
- Instance ID optimization
- Component pooling for frequently instantiated objects
- Networked objects: Disable unused components

## Performance Profiling

### Key Metrics to Monitor
- GPU: Renderthread time < 16.67ms (60 FPS) or < 33.33ms (30 FPS)
- CPU: Game thread < 16.67ms or < 33.33ms
- Memory: Total < 800 MB
- Battery: 2-3% per minute on idle
- Thermal: < 40°C during gameplay

### Profiler Markers
- PlayerMovement
- EnemyAI
- AudioUpdate
- RenderFrame
- PhysicsUpdate

## Troubleshooting

### Performance Issues
1. Check shadow rendering time (Profiler → Rendering)
2. Verify LOD levels are configured
3. Enable Occlusion Culling
4. Reduce draw calls (batch materials)
5. Profile memory allocations (GC.Alloc)

### Mobile Specific
1. Test on actual device (not emulator)
2. Monitor thermal throttling
3. Check battery drain
4. Verify audio latency
5. Test with different GPU architectures
