# 🧟‍♂️ Swarm City

A Unity-powered first-person survival prototype set in a gritty urban block overrun by zombies. This repo highlights clean gameplay code, lightweight AI building blocks, and production-ready content aimed to impress recruiters and collaborators.

## ✨ Highlights
- **Modern Unity (2025)** — Built on Unity `6000.2.13f1`, ready for current engine pipelines.
- **Gameplay-ready controllers** — Smooth first-person movement with mouse look, crouching, and noise-aware footstep logic that feeds AI perception.
- **Cinematic intro** — Optional video splash that can be skipped with any key for rapid iteration.
- **Diegetic UI** — Simple HUD elements that react to player stance and objectives.
- **Content-rich scene** — Includes vehicles, city props, ambient audio, and zombie characters to showcase level dressing.

## 🧠 AI & Gameplay Architecture
| System | What it does | Key script |
| --- | --- | --- |
| Finite State Machine | Minimal `IState`/`StateMachine` pair driving zombie behaviors like patrol, investigate, chase, and search. | `Assets/AI/Core/StateMachine.cs` |
| Blackboard memory | Shares last seen/heard positions and suspicion across sensors & states. | `Assets/AI/Core/Blackboard.cs` |
| Vision sensor | Field-of-view checks with optional occlusion, time-limited memory, and in-editor gizmos. | `Assets/AI/Perception/VisionSensor.cs` |
| Hearing sensor | Listens for noise events (footsteps, clicks, crouched movement) with crouch suppression & debug gizmos. | `Assets/AI/Perception/HearingSensor.cs` |
| Noise event bus | Global dispatcher that scores and serves the strongest nearby sound to listeners. | `Assets/AI/Perception/NoiseEventBus.cs` |
| Navigation adapter | Thin wrapper around `NavMeshAgent` for clean movement semantics & arrival detection. | `Assets/AI/Move/NavAgentAdapter.cs` |
| Influence map | Decaying 2D grid for spatial reasoning/heatmaps, rendered with gizmos for quick tuning. | `Assets/AI/Coord/InfluenceMap.cs` |

### Zombie Brain (behavior loop)
The `ZombieBrain` composes the above pieces into a readable FSM:
- **Patrol** between waypoints until a stimulus appears.
- **Investigate** the strongest recent sound and retarget as new noises arrive.
- **Chase** the player, with a grace period before losing sight.
- **Search** radial probes around the last known position, then de-escalate suspicion.

Each state is self-contained, making it easy to extend with new behaviors (e.g., flank, retreat) without rewriting the loop.

### Player Systems
- **First-person locomotion** with mouse look, WASD movement, gravity, and optional crouch that dynamically adjusts controller height and camera. Noise events are emitted per footstep and suppressed while crouched for stealth-oriented play. (`Assets/Game/PlayerControllerFP.cs`)
- **Click-to-lure** utility that spawns attractor noises for testing hearing AI. (`Assets/Game/ClickNoise.cs`)
- **Stance-aware UI** showing whether the player is standing or crouched. (`Assets/Game/PlayerStatusUI.cs`)
- **Intro video controller** that hides the splash once finished or on any key press. (`Assets/Game/IntroController.cs`)

### Enemy Spawning
A configurable spawner instantiates zombies around a center point, sampling the NavMesh for valid positions and optionally running timed waves with alive-count limits. (`Assets/Game/ZombieSpawner.cs`)

## 🗺️ Project Layout
- **Assets/AI/** — Core AI framework (perception, decision, movement, coordination, debug tooling).
- **Assets/Game/** — Player controls, UI hooks, noise utilities, spawners, and intro logic.
- **Assets/Scenes/** — Game scenes ready to load in the editor.
- **Assets/Prefabs/** — Prefabs for quick scene assembly (characters, props, interactables).
- **ProjectSettings/** — Unity project configuration targeting `6000.2.13f1`.

## 🚀 Getting Started
1. Open the project with Unity **6000.2.13f1** (or newer in the 6000.2 stream).
2. Import TextMesh Pro essentials if prompted.
3. Load the main scene under `Assets/Scenes/` and bake the NavMesh if needed.
4. Press **Play** to explore, lure zombies with left-click noises, and observe AI debug gizmos in the Scene view.

## 🎮 Default Controls
- **WASD** — Move
- **Mouse** — Look
- **Left Ctrl / C** — Crouch (quieter footsteps)
- **Left Click** — Emit lure noise at cursor hit point
- **Any key** — Skip intro video

## 📌 Future Ideas (will try implement, probably)
- Expand influence map usage for group tactics (e.g., swarming toward “hot” cells).
- Add melee/combat interactions and health UI.
- Experiment with utility-based scoring layered on top of the existing FSM.
- Package a short gameplay loop (objectives, win/lose) for a polished vertical slice.

