# The Shift - Android Build Configuration

## Pre-Build Setup

### Android SDK/NDK Installation
1. Open Android Studio
2. SDK Manager > SDK Platforms
3. Install:
   - Android 14 (API 34) ← Target
   - Android 7.0 (API 24) ← Minimum
4. SDK Tools:
   - Android SDK Build-Tools 34.0.0
   - NDK (latest)
   - Android Emulator (optional)

### Unity Android Setup
1. Edit > Preferences (Mac: Unity > Preferences)
2. External Tools > Android:
   - Android SDK Path: `<your-android-sdk>`
   - Android NDK Path: `<your-android-ndk>`
   - OpenJDK: Use embedded OpenJDK or set path
3. Click "Done"

## Build Settings Configuration

### Player Settings

**Company Name**:
- Set to your studio/dev name

**Product Name**:
- "The Shift"

**Version**:
- Version: 1.0
- Bundle Version Code: 1

**Minimum API Level**:
- 24 (Android 7.0)

**Target API Level**:
- 34 (Android 14)

**Scripting Backend**:
- IL2CPP (better performance than Mono)

**API Compatibility Level**:
- .NET Framework

**Architecture**:
- ARM64 only (no ARMv7)

**Graphics**:
- Graphics APIs: Vulkan (primary), OpenGL ES 3.2 (fallback)
- Auto Graphics API: OFF
- Instancing Variants: Stripped

### Resolution and Presentation

**Default Orientation**:
- Portrait

**Allowed Orientations**:
- Portrait only

**Target Resolution**:
- Leave blank (adapts to device)

**Fullscreen**:
- ON

**Status Bar Hidden**:
- ON

**Notch Support**:
- ON

### Other Settings

**Rendering**:
- Color Space: Linear (required for proper lighting)
- Use 32-bit Display Buffer: OFF
- Use ASTC Instead of ETC2: ON (mobile optimization)
- Vulkan Instance Extensions: Leave default

**Performance**:
- Optimize Mesh Data: ON
- Optimize Mesh Polygons: ON
- Optimize Frame Pacing: ON
- Managed Stripping Level: Medium
- Code Stripping: Aggressive
- IL2CPP Code Generation: Faster (smaller builds)

**Quality**:
- V Sync: Double Buffering
- Target Frame Rate: -1 (device default)

## Keystore Setup (For Release Builds)

### Create Keystore
1. Edit > Project Settings > Player > Publishing Settings
2. Keystore Manager > "Create New"
3. Set password (write it down!)
4. Key alias: "the_shift_key"
5. Key password: (same or different)
6. Key validity: 50+ years
7. Click "Create"

### Use Keystore in Build
1. Publishing Settings > Use Custom Keystore: ON
2. Select your .keystore file
3. Enter keystore password
4. Enter key alias password

## Build Process

### Development Build (for testing)
```
File > Build Settings
├── Platform: Android
├── Development Build: ON
├── Script Debugging: ON
├── Click "Build and Run"
└── Select Android device
```

### Production Build (for release)
```
File > Build Settings
├── Platform: Android
├── Development Build: OFF
├── Script Debugging: OFF
├── Create Symbols.zip: ON
├── Build App Bundle (for Google Play)
└── Click "Build"
```

## Output Files

**APK** (for direct installation):
- `the-shift.apk`
- Size: ~150-200MB (typical)
- Install: `adb install the-shift.apk`

**AAB** (for Google Play):
- `the-shift.aab`
- Upload to Play Console
- Google optimizes for each device

## Performance Optimization for Mobile

### Before Building

1. **Texture Optimization**
   - Use ASTC 6x6 compression
   - Target 256MB VRAM budget
   - Implement texture streaming

2. **Mesh Optimization**
   - Enable mesh compression
   - Use LOD groups
   - Combine static meshes

3. **Shader Optimization**
   - Use mobile-optimized URP shaders
   - Avoid complex calculations
   - Bake lighting where possible

4. **Scene Optimization**
   - Enable Occlusion Culling
   - Set proper culling distances
   - Use spatial partitioning

5. **Audio Optimization**
   - Compress audio (Vorbis for OGG)
   - Limit simultaneous audio sources
   - Use mono for 3D spatial audio

## Testing on Device

### First Time Setup
1. Enable Developer Mode on Android device
2. Enable USB Debugging
3. Connect device via USB
4. Authorize computer on device
5. Run `adb devices` (should list your device)

### Performance Monitoring
1. In-game profiler: Window > Analysis > Profiler
2. Monitor:
   - Frame rate (target 60 or 30 FPS)
   - Memory usage (< 800MB)
   - CPU/GPU load
   - Thermal throttling

### Debugging
1. Connect device
2. Window > General > Device Console
3. View device logs in real-time
4. Filter by "[PlayerLog]"

## Troubleshooting

### Build Fails
- Ensure NDK installed
- Check API level compatibility
- Clear Temp/Library folders
- Restart Unity

### Low Frame Rate
- Reduce shadow resolution
- Disable dynamic shadows
- Lower texture quality
- Reduce draw calls

### Crashes on Startup
- Check IL2CPP compilation errors
- Verify managed code stripping
- Test on different device
- Check logcat with `adb logcat`

### Audio Issues
- Check Audio Listener setup
- Verify spatial blend settings
- Test on different device
- Check audio format compatibility

## App Store Submission (Google Play)

1. Create Google Play Developer account ($25)
2. Create app listing
3. Prepare screenshots and description
4. Generate AAB (not APK)
5. Upload to Play Console
6. Set pricing and ratings
7. Submit for review (24-48 hours)
8. Monitor reviews and ratings

## Distribution Channels

- **Google Play**: Primary distribution
- **Samsung Galaxy Store**: Secondary
- **Side-loading**: Direct APK distribution
- **Cloud Gaming**: GeForce Now, Xbox Cloud

## Version Updates

For new versions:
1. Increment Bundle Version Code
2. Update Version string
3. Rebuild AAB
4. Upload to Play Console
5. Set rollout percentage (gradual)
6. Monitor for crashes

## Security Considerations

- Enable code stripping (remove debug info)
- Obfuscate code with ProGuard/R8
- Sign all releases with keystore
- Keep keystore password secure
- Never commit keystore to git
- Use `.gitignore` for .keystore files

## Size Optimization

**Target Build Size**: < 200MB

- Use compressed textures (ASTC)
- Stream assets (don't load all at once)
- Remove unused assets
- Use minimal audio quality
- Strip unused features

## Minimum Device Requirements

- **RAM**: 2GB minimum (4GB recommended)
- **Storage**: 200MB free
- **GPU**: Vulkan support (Mali-G76+, Adreno 640+)
- **CPU**: Snapdragon 480+ or equivalent
- **OS**: Android 7.0+ (API 24+)

## Recommended Testing Devices

- Pixel 6/7/8 (Google flagship)
- Samsung Galaxy S21+ (Samsung flagship)
- OnePlus 9/10 (Mid-range high-end)
- Xiaomi Redmi Note 11 (Mid-range budget)
- Motorola Moto G7 (Budget)
