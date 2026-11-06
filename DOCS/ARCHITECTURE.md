# Asunder: Earthbound — Technical Architecture & Engine Blueprint

This document details the software architecture, runtime flow, input handling, and subsystem designs for the **Asunder: Earthbound** Unity 6 and OpenXR revival. It is intended for contributors, developers, and researchers wishing to understand, extend, or maintain the project.

---

## 1. High-Level Engine Overview

* **Engine Version**: Unity 6 LTS (`6000.6.6f1`)
* **Graphics Pipeline**: Built-in Forward Rendering (optimized for multi-pass OpenXR stereo rendering and low latency)
* **VR Integration**: OpenXR Plugin (`1.14.0`) via Unity XR Management
* **Target OS**: Windows 64-bit (DirectX 11 / DirectX 12)
* **Audio Engine**: Unity Audio Foundation with low-latency DSP (512 samples) and zero Doppler distortion

```
                                  [ Loading.unity ]
                                          │
                                   (AppManager Boot)
                                          │
                                 [ IntroScene.unity ]
                                          │
                                [ PreMainScene.unity ]
                                          │
                                  [ MainScene.unity ]
                     (Flight, Storm, Interactions, Decompression)
                                          │
                                [ PostMainScene.unity ]
                                          │
                                  [ EndScene.unity ]
                                  (Crash & Epilogue)
```

---

## 2. Core Managers & Lifecycles

### `AppManager` (`Assets/Scripts/Assembly-CSharp/AppManager.cs`)
* **Role**: The persistent application singleton (`DontDestroyOnLoad`).
* **Responsibilities**:
  * Decouples **PC Desktop** and **VR OpenXR** execution modes.
  * Camera arbitration (`InitateCameras`):
    * In **PC mode**: Activates `CameraNormal`, stops VR subsystems, decouples mouse look, and enforces a comfortable 75° widescreen FOV.
    * In **VR mode**: Activates `CameraOVR` (left/right stereo eye rig), starts OpenXR tracking, and assigns the primary `AudioListener` to `CameraRight`.
  * Enforces graphics quality overrides: 8x MSAA, anisotropic filtering, 4-cascade shadows, soft particles, and unthrottled frame pacing (`targetFrameRate = -1`).
  * Story checkpoint persistence and audio source pausing on state shifts.

### `PlayerManager` (`Assets/Scripts/Assembly-CSharp/PlayerManager.cs`)
* **Role**: Controls the seated protagonist's physical presence, gaze, and hand interactions.
* **Aiming & Raycasting**:
  * Employs `AsunderInput.AimSphereCastAll(camTr, radius)` to detect colliders tagged `"Interactive"` or bearing `AppButton`.
  * Supports dual-modality aiming:
    * In **VR mode**: Aiming ray originates from the active controller pointer or head center eye.
    * In **PC mode**: Aiming ray follows the center of the viewport driven by `MouseLook`.
* **Physical Hand IK & Feedback**:
  * Looking at an interactive item (e.g., peanut bowl, reading light switch) automatically directs `_HandController` to transition from resting to reaching (`HandState.reachFor...`).
  * Interacting triggers haptic ticks on VR controllers and executes the assigned narrative action.

### `OVRDevice` & `AsunderInput` (`Assets/Scripts/Assembly-CSharp/OVRDevice.cs`)
* **Role**: The unified hardware abstraction layer.
* **OpenXR Integration**:
  * Replaces legacy Oculus 0.4–0.8 native C++ binaries with Unity's modern OpenXR subsystem (`InputDevices` and `InputFeatureUsage`).
  * Supports 8 native hardware interaction profiles: Meta Quest Touch (all generations), Valve Index Knuckles, HTC Vive Wands, HP Reverb G2, and Windows Mixed Reality.
* **Snap Turning & Recenter**:
  * Flicking the right thumbstick left/right (or pressing `Q`/`E` on PC) rotates the player orientation in **30° increments** with tactile haptic confirmation.
  * Pressing controller system buttons or `Space`/`R` resets tracking origin (`TryRecenter()`).

---

## 3. Story & Environment Systems

### `StoryManager` (`Assets/Scripts/Assembly-CSharp/StoryManager.cs`)
* **Role**: State machine orchestrating the central flight sequence in `MainScene.unity`.
* **State Sequence**:
  1. `introCabin`: Ambient cabin hum, passengers settling in, Walter reading paper.
  2. `firstSighting`: Flickering reading lights, eerie audio cue, shadowy shape visible outside window.
  3. `panicBuild`: Cabin turbulence escalation, Lydia’s anxiety dialogues, Ed’s aggression.
  4. `climaxBlowout`: Cabin emergency hatch failure, PhysX dynamic detachment, explosive decompression suck-out.

### `EnvironmentManager` (`Assets/Scripts/Assembly-CSharp/EnvironmentManager.cs`)
* **Role**: Controls weather, turbulence, aircraft vibrations, and engine sounds.
* **Turbulence Engine**:
  * Periodically triggers aircraft roll/pitch shakes (`turbulence-3sec`, `turbulence-8sec`).
  * Synchronizes twin-propeller rumble (`DC3_engine-looped-CLEAN.ogg`) with controller haptics.
* **Lightning & Storm VFX**:
  * Triggers randomized lightning illumination synchronized with stereo thunderclaps.

---

## 4. Audio Pipeline & Spatialization

* **DSP Buffer**: Configured to **512 samples** (`AudioManager.asset`) to guarantee sub-15ms acoustic response.
* **Doppler Factor**: Set to **0** (`Doppler Factor: 0`), preventing artificial frequency flutter when rotating the head rapidly in VR.
* **Voice Allocation**: Expanded to **64 real voice channels** (up from 32) to prevent voice stealing during heavy storm and turbulence sequences.
* **Distance Attenuation**:
  * All 3D cabin voices (Lydia, Walter, Ed, Attendant) use **Logarithmic Rolloff (1m min – 500m max)**.
  * Fixed original 2013 linear rolloff bug where Lydia's voice faded to 0 at 2.38 meters.

---

## 5. Coroutine & Memory Architecture

To completely eliminate garbage collection (GC) micro-stutters during VR gameplay:
* **`AsunderCoroutines` (`Assets/Scripts/Assembly-CSharp/AsunderCoroutines.cs`)**:
  * Pre-allocates and caches static `WaitForSeconds` instances (`QuarterSecond`, `HalfSecond`, `OneSecond`, `FsmWait`).
  * Eliminates runtime heap allocations in high-frequency coroutines across `StoryManager`, `PlayerManager`, and particle effect spawners.
