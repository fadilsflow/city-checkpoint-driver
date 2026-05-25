# City Checkpoint Driver

City Checkpoint Driver is a 3D arcade driving game built in Unity. Drive through an urban route, hit checkpoints in sequence, beat the timer, or switch to free drive and explore the city without objectives.

Built with Unity `6000.3.12f1` and Universal Render Pipeline (URP).

## Features

- Arcade-style 3D car handling with throttle, reverse, braking, steering, and handbrake controls
- Checkpoint race mode with ordered route progression and timer pressure
- Free drive mode for open city exploration
- HUD with speed, timer, checkpoint progress, direction guidance, and minimap support
- Star rating and saved progress for completed levels
- Background music, engine audio, checkpoint feedback, crash sound, and honk SFX
- Third-person follow camera
- Low-poly city scene and imported car/environment assets

## Game Modes

| Mode | Description |
| --- | --- |
| Checkpoint Level | Complete checkpoints in order before time runs out. |
| Free Drive | Explore the city without timers, objectives, or failure conditions. |

## Controls

| Input | Action |
| --- | --- |
| `W` / Up Arrow | Accelerate |
| `S` / Down Arrow | Reverse |
| `A` / `D` | Steer |
| `Space` | Brake |
| Left Shift | Handbrake |
| `H` | Honk |
| `Esc` | Pause / resume |

## Requirements

- Unity `6000.3.12f1` or another compatible Unity 6 editor version
- Git LFS for binary Unity assets
- Universal Render Pipeline packages are restored through Unity Package Manager

## Getting Started

```bash
git clone https://github.com/fadilsflow/city-checkpoint-driver.git
cd city-checkpoint-driver
git lfs pull
```

Then open the project folder in Unity Hub and load:

```text
Assets/Scenes/main.unity
```

Press Play in the Unity editor to run the game.

## Project Structure

```text
Assets/
  Scenes/             Main Unity scene
  Scripts/            Gameplay, checkpoint, UI, camera, save, and audio scripts
  Settings/           URP render pipeline settings
  Sound/              Music and sound effects
  POLYGON city pack/  City environment assets
  Azerilo/            Car model asset
  animation/          Animation clips and controllers
Packages/             Unity package manifest and lock file
ProjectSettings/      Unity project settings
GDD.md                Game design document
PLAN.md               Implementation plan
```

## Key Scripts

| Script | Responsibility |
| --- | --- |
| `CarController3D.cs` | Car movement, wheel physics, steering, and braking |
| `CarFollowCamera.cs` | Third-person camera follow behavior |
| `CarAudioController.cs` | Car engine and vehicle sound effects |
| `GameManager.cs` | Game state and flow orchestration |
| `LevelManager.cs` | Level setup, spawning, and checkpoint tracking |
| `Checkpoint.cs` / `CheckpointGroup.cs` | Checkpoint trigger logic and ordering |
| `GameplayUI.cs` | HUD, route guidance, and minimap UI |
| `SaveManager.cs` | PlayerPrefs-based save and progress data |
| `GameAudioManager.cs` | Music and global SFX playback |

## Version Control Notes

This repository is configured for Unity source control:

- `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`, builds, generated IDE files, and OS files are ignored.
- Binary assets such as models, textures, audio, archives, videos, fonts, and PDFs are tracked with Git LFS.
- Unity YAML assets use line-ending and merge settings through `.gitattributes`.

## Build

Open Unity and use:

```text
File > Build Settings > PC, Mac & Linux Standalone > Build
```

## Design Docs

- [Game Design Document](GDD.md)
- [Implementation Plan](PLAN.md)

## Asset Credits and License

This is a student/game-development portfolio project. Third-party assets remain under their original licenses and are included only for use with this Unity project.

- POLYGON City Pack by Synty Studios
- Car model asset by Azerilo
- Additional audio and art assets as included under `Assets/`

Project code may be referenced for learning purposes. Check third-party asset licenses before reusing or redistributing assets outside this project.
