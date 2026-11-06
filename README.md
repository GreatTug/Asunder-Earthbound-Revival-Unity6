# Asunder: Earthbound — Unity 6 & OpenXR Revival

[![Unity Version](https://img.shields.io/badge/Unity-6000.6.6f1%20(Unity%206)-blue.svg?logo=unity)](https://unity.com/)
[![Platforms](https://img.shields.io/badge/Platforms-PC%20%7C%20PCVR%20%7C%20Meta%20Quest%203%20%7C%20Google%20Cardboard-success.svg?logo=android)](https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6/releases/tag/v1.0.0)
[![VR Runtime](https://img.shields.io/badge/VR%20Runtime-OpenXR%20%2B%20Cardboard%20Mobile-brightgreen.svg?logo=khronosgroup)](https://www.khronos.org/openxr/)
[![Build Status](https://img.shields.io/badge/Build-4%2F4%20Verified%20(0%20Errors)-brightgreen.svg)](#-ready-to-play-standalone-downloads-release-v100)
[![License](https://img.shields.io/badge/License-MIT%20%2B%20Aldin%20Dynamics%20Notice-orange.svg)](LICENSE)

> ### ⚠️ Cultural Preservation & Legal Disclaimer
> This is a **strictly non-commercial, non-profit community digital preservation project**.
>
> **Asunder: Earthbound** was originally developed and published by **[Aldin Dynamics](https://www.aldindynamics.com/)** in 2013 as a groundbreaking psychological suspense experience for the Oculus Rift Development Kit 1 (DK1). Due to the deprecation of legacy 32-bit Oculus runtimes (0.4–0.8) and older display servers, the original binary became completely unplayable on modern Windows 10/11 operating systems and contemporary VR headsets.
>
> This project reconstructs and modernizes the original game for **Unity 6 (6000.6.6f1)**, universal **OpenXR** standards, **Meta Quest 3 Standalone**, and **Google Cardboard Mobile VR** purely for historical preservation, cultural archiving, and educational study.
>
> *All original intellectual property, 3D character models, audio recordings, animations, and narrative elements remain the sole copyright of **Aldin Dynamics ehf.*** If you represent Aldin Dynamics and would like this repository adjusted or removed, please open an issue or reach out to the project maintainers.

---

## ✈️ About Asunder: Earthbound

Set aboard a 1930s luxury passenger airliner cruising through a turbulent, storm-swept night, *Asunder: Earthbound* places you in the passenger seat of an escalating airborne nightmare. Inspired by classic psychological horror and *The Twilight Zone*, the experience relies on tension, physical claustrophobia, and passenger interactions rather than jump scares.

---

## 🌟 Modernization & Technical Enhancements

This revival goes far beyond a simple recompile; every subsystem has been overhauled to meet next-generation VR, PC, and standalone mobile standards while preserving 100% of the original 2013 narrative and artistic vision:

### 1. Meta Quest 3 Native Standalone VR (`.apk`)
* **Pure Standalone 6DoF VR**: Built specifically for **Meta Quest 3**, **Quest 3S**, **Quest 2**, and **Quest Pro** without requiring a PC or link cable.
* **OpenXR Android Pipeline**: Integrated OpenXR runtime with **Meta Quest Touch Plus / Touch Pro / Touch** controller interaction profiles, physical gestures, and haptics.
* **High-Performance Architecture**: ARM64 64-bit architecture compiled with IL2CPP and Android GameActivity entry point.
* **Vulkan Graphics Engine**: Native Vulkan rendering pipeline with memoryless MSAA discard optimizations for maximum battery efficiency and thermal headroom.

### 2. Google Cardboard & Smartphone Mobile VR (`.apk`)
* **Split-Screen Stereo Rendering**: Custom 50/50 dual-viewport stereo camera system configured for any Android mobile VR headset or Google Cardboard viewer.
* **Hardware Gyroscope 3DoF Head Tracking**: Real-time reading of mobile phone gyroscope attitude (`Input.gyro.attitude`), mapped directly into landscape camera rotation.
* **Single-Tap Screen Interaction**: Single screen tap (or headset capacitive button) triggers character interaction, dialog progression, and peanut tossing.
* **Two-Finger Screen Recenter**: Double-finger touch tap immediately resets your forward tracking orientation to zero.

### 3. Universal PCVR OpenXR Compatibility
* **8 Native Interaction Profiles**: Plug-and-play support for **Meta Quest Touch / Touch Pro / Touch Plus**, **Valve Index Knuckles**, **HTC Vive Wands**, **HP Reverb G2**, **Microsoft Mixed Reality**, and **Khronos Simple Controller**.
* **Decoupled Architecture**: Fully separated VR OpenXR and Flat Desktop PC tracking pipelines—preventing background runtime conflicts when playing in desktop mode.

### 4. Dual-Hand Tactile Haptics (VR Controllers)
* **Turbulence Tremors**: Dynamic micro-vibrations (0.35–0.85 intensity) synced with aircraft hull shaking and lightning rumbles.
* **Firearms & Explosions**: 0.85–1.0 kickback impulse when Ed discharges his weapon, plus full haptic impact upon cabin decompression.
* **Physical World Interactions**: Tactile ticks when flicking reading lights, kicking the front seat, tossing peanut shells, or navigating menus.

### 5. Comfort & Locomotion
* **30° Snap Turning**: Smooth, instant 30° snap rotation mapped to the right thumbstick (and keyboard `Q` / `E`) with haptic feedback to prevent VR motion sickness while seated.
* **Instant Recenter**: Press controller menu buttons or keyboard `Space` / `R` at any time to re-zero your tracking center.

### 6. Acoustic & Audio Engineering
* **Zero Doppler Pitch Warping**: Disabled artificial Doppler shift (`Doppler Factor: 0`), preventing frequency flutter on voice lines during rapid head turns in VR.
* **Ultra-Low Latency DSP Buffer**: Reduced DSP buffer from legacy 1024 samples to **512 samples**, backed by **64 real voice channels** to eliminate audio dropping or voice stealing.
* **Acoustic Rolloff Restoration**: Corrected Lydia’s 2013 linear rolloff bug (which muted her voice at 2.38 meters) to a proper **Logarithmic Curve (1m–500m)**.
* **Script Typo & RNG Fixes**: Fixed `Random.Range` boundary truncation in `EnvironmentManager` and `StoryManager`, restoring the missing 3-second/8-second turbulence audio clips and peanut throw line #25.

### 7. Desktop PC Experience
* **Modern 75° Widescreen FOV**: Elevated PC camera FOV from 2013's narrow 4:3-era 60° to a comfortable **75°**, offering natural peripheral visibility of fellow passengers on 16:9 and 21:9 ultra-wide displays.
* **Smooth Mouse Look**: Smooth, clamped mouse rotation with cursor locking/toggling.

### 8. Visual Fidelity & Engine Pacing
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

### 🥽 VR Controllers (Meta Quest Touch / Index / Vive)
| Action | Input Binding |
|--------|---------------|
| **Aim / Interact** | Right Trigger / Gaze + Trigger |
| **Snap Turn (30°)** | Right Thumbstick Flick Left / Right |
| **Nod Head (Yes)** | Left Thumbstick Flick Up (or physical head nod) |
| **Shake Head (No)** | Left Thumbstick Flick Left / Right (or physical head shake) |
| **Recenter Origin** | Controller System / Menu Button (or Right Stick Click) |
| **Pause / Menu** | Left Controller Menu Button |

### 🖥️ Desktop PC (Keyboard & Mouse)
| Action | Input Binding |
|--------|---------------|
| **Look Around** | Mouse Move |
| **Interact / Select** | Left Mouse Button or `E` |
| **Snap Turn** | `Q` (Left 30°) / `E` (Right 30°) |
| **Recenter** | `R` or `Space` |
| **Fullscreen Toggle** | `F11` |
| **Quit Game** | `Escape` |

### 📱 Google Cardboard & Smartphone Mobile VR
| Action | Mobile Input Gesture |
|--------|----------------------|
| **Head Tracking (3DoF)** | Move head naturally (powered by phone internal gyroscope) |
| **Interact / Select** | Single screen tap (or Cardboard capacitive button) |
| **Recenter Heading** | Two-finger touch tap on screen |
| **Exit Game** | Android Back Button / System Navigation |

---

## 📥 Ready-to-Play Standalone Downloads (Release v1.0.0)

All 4 target platforms are pre-compiled, audited, verified, and available as direct downloads from [GitHub Releases](https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6/releases/tag/v1.0.0):

| Target Platform | Package Name | Format | Recommended For | Direct Download |
|:---|:---|:---:|:---|:---:|
| 🥽 **Meta Quest 3 Standalone** | `Asunder_Earthbound_Quest3_v1.0.0.apk` | `.apk` (192 MB) | Meta Quest 3, Quest 3S, Quest 2, Quest Pro standalone VR | [**Download Quest 3 APK**](https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6/releases/download/v1.0.0/Asunder_Earthbound_Quest3_v1.0.0.apk) |
| 📱 **Google Cardboard Mobile VR** | `Asunder_Earthbound_Cardboard_v1.0.0.apk` | `.apk` (188 MB) | Any Android smartphone + Cardboard / Daydream viewer | [**Download Cardboard APK**](https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6/releases/download/v1.0.0/Asunder_Earthbound_Cardboard_v1.0.0.apk) |
| 🥽 **PCVR OpenXR Standalone** | `Asunder_Earthbound_VR_OpenXR_v1.0.0.zip` | `.zip` (223 MB) | PC-tethered VR (Quest Link, Valve Index, HTC Vive, Reverb G2) | [**Download PCVR (.zip)**](https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6/releases/download/v1.0.0/Asunder_Earthbound_VR_OpenXR_v1.0.0.zip) |
| 🖥️ **PC Flat Desktop Standalone** | `Asunder_Earthbound_PC_Standalone_v1.0.0.zip` | `.zip` (223 MB) | Flat-screen desktop/laptop, 75° widescreen FOV, mouse look | [**Download PC (.zip)**](https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6/releases/download/v1.0.0/Asunder_Earthbound_PC_Standalone_v1.0.0.zip) |

---

## 📲 Android Installation & Sideloading Guide

### A. Meta Quest 3 / Quest 2 / Quest Pro
You can sideload `Asunder_Earthbound_Quest3_v1.0.0.apk` using any of the standard methods:

* **Method 1: SideQuest (Easiest)**
  1. Open [SideQuest](https://sidequestvr.com/) on your PC or Mac.
  2. Connect your Quest headset via USB-C cable (ensure Developer Mode is enabled).
  3. Drag and drop `Asunder_Earthbound_Quest3_v1.0.0.apk` onto the top bar of SideQuest.
  4. Wait for the green notification confirming installation.
  5. Put on your Quest: Navigate to **App Library** > Click the Search / Category Dropdown (top right) > Select **"Unknown Sources"** > Click **"Asunder: Earthbound"**.

* **Method 2: Meta Quest Developer Hub (MQDH)**
  1. Open Meta Quest Developer Hub.
  2. In the **Device Manager** tab, click **Install APK**.
  3. Select `Asunder_Earthbound_Quest3_v1.0.0.apk` and confirm.

* **Method 3: Direct ADB Command Line**
  ```bash
  adb install -r Asunder_Earthbound_Quest3_v1.0.0.apk
  ```

---

### B. Google Cardboard & Mobile Smartphone VR
1. Download `Asunder_Earthbound_Cardboard_v1.0.0.apk` onto your Android phone (or transfer via USB).
2. Tap the file in your file manager to install (enable *"Install from unknown sources"* if prompted).
3. Launch **Asunder: Earthbound (Cardboard VR)**.
4. Place your phone inside your Google Cardboard or mobile VR headset.
5. Look around naturally; single tap the screen or viewer button to interact, two-finger tap to recenter.

---

## 🛠️ Building From Source

### Prerequisites
1. **Unity Hub** with **Unity 6 (6000.6.6f1)** installed.
2. Windows Build Support (Mono / IL2CPP Standalone).
3. Android Build Support (with Android SDK, NDK r27c, and OpenJDK 17).

### Steps
1. Clone this repository:
   ```bash
   git clone https://github.com/GreatTug/Asunder-Earthbound-Revival-Unity6.git
   ```
2. Open **Unity Hub** -> **Add project from disk** -> Select `Asunder_Unity6`.
3. Once the project loads and imports, navigate to the top Unity Editor menu:
   - **`Asunder` > `Run All Builds and Audits`**: Audits all 13 scenes and compiles PC + PCVR executables.
   - **`Asunder` > `Build Quest 3 Standalone APK`**: Compiles standalone 6DoF OpenXR APK for Meta Quest 3.
   - **`Asunder` > `Build Cardboard Mobile VR APK`**: Compiles split-screen stereo + gyro APK for Cardboard.
   - **`Asunder` > `Build Both Android APKs (Quest 3 + Cardboard)`**: Compiles both Android builds sequentially.

---

## 📜 Credits, Dedications & Special Thanks

* **Original Game Creation, Design & Art**: **[Aldin Dynamics](https://www.aldindynamics.com/)** (2013) — Our deepest respect and gratitude to the original visionary team for crafting this pioneering 1930s airborne psychological horror experience.
* **Special Thanks & Discovery**: **LoverBatu (Asikbatu)** — Sincere thanks and appreciation for recommending this hidden VR classic and inspiring its revival, restoration, and digital preservation.
* **Revival, Unity 6 Porting, OpenXR & Android Integration**: GreatTug & Community Contributors (2026).
* **An Open Invitation to the Community**: This project belongs to the VR community. Anyone wishing to continue the project, polish interactions, add language translations, refine visuals, or optimize performance is warmly welcomed and encouraged to fork, improve, and submit pull requests!
* Dedicated to the pioneers of early consumer VR and the preservation of historic interactive fiction.
