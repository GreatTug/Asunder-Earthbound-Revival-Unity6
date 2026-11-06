# Asunder: Earthbound — Scene Directory & Breakdown

All 13 scenes from the original 2013 game binary have been reconstructed, upgraded, and audited for Unity 6 and OpenXR with **0 missing scripts**, **0 broken renderers**, and **0 shader errors**.

---

## 🎬 Main Story Sequence

### 0. `Assets/SCENES/Main/Loading.unity` (Build Index: 0)
* **Purpose**: Application bootstrap, asset preloading, and mode resolution.
* **Key Components**:
  * Initializes the `AppManager` singleton.
  * Detects runtime mode: Flat Desktop PC vs. OpenXR VR.
  * Transitions smoothly to `IntroScene` with black screen fade.
* **GameObject Count**: 42

### 1. `Assets/SCENES/Main/IntroScene.unity` (Build Index: 1)
* **Purpose**: 1930s cinematic title sequence, opening jazz radio broadcast, and credit rollout.
* **Key Components**:
  * `IntroManager.cs`: Handles title billboard transitions and audio synchronization.
  * Ambient radio music (`Radio-Sandman.ogg`, `Bethena-Scott-Joplin.ogg`).
  * Seamless transition into aircraft boarding.
* **GameObject Count**: 169

### 2. `Assets/SCENES/Main/PreMainScene.unity` (Build Index: 2)
* **Purpose**: Boarding phase, seating the protagonist in the 1930s twin-engine aircraft.
* **Key Components**:
  * `PreStoryManager.cs`: Controls initial dialogue, newspaper headlines, and attendant greeting.
  * Seated player calibration and tracking origin centering.
* **GameObject Count**: 310

### 3. `Assets/SCENES/Main/MainScene.unity` (Build Index: 3)
* **Purpose**: The central core of the game—in-flight psychological horror and climax.
* **Key Components**:
  * `StoryManager.cs`: Main timeline driving the progressive escalation.
  * `EnvironmentManager.cs`: Dual-propeller rumble, turbulence tremors, and rain/lightning fx.
  * `PlayerManager.cs`: Seat kicking, peanut bowl grabbing/throwing, overhead reading light switch.
  * Passenger NPCs: Walter (across aisle), Lydia (adjacent row), Ed (armed passenger), Flight Attendant.
  * Climax: Emergency hatch blowout, decompression physics, and cabin chaos.
* **GameObject Count**: 3,242

### 4. `Assets/SCENES/Main/PostMainScene.unity` (Build Index: 4)
* **Purpose**: Aftermath of decompression, emergency descent through dense cloud banks.
* **Key Components**:
  * `PostStoryManager.cs`: Cabin door state, wind howl, damaged interior geometry.
* **GameObject Count**: 985

### 5. `Assets/SCENES/Main/EndScene.unity` (Build Index: 5)
* **Purpose**: Aircraft crash sequence and closing epilogue.
* **Key Components**:
  * Ground terrain impact sequence, final blackout, and credit sequence.
* **GameObject Count**: 2,830

---

## 🌟 Extras & Concept Scenes

Accessible from the Extras Menu or for standalone exploration:

### 6. `Assets/SCENES/Extras/ExtrasMenuScene.unity` (Build Index: 6)
* **Purpose**: Retro 1930s chapter and bonus scene selector.
* **Key Components**:
  * `ExtrasMenuManager.cs`: Menu interaction with VR controller aiming and haptic feedback.
* **GameObject Count**: 37

### 7. `Assets/SCENES/Extras/Extra1_WalterNightmare.unity` (Build Index: 7)
* **Purpose**: Surreal psychological nightmare vignette featuring Walter.
* **GameObject Count**: 1,877

### 8. `Assets/SCENES/Extras/Extra2_Fall.unity` (Build Index: 8)
* **Purpose**: High-altitude free-fall sequence plunging through atmospheric clouds.
* **GameObject Count**: 1,222

### 9. `Assets/SCENES/Extras/Extra3_Flyby.unity` (Build Index: 9)
* **Purpose**: Exterior cinematic flyby camera tracking the aircraft through a thunderstorm.
* **Key Components**:
  * `FlybyMoverII.cs`: Smooth exterior tracking camera controls.
* **GameObject Count**: 3,000

### 10. `Assets/SCENES/Extras/Extra4_Blood.unity` (Build Index: 10)
* **Purpose**: Hallucinatory horror vignette with blood lighting and distorted visuals.
* **GameObject Count**: 877

### 11. `Assets/SCENES/Extras/Extra5_Beach.unity` (Build Index: 11)
* **Purpose**: Atmospheric shoreline and deserted island wreckage sequence.
* **Key Components**:
  * Upgraded Unity 6 terrain with modern `TerrainLayer` assets.
* **GameObject Count**: 879

### 12. `Assets/SCENES/Extras/Extra6_City.unity` (Build Index: 12)
* **Purpose**: Vintage 1930s cityscape flyover showcasing period architecture.
* **GameObject Count**: 753
