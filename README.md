# WOTO - Wrath Of The Olympians

Wrath Of The Olympians is a legacy Unity fighting game project originally created as a final year project and now partially modernized so it can open and run again in a current Unity editor.

## Current State

- Original project version: Unity `5.1.2f1`
- Current repo goal: make the project compile and run in modern Unity
- Recommended editor target: Unity 6
- Current gameplay state: playable prototype / recovery build

This repository is not a finished commercial game build. It is an upgraded legacy project with:

- Unity 6 compatibility fixes for many old APIs
- repaired build scene setup
- a working main menu and stage flow
- a rebuilt `stage 1` runtime arena
- local two-player prototype combat
- procedural placeholder fighters for Zeus and Ares

## What Was Modernized

The repo now includes:

- package/bootstrap fixes so `UnityEngine.UI` and runtime scene setup work in a newer editor
- compatibility updates for old Standard Assets and editor scripts
- runtime scene recovery helpers for camera and event system setup
- a generated arena/combat slice for `stage 1`
- HUD, health bars, timer, round flow, melee attacks, and a simple ultra move

Important: the project still contains old assets and legacy content. Some parts are modernized enough to run; others are still historic project content.

## Running The Project

1. Clone the repository:

```bash
git clone https://github.com/StefanosIr/WOTO.git
cd WOTO
```

2. Open the project in Unity Hub.

3. Use Unity 6 if possible.

4. Let Unity upgrade/import the project.

5. Open one of these scenes:
- `Assets/mainmenu.unity`
- `Assets/stage 1.unity`

6. Press Play.

If you want a standalone build:

1. Open `File > Build Settings` or `Build Profiles`
2. Make sure these scenes are included:
- `Assets/mainmenu.unity`
- `Assets/stage 1.unity`
3. Build and run

## Controls

Current prototype controls:

### Player 1
- Move: `A / D`
- Jump: `Space`
- Punch: `Z`
- Kick: `X`
- Ultra: `C`

### Player 2
- Move: `J / L`
- Jump: `O`
- Punch: `U`
- Kick: `P`
- Ultra: `M`

## Scenes

- `Assets/mainmenu.unity`
  Current menu/start scene

- `Assets/stage 1.unity`
  Current rebuilt prototype arena scene

## Project Structure

Key folders:

- `Assets/Scripts/Runtime`
  Current runtime arena, combat, HUD, and fighter systems

- `Assets/Scripts/Gameplay`
  Older/custom gameplay controller scripts still kept in the project

- `Assets/Scripts/UI`
  UI scene logic

- `Assets/Editor/Bootstrap`
  Editor helpers for scene/bootstrap repair

- `Assets/Humanoid`
  Legacy humanoid asset content

- `Assets/Standard Assets`
  Legacy Unity Standard Assets content

## Notes

- `ProjectSettings/ProjectVersion.txt` still records the original Unity version because this started as a Unity 5 project.
- The current runtime fighter visuals are procedural placeholder demigod characters, not final production art.
- `stage 1` is currently a reconstructed gameplay slice, not a verified restoration of the original 2015 gameplay scene.
- This repo is best treated as an upgraded prototype and recovery effort.

## Documentation

Additional historical/upgrade docs in the repo:

- [`UPGRADE_GUIDE.md`](UPGRADE_GUIDE.md)
- [`MODERNIZATION_REPORT.md`](MODERNIZATION_REPORT.md)

## Author

Stefanos Irodotou
