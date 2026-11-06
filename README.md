# Asunder: Earthbound — Unity 6 & OpenXR Revival

[![Unity Version](https://img.shields.io/badge/Unity-6000.6.6f1%20(Unity%206)-blue.svg?logo=unity)](https://unity.com/)
[![Target Platform](https://img.shields.io/badge/Platform-Windows%2064--bit-success.svg?logo=windows)](https://microsoft.com)
[![VR Runtime](https://img.shields.io/badge/VR%20Runtime-OpenXR%201.14.0-brightgreen.svg?logo=khronosgroup)](https://www.khronos.org/openxr/)
[![Build Status](https://img.shields.io/badge/Build-Verified%20(0%20Errors)-brightgreen.svg)](#audit--verification)
[![License](https://img.shields.io/badge/License-MIT%20%2B%20Aldin%20Dynamics%20Notice-orange.svg)](LICENSE)

> ### ⚠️ Cultural Preservation & Legal Disclaimer
> This is a **strictly non-commercial, non-profit community digital preservation project**.
>
> **Asunder: Earthbound** was originally developed and published by **[Aldin Dynamics](https://www.aldindynamics.com/)** in 2013 as a groundbreaking psychological suspense experience for the Oculus Rift Development Kit 1 (DK1). Due to the deprecation of legacy 32-bit Oculus runtimes (0.4–0.8) and older display servers, the original binary became completely unplayable on modern Windows 10/11 operating systems and contemporary VR headsets.
>
> This project reconstructs and modernizes the original game for **Unity 6 (6000.6.6f1)** and universal **OpenXR** standards purely for historical preservation, cultural archiving, and educational study.
>
> *All original intellectual property, 3D character models, audio recordings, animations, and narrative elements remain the sole copyright of **Aldin Dynamics ehf.*** If you represent Aldin Dynamics and would like this repository adjusted or removed, please open an issue or reach out to the project maintainers.

---

## ✈️ About Asunder: Earthbound

Set aboard a 1930s luxury passenger airliner cruising through a turbulent, storm-swept night, *Asunder: Earthbound* places you in the passenger seat of an escalating airborne nightmare. Inspired by classic psychological horror and *The Twilight Zone*, the experience relies on tension, physical claustrophobia, and passenger interactions rather than jump scares.

---

## 🌟 Modernization & Technical Enhancements

This revival goes far beyond a simple recompile; every subsystem has been overhauled to meet next-generation VR and PC standards while preserving 100% of the original 2013 narrative and artistic vision:

### 1. Universal OpenXR Compatibility
* **8 Native Interaction Profiles**: Plug-and-play support for **Meta Quest Touch / Touch Pro / Touch Plus**, **Valve Index Knuckles**, **HTC Vive Wands**, **HP Reverb G2**, **Microsoft Mixed Reality**, and **Khronos Simple Controller**.
* **Decoupled Architecture**: Fully separated VR OpenXR and Flat Desktop PC tracking pipelines—preventing background runtime conflicts when playing in desktop mode.

### 2. Dual-Hand Tactile Haptics (VR Controllers)
* **Turbulence Tremors**: Dynamic micro-vibrations (0.35–0.85 intensity) synced with aircraft hull shaking and lightning rumbles.
* **Firearms & Explosions**: 0.85–1.0 kickback impulse when Ed discharges his weapon, plus full haptic impact upon cabin decompression.
* **Physical World Interactions**: Tactile ticks when flicking reading lights, kicking the front seat, tossing peanut shells, or navigating menus.

### 3. Comfort & Locomotion
* **30° Snap Turning**: Smooth, instant 30° snap rotation mapped to the right thumbstick (and keyboard `Q` / `E`) with haptic feedback to prevent VR motion sickness while seated.
* **Instant Recenter**: Press controller menu buttons or keyboard `Space` / `R` at any time to re-zero your tracking center.

### 4. Acoustic & Audio Engineering
* **Zero Doppler Pitch Warping**: Disabled artificial Doppler shift (`Doppler Factor: 0`), preventing frequency flutter on voice lines during rapid head turns in VR.
* **Ultra-Low Latency DSP Buffer**: Reduced DSP buffer from legacy 1024 samples to **512 samples**, backed by **64 real voice channels** to eliminate audio dropping or voice stealing.
* **Acoustic Rolloff Restoration**: Corrected Lydia’s 2013 linear rolloff bug (which muted her voice at 2.38 meters) to a proper **Logarithmic Curve (1m–500m)**.
* **Script Typo & RNG Fixes**: Fixed `Random.Range` boundary truncation in `EnvironmentManager` and `StoryManager`, restoring the missing 3-second/8-second turbulence audio clips and peanut throw line #25.

### 5. Desktop PC Experience
* **Modern 75° Widescreen FOV**: Elevated PC camera FOV from 2013's narrow 4:3-era 60° to a comfortable **75°**, offering natural peripheral visibility of fellow passengers on 16:9 and 21:9 ultra-wide displays.
* **Smooth Mouse Look**: Smooth, clamped mouse rotation with cursor locking/toggling.

### 6. Visual Fidelity & Engine Pacing
* **8x MSAA & Forced Anisotropic Filtering**: Pristine cabin edge anti-aliasing and razor-sharp textures.
* **High-Resolution Soft Shadows & Reflection Probes**: Realistic cabin lighting with 4-cascade shadow maps.
* **Zero GC Stutter**: Replaced dynamic coroutine allocations and TextMesh querying with cached, pre-allocated static routines (`AsunderCoroutines`).
* **Unthrottled Frame Pacing**: Native 90/120/144 Hz headset synchronization with zero frame jitter.

---

## 🎬 Included & Audited Scenes

All 13 original scenes have been verified with **0 missing scripts**, **0 broken renderers**, and **0 shader errors**:

| # | Scene Path | Description | GameObjects |
|---|------------|-------------|:-----------:|
| 0 | `Assets/SCENES/Main/Loading.unity` | Initial Bootstrap & Asset Preloader | 42 |
| 1 | `Assets/SCENES/Main/IntroScene.unity` | 1930s Title Sequence & Opening Theme | 169 |
| 2 | `Assets/SCENES/Main/PreMainScene.unity` | Cabin Boarding & Initial Narrative Setup | 310 |
| 3 | `Assets/SCENES/Main/MainScene.unity` | Main Flight, Storm, Passenger Drama & Climax | 3,242 |
| 4 | `Assets/SCENES/Main/PostMainScene.unity` | Post-Decompression Cabin Descent | 985 |
| 5 | `Assets/SCENES/Main/EndScene.unity` | Final Aircraft Crash & Epilogue Sequence | 2,830 |
| 6 | `Assets/SCENES/Extras/ExtrasMenuScene.unity` | Retro Extras Menu & Chapter Select | 37 |
| 7 | `Assets/SCENES/Extras/Extra1_WalterNightmare.unity` | Extra 1: Walter's Nightmare | 1,877 |
| 8 | `Assets/SCENES/Extras/Extra2_Fall.unity` | Extra 2: Free Fall Through Clouds | 1,222 |
| 9 | `Assets/SCENES/Extras/Extra3_Flyby.unity` | Extra 3: Aerial Flyby Exterior Camera | 3,000 |
| 10 | `Assets/SCENES/Extras/Extra4_Blood.unity` | Extra 4: Blood Hallucination | 877 |
| 11 | `Assets/SCENES/Extras/Extra5_Beach.unity` | Extra 5: Deserted Beach Shoreline | 879 |
| 12 | `Assets/SCENES/Extras/Extra6_City.unity` | Extra 6: 1930s Vintage City Skyline | 753 |

---

## 🎮 Controls

### VR Controllers (Oculus Touch / Index / Vive)
| Action | Input Binding |
|--------|---------------|
| **Aim / Interact** | Right Trigger / Gaze + Trigger |
| **Snap Turn (30°)** | Right Thumbstick Flick Left / Right |
| **Nod Head (Yes)** | Left Thumbstick Flick Up (or physical head nod) |
| **Shake Head (No)** | Left Thumbstick Flick Left / Right (or physical head shake) |
| **Recenter Origin** | Controller System / Menu Button (or Right Stick Click) |
| **Pause / Menu** | Left Controller Menu Button |

### Desktop PC (Keyboard & Mouse)
| Action | Input Binding |
|--------|---------------|
| **Look Around** | Mouse Move |
| **Interact / Select** | Left Mouse Button or `E` |
| **Snap Turn** | `Q` (Left 30°) / `E` (Right 30°) |
| **Recenter** | `R` or `Space` |
| **Fullscreen Toggle** | `F11` |
| **Quit Game** | `Escape` |

---

---

## 📥 Ready-to-Play Standalone Downloads (Release v1.0.0)

If you just want to experience the game without opening Unity, download the pre-compiled, verified standalone packages directly from [GitHub Releases](https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6/releases/tag/v1.0.0):

| Edition | Package | Recommended For | Direct Download |
|:---|:---|:---|:---:|
| 🖥️ **PC Standalone** | `Asunder_Earthbound_PC_Standalone_v1.0.0.zip` | Flat-screen desktop/laptop, 75° widescreen FOV, mouse look | [**Download PC (.zip)**](https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6/releases/download/v1.0.0/Asunder_Earthbound_PC_Standalone_v1.0.0.zip) |
| 🥽 **VR OpenXR Standalone** | `Asunder_Earthbound_VR_OpenXR_v1.0.0.zip` | Meta Quest 2/3/Pro, Valve Index, HTC Vive, Reverb G2 | [**Download VR (.zip)**](https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6/releases/download/v1.0.0/Asunder_Earthbound_VR_OpenXR_v1.0.0.zip) |

*Instructions: Extract the `.zip` to any folder on your computer and run `Asunder_Earthbound_PC.exe` or `Asunder_Earthbound_VR.exe`.*

## 🛠️ Building From Source

### Prerequisites
1. **Unity Hub** with **Unity 6 (6000.6.6f1)** installed.
2. Windows Build Support (IL2CPP / Mono Standalone).

### Steps
1. Clone this repository:
   ```bash
   git clone https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6.git
   ```
2. Open **Unity Hub** -> **Add project from disk** -> Select the project root folder.
3. Once the project loads and imports, navigate to the top Unity Editor menu:
   - **`Asunder` > `Run All Builds and Audits`**
4. The automated builder will:
   - Run a deep audit on all 13 scenes, 80 shaders, and 401 materials.
   - Configure OpenXR standalone features and shader stripping.
   - Compile both dedicated standalone executables into:
     - `Builds/PC/Asunder_Earthbound_PC.exe` (Dedicated Desktop Mode)
     - `Builds/VR/Asunder_Earthbound_VR.exe` (Dedicated OpenXR VR Mode)

---

## 📜 Credits, Dedications & Special Thanks

* **Original Game Creation, Design & Art**: **[Aldin Dynamics](https://www.aldindynamics.com/)** (2013) — Our deepest respect and gratitude to the original visionary team for crafting this pioneering 1930s airborne psychological horror experience.
* **Special Thanks & Discovery**: **LoverBatu (Asikbatu)** — Sincere thanks and appreciation for recommending this hidden VR classic and inspiring its revival, restoration, and digital preservation.
* **Revival, Unity 6 Porting & OpenXR Integration**: GreatTug & Community Contributors (2026).
* **An Open Invitation to the Community**: This project belongs to the VR community. Anyone wishing to continue the project, polish interactions, add language translations, refine visuals, or optimize performance is warmly welcomed and encouraged to fork, improve, and submit pull requests!
* Dedicated to the pioneers of early consumer VR and the preservation of historic interactive fiction.
