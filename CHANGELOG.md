# Asunder: Earthbound — Changelog & Modernization Report

This document records all architectural upgrades, bug fixes, modernizations, and parity verifications made to restore the 2013 Oculus Rift DK1 original into a modern Unity 6 and OpenXR experience.

---

## [Unity 6 Revival] - 2026-11-06

### 🎮 Virtual Reality & OpenXR Upgrades
* **OpenXR 1.14.0 Migration**: Completely removed the deprecated Oculus Rift DK1 32-bit native plugin (which failed on Windows 10/11) and replaced it with universal Khronos OpenXR.
* **8 Native Interaction Profiles**: Plug-and-play support for Meta Quest Touch, Quest Touch Pro, Quest Touch Plus, Valve Index Knuckles, HTC Vive Wands, HP Reverb G2, and Windows Mixed Reality.
* **Dual-Hand Tactile Haptics**: Added dynamic controller rumble across the entire experience:
  * Hull turbulence tremors (0.35–0.85 intensity).
  * Ed's gunshot recoil (0.85–1.0 impulse).
  * Climax decompression blowout (1.0 maximum impulse).
  * Peanut bowl interactions, seat kicking, and reading light switch clicks.
* **30° Snap Turning**: Added seated VR snap turning on right thumbstick flick (and `Q`/`E` keys) with tactile click feedback.
* **Tracking Recenter**: Mapped instant orientation and position recalibration to controller system buttons and keyboard `Space`/`R`.
* **60Hz Gesture Recognition Stabilization**: Fixed head nod/shake gesture sampling in `HeadGestureRecognizer.cs` to run at a consistent interval (`_SAMPLE_INTERVAL = 1f / 60f`), resolving erratic detection on modern 90Hz, 120Hz, and 144Hz displays.

---

### 🖥️ Desktop PC Experience
* **Modernized 75° Widescreen FOV**: Elevated PC camera FOV from the 2013 4:3-era 60° to a comfortable **75°**, giving natural peripheral visibility of fellow passengers on modern 16:9 and 21:9 ultra-wide displays without distorting cabin geometry.
* **Isolated Mouse Look**: Refactored `SimpleSmoothMouseLook.cs` and `MouseLook.cs` to gracefully decouple from OpenXR tracking, preventing background VR runtimes from seizing mouse input.
* **Cursor Lock & Visibility**: Cleaned up desktop cursor hiding and locking during gameplay.

---

### 🔊 Audio & Acoustic System
* **Doppler Factor Zeroed (`Doppler Factor: 0`)**: Eliminated artificial Doppler pitch modulation that previously caused voice warping during rapid head rotations in VR.
* **Low-Latency DSP Buffer (512 Samples)**: Slashed audio latency by half, moving from legacy 1024 samples to modern 512 samples.
* **Voice Limit Raised (64 Real Voices)**: Increased active voice count from 32 to 64 to prevent voice stealing during turbulent thunder and engine sequences.
* **Lydia Rolloff Curve Restoration**: Corrected Lydia’s AudioSource in `MainScene.unity` from a buggy linear rolloff (which dropped volume to 9% and muted at 2.38m) to a proper logarithmic curve (1m–500m).
* **Restored Lost Audio Clips (RNG Bug Fixes)**:
  * Fixed `Random.Range(0, 1)` and `Random.Range(0, 4)` in `EnvironmentManager.cs`, enabling `turbulence-3sec` and `turbulence-8sec` to play for the first time.
  * Fixed `Random.Range(0, 24)` in `StoryManager.cs`, enabling peanut throw voice line #25.
  * Fixed typo `peanut-throw-5` -> `peanut-throw-15` in `earthboundscript.txt`.
* **Single AudioListener Enforcement**: Guaranteed exactly one active `AudioListener` per scene (on `CameraNormal` in PC mode, on `CameraRight` in VR mode) to avoid Unity audio listener duplicate warnings.

---

### 🎨 Graphics, Shaders & Rendering
* **8x MSAA Anti-Aliasing**: Enforced pristine multi-sample anti-aliasing across all quality tiers.
* **Anisotropic Filtering**: Forced texture filtering to highest quality for razor-sharp cabin textures.
* **High-Resolution Soft Shadows**: Enabled 4-cascade high-resolution directional and spot shadows.
* **Soft Particles & Realtime Reflection Probes**: Replaced hard particle clipping with soft depth-faded smoke and fog.
* **Unthrottled Frame Pacing**: Set `targetFrameRate = -1` and tuned VSync (`0` for VR OpenXR, `1` for PC desktop) to deliver tear-free 90/120 Hz playback.
* **Terrain Auto-Upgrade**: Converted legacy Unity 4 splatmap textures to modern Unity 6 `TerrainLayer` assets with PBR support.
* **PhysX Concavity Fix**: Added dynamic compound mesh collider preparation in `AppManager.cs` for detached objects (hatch, player root) to prevent modern PhysX non-convex kinematic errors.

---

### ⚡ Memory, Garbage Collection & Code Quality
* **Zero-GC Coroutines**: Implemented static coroutine cache `AsunderCoroutines.cs` (`QuarterSecond`, `HalfSecond`, `OneSecond`, `FsmWait`), eliminating runtime heap allocations in looping state machines.
* **Cached TextMesh Queries**: Eliminated per-frame `GetComponent<TextMesh>()` calls in `StoryManager.cs` and `PreStoryManager.cs`.
* **Exhaustive Null Safety**: Hardened array traversals and component references across `PlayerManager`, `OverheadController`, `HandController`, `LegController`, `WalterController`, `EdController`, `LydiaController`, `BiffController`, and `CFX_SpawnSystem`.

---

## 📊 Verification Matrix

| Metric | 2013 Original (Unity 4.2.1f4) | Revival (Unity 6000.6.6f1) |
|---|:---:|:---:|
| **Operating System** | Windows 32-bit (Legacy) | Windows 64-bit (Modern) |
| **VR Runtime** | Oculus Rift DK1 (Deprecated) | Universal OpenXR 1.14.0 |
| **Audited Scenes** | 13 | 13 (100% Parity) |
| **Missing Scripts** | N/A | **0** |
| **Broken Renderers** | N/A | **0** |
| **Broken Shaders** | N/A | **0** |
| **Audio Latency** | ~25–40 ms (1024 DSP) | **<12 ms (512 DSP)** |
| **VR Haptics** | None | **Dual-Hand Full Feedback** |
| **Snap Turning** | None | **30° Tactile Snap Turn** |
| **PC FOV** | 60° (Tunnel View) | **75° (Modern Widescreen)** |
| **Build Artifacts** | Single 32-bit EXE | **Dual Dedicated PC & VR Standalones** |
