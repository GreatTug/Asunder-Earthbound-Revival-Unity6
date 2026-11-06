# Contributing to Asunder: Earthbound Revival

Thank you for your interest in contributing to the preservation and modernization of **Asunder: Earthbound**!

This project is an unofficial community effort dedicated to **digital preservation, historical accessibility, and open-source VR enhancement**.

---

## 🧭 Core Principles

1. **Preserve Narrative & Artistic Identity**:
   * We do not alter original dialogue lines, voice recordings, character designs, or plot progression.
   * Enhancements must focus on **fidelity, hardware compatibility, immersion, bug fixes, and audio/visual quality** without erasing the 1930s noir atmosphere.
2. **Strictly Non-Commercial**:
   * No monetization, advertisements, paid DLC, or paywalls.
   * All original assets remain the intellectual property of **Aldin Dynamics**.
3. **Clean, Modern Unity 6 Standards**:
   * Zero compiler errors and zero compiler warnings where reasonably possible.
   * No per-frame GC allocations in gameplay loops (`Update`, coroutines).
   * OpenXR-compliant input abstractions (no proprietary vendor SDK locks).

---

## 🛠️ Development Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/<your-repo>/Asunder-Earthbound-Unity6.git
   cd Asunder-Earthbound-Unity6
   ```
2. **Open in Unity Hub**:
   * Recommended Unity version: **`6000.6.6f1` (Unity 6 LTS)**.
   * Ensure the **Windows Build Support (IL2CPP / Mono)** module is installed.
3. **Open the Project**:
   * Add the cloned folder to Unity Hub and launch it.
   * Allow Unity to complete the initial shader compilation and asset indexing.

---

## 🧪 Testing & Verification Workflow

Before submitting a Pull Request, you must run the automated project audit:

### Method A: Via Unity Editor GUI
1. In the top menu bar, select:
   * **`Asunder` > `Run Full Audit (No Build)`**
2. Confirm the Console outputs `[AsunderAudit] All 13 scenes and assets passed with 0 errors!`.

### Method B: Via CLI Helper Script
From the project root in PowerShell:
```powershell
.\build.ps1 -Target Audit
```

To run a complete dual build test:
```powershell
.\build.ps1 -Target All
```

---

## 📦 Pull Request Checklist

When submitting a PR, please ensure:
- [ ] No files in `Library/`, `Builds/`, `Logs/`, or `UserSettings/` are tracked in git.
- [ ] Added or modified C# scripts compile without errors or obsolete warnings.
- [ ] Any coroutines avoid creating `new WaitForSeconds(...)` per tick; use `AsunderCoroutines`.
- [ ] All 13 scenes load cleanly in the audit tool.
- [ ] Commit messages follow the [Conventional Commits](https://www.conventionalcommits.org/) format (e.g. `fix(audio): ...`, `feat(vr): ...`, `docs: ...`).

---

## 💬 Questions & Support
Feel free to open an **Issue** or **Discussion** on GitHub for feedback, ideas, or questions!
