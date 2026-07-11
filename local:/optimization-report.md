# Optimization Report

## Done — Saves Applied

### Asset Cleanup (415MB saved)
**484MB → 68MB (86% reduction)**

Deleted folders (0 references in main scene):
- `Demo City By Versatile Studio/` — 413MB (city models, textures, lightmaps, scene)
- `HellaFlush_DriftCar_Free/` — 980K (car model, not used)
- `animation/` — 104K (old animations)
- `Script/` — 104K (old scripts)
- `Azerilo/Demo Assets/` — 4K (ground material, unused)
- Root-level: `coin.mat`, `game-icon.png`, `road material.mat` — all unreferenced

Cleaned orphaned `.meta` files from deleted folders.

### Code Optimizations (CarController3D.cs)
1. **Moved Input.GetAxis("Horizontal") from FixedUpdate → Update**: Input reads in FixedUpdate can cause jitter (FixedUpdate runs at variable rate per frame). Now reads once per frame in Update, stored in `steerInputCached`.
2. **Removed redundant GetMobileInput() from FixedUpdate**: `mobileInput` is only needed for steering x-axis, now done in Update via `steerInputCached`. FixedUpdate no longer fetches mobile input.
3. **Added `steerInputCached` field**: Private float, eliminates per-physics-frame Input.GetAxis call.

### FallDetector (previous session)
- Created `Assets/Scripts/FallDetector.cs` with Y-threshold detection + trigger zone support
- Attached to PlayerCar via Unity MCP: `fallThresholdY=-40`, `restartDelay=0.8`
- Car restarts via LevelManager.RestartCurrent() on fall
- Verified no GC allocations or physics modifications in FallDetector code

---

## Remaining: Needs Unity Editor

For further optimization, open the project in Unity and do:

### 1. Player Settings → Optimization
- **Enable Managed Stripping**: Set to Medium or High (saves build size)
- **Disable Unity Splash Screen**: `m_ShowUnitySplashScreen: 0` in ProjectSettings
- **Enable Mip Stripping**: Strip unused mip levels

### 2. URP Asset Settings (PC_RPAsset)
- Reduce `Shadow Distance` from 50 → 30
- Reduce `Shadow Cascade Count` from 4 → 2
- Set `Soft Shadow Quality` from High → Medium
- Disable `Depth Texture` and `Opaque Texture` if not used by effects

### 3. Texture Import Settings
- Set Max Size on POLYGON city pack textures: 1024 for most, 512 for small
- Enable Crunch Compression for city textures
- Set Compression to ASTC (mobile) or BC7 (desktop)

### 4. Audio Import Settings
- Set all `.wav` files to Vorbis/MP3 with quality 60%
- Set Load Type to Decompress on Load (for short SFX) or Streaming (for music)

### 5. Model Import Settings
- Enable Mesh Compression on city models (POLYGON city pack)
- Disable Read/Write on all static meshes

### 6. Quality Settings (QualitySettings.asset)
- Mobile tier: Reduce shadow distance to 25, pixel light count to 1
- PC tier: Enable 2x MSAA if not already set

### 7. Build Settings
- Enable LZ4 Compression
- Use Development Build only for testing
- Enable Dedicated Server Optimizations (already on)

---

## Car Stutter (Kedat Kedut) — Investigation

**FallDetector ruled out**: Zero allocations, no physics writes, only reads transform.position.y and does two float comparisons per frame. OnTriggerEnter with triggerOnly=false returns immediately.

**Suggested debugging:**
1. Restart Unity Editor (Play → Stop → Play again)
2. Disable FallDetector in Inspector temporarily to confirm it's unrelated
3. Check for other sources: physics timestep (0.02 default), WheelCollider friction stutter, or general Rigidbody.interpolation behavior
4. Try reducing Fixed Timestep from 0.02 to 0.016 (60Hz physics)
