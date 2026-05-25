# PLAN — City Checkpoint Driver MVP

## Status
**Implementation status:** MVP feature pass selesai.  
**Scene:** `Assets/POLYGON city pack/scene/DemoScene.unity`  
**Target publish pertama:** PC playable MVP.

Game sekarang fokus ke 2 mode:
1. **Checkpoint Level** — lewati checkpoint berurutan sebelum timer habis.
2. **Free Drive** — berkendara bebas tanpa misi/timer.

---

## Implemented Features

### Core Driving
- [x] `PlayerCar` bisa dikendarai.
- [x] Rigidbody, body collider, wheel colliders configured.
- [x] Forward/reverse, steering, brake, handbrake.
- [x] Wheel visual rotation fixed dengan visual offset.
- [x] Restart reset velocity, angular velocity, wheel torque/brake/steer.

### Camera
- [x] Camera gameplay pakai `_Scene/Camera/Main Camera`.
- [x] `CarFollowCamera` follow `PlayerCar`.
- [x] `CameraTargetBinder` bind camera ke player.
- [x] Single active `AudioListener`.

### Game Flow
- [x] `GameManager`.
- [x] `LevelManager`.
- [x] `GameMode` enum.
- [x] `GameState` enum.
- [x] Main menu.
- [x] Level select.
- [x] Free Drive.
- [x] Pause/resume.
- [x] Restart.
- [x] Result screen.
- [x] Return main menu.

### Levels
- [x] Level 1 playable: 4 checkpoint.
- [x] Level 2 playable: 6 checkpoint.
- [x] Level 2 unlock setelah Level 1 complete.
- [x] Level 3+ tampil `Coming Soon`.
- [x] Level system scalable via `LevelManager.LevelConfig[]`.

### Checkpoints
- [x] `Checkpoint.cs` trigger.
- [x] `CheckpointGroup.cs` order manager.
- [x] Checkpoint aktif satu per satu.
- [x] Checkpoint grounded, tidak ngambang.
- [x] Checkpoint visual merah, 50% transparansi.
- [x] Checkpoint lebih sempit dan tinggi.
- [x] Checkpoint sound `checkpoint.mp3`.

### UI/HUD
- [x] Main menu UI.
- [x] Level select UI.
- [x] HUD timer.
- [x] HUD checkpoint count.
- [x] HUD speedometer.
- [x] Direction arrow ke checkpoint.
- [x] Distance text ke checkpoint.
- [x] Mini map sederhana: player arrow + checkpoint dot.
- [x] Pause menu.
- [x] Result screen.

### Save
- [x] `SaveManager` pakai `PlayerPrefs`.
- [x] Save unlocked level.
- [x] Save best time.
- [x] Save best stars.

### Audio
- [x] `GameAudioManager`.
- [x] Background music: `game-backsound.mp3`.
- [x] Car start sound: `car-engine.mp3`.
- [x] Car loop sound: `car-loop.mp3`.
- [x] Crash sound: `car-crash.mp3`.
- [x] Honk sound: `car-honk.mp3` on `H`.
- [x] Checkpoint sound: `checkpoint.mp3`.

---

## Current Scene Architecture

```text
_Scene
  Environment
  Gameplay
    PlayerCar
      Visuals
      Physics
      Scripts
    SpawnPoints
      FreeDriveSpawn
      Level01Spawn
      Level02Spawn
    Checkpoints
      Level01
        CP_01 ... CP_04
      Level02
        CP_01 ... CP_06
  Camera
    Main Camera
  Lighting
  UI
    GameCanvas
  Managers
    GameManager
    AudioManager
```

---

## Scripts Implemented

- `Assets/Scripts/CarController3D.cs`
- `Assets/Scripts/CarFollowCamera.cs`
- `Assets/Scripts/CameraTargetBinder.cs`
- `Assets/Scripts/CarAudioController.cs`
- `Assets/Scripts/GameAudioManager.cs`
- `Assets/Scripts/GameMode.cs`
- `Assets/Scripts/GameState.cs`
- `Assets/Scripts/GameManager.cs`
- `Assets/Scripts/LevelManager.cs`
- `Assets/Scripts/Checkpoint.cs`
- `Assets/Scripts/CheckpointGroup.cs`
- `Assets/Scripts/CheckpointVisualPulse.cs`
- `Assets/Scripts/GameplayUI.cs`
- `Assets/Scripts/SaveManager.cs`

---

## MVP Acceptance Criteria

- [x] Game bisa mulai dari main menu.
- [x] Free Drive bisa dimainkan.
- [x] Level 1 bisa dimainkan.
- [x] Level 2 bisa dimainkan setelah unlock.
- [x] Level 3+ tampil `Coming Soon`.
- [x] Timer/checkpoint HUD jalan di checkpoint mode.
- [x] Timer/checkpoint disembunyikan di Free Drive.
- [x] Result screen muncul saat complete/fail.
- [x] Restart kembali ke spawn.
- [x] Save progress berjalan.
- [x] Camera render dari `_Scene/Camera`.
- [x] Audio utama terpasang.
- [x] Console terakhir diuji tanpa error fatal.

---

## Remaining Before Publish

### Must Do
- [ ] Build PC test via Unity Build Settings.
- [ ] Test fresh playthrough dari save kosong.
- [ ] Test Level 1 complete unlock Level 2.
- [ ] Test Level 2 complete result screen.
- [ ] Test Free Drive restart.
- [ ] Test audio balance pakai speaker/headphone.

### Nice To Have
- [ ] Settings menu sederhana untuk volume.
- [ ] Reset save button untuk testing/user.
- [ ] Better minimap frame/art.
- [ ] Better checkpoint arrow art.
- [ ] Add start countdown `3, 2, 1, GO`.
- [ ] Add UI click SFX.

---

## Scalability Notes

Sistem sengaja dibuat scalable:
- Tambah level baru cukup tambah spawn point, checkpoint group, dan `LevelConfig`.
- `CheckpointGroup` otomatis urut checkpoint berdasarkan index.
- `SaveManager` menyimpan progress berdasarkan level index.
- Level select saat ini hardcoded untuk MVP; nanti bisa diganti dynamic list.
- Checkpoint visual bisa dijadikan prefab nanti.
- Level data bisa dipindah ke ScriptableObject nanti.

---

## Do Not Add Yet
Agar scope publish tetap kecil, jangan tambah dulu:
- delivery
- parking
- traffic AI
- police chase
- car upgrade
- garage
- multiplayer
- complex damage
- open world quest

---

## Next Recommended Step
1. Build PC test.
2. Full playthrough test dari main menu.
3. Balance timer Level 1 & Level 2.
4. Adjust audio volume jika terlalu keras/pelan.
5. Publish MVP.
