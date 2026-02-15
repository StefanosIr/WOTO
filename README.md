# WOTO - Wrath Of The Olympians

## Final Year Project Deliverable

**Game Type:** 3D Fighting Game  
**Engine:** Unity  
**Status:** Modernized and ready for Unity upgrade

---

## 🎮 About

WOTO (Wrath Of The Olympians) is a 3D fighting game developed as a Final Year Project. The game features character combat mechanics, animations, and multiple stages.

---

## 📦 Current Status

### Version
- **Original:** Alpha (Unity 5.1.2f1 - 2015)
- **Current:** Modernized for Unity 2023 LTS / Unity 6

### Recent Updates (February 2026)

✅ **Repository Cleaned**
- Removed build files and auto-generated project files
- Added proper .gitignore for Unity projects

✅ **Scripts Modernized**
- Updated controller scripts with modern code practices
- Fixed deprecated API calls
- Improved code readability and maintainability

✅ **Documentation Added**
- [UPGRADE_GUIDE.md](UPGRADE_GUIDE.md) - Step-by-step Unity upgrade instructions
- [MODERNIZATION_REPORT.md](MODERNIZATION_REPORT.md) - Detailed analysis of changes

---

## 🚀 Getting Started

### For Development

1. **Clone the repository:**
   ```bash
   git clone https://github.com/StefanosIr/WOTO.git
   cd WOTO
   ```

2. **Open in Unity:**
   - Install [Unity Hub](https://unity.com/download)
   - Install Unity 2023 LTS (recommended) or Unity 6
   - Add the project folder in Unity Hub
   - Open the project (Unity will auto-upgrade from 5.1.2f1)

3. **First-time setup:**
   - Let Unity import and convert all assets (may take several minutes)
   - Check console for any warnings
   - Test both scenes: `mainmenu.unity` and `stage 1.unity`

### For Playing

**Note:** Build files are not included in the repository. To play:

1. Open the project in Unity (see above)
2. Go to `File > Build Settings`
3. Add scenes to build:
   - `Assets/mainmenu.unity`
   - `Assets/stage 1.unity`
4. Click `Build` and select output folder
5. Run the generated executable

---

## 🎯 Controls

### Player 1
- **Move:** WASD
- **Turn:** Q / E
- **Jump:** Space
- **Punch:** Left Mouse Button
- **Kick:** Right Mouse Button
- **Actions:** 1, 2, 3 keys

### Player 2
- **Move:** Arrow Keys
- **Turn:** [ / ]
- **Jump:** (Custom button - check Input Manager)
- **Punch:** (Numpad - check Input Manager)
- **Kick:** (Numpad - check Input Manager)
- **Actions:** ' (quote), ; (semicolon), 3

---

## 📁 Project Structure

```
WOTO/
├── Assets/
│   ├── Animations/          # Character animations
│   ├── Fonts/               # UI fonts
│   ├── Humanoid/            # Character models
│   ├── Materials/           # Material assets
│   ├── Scripts/             # C# gameplay scripts
│   │   ├── Controls1.cs     # Player 1 controller
│   │   ├── Controls2.cs     # Player 2 controller
│   │   ├── UIControl.cs     # UI management
│   │   └── cameramovement.cs # Camera controller
│   ├── SkyBoxes/            # Skybox textures
│   ├── Sounds/              # Audio files
│   ├── Standard Assets/     # Unity standard assets
│   ├── Textures/            # Texture assets
│   ├── mainmenu.unity       # Main menu scene
│   └── stage 1.unity        # Stage 1 game scene
├── ProjectSettings/         # Unity project configuration
├── .gitignore               # Git exclusions
├── README.md                # This file
├── UPGRADE_GUIDE.md         # Unity upgrade instructions
└── MODERNIZATION_REPORT.md  # Detailed modernization analysis
```

---

## 🔧 Technical Details

### Scripts

- **Controls1.cs** - Player 1 movement, combat, and action controller
- **Controls2.cs** - Player 2 movement, combat, and action controller
- **UIControl.cs** - UI and scene management
- **cameramovement.cs** - Camera follow logic

### Features

- Two-player local gameplay
- Character animation system
- Combat mechanics (punching, kicking)
- Character rotation and movement
- Action system with multiple states

---

## 📚 Documentation

### For Upgrading from Unity 5.1.2f1

See [UPGRADE_GUIDE.md](UPGRADE_GUIDE.md) for:
- Step-by-step upgrade process
- Common issues and solutions
- Post-upgrade checklist
- Recommendations for modern Unity features

### For Understanding Changes

See [MODERNIZATION_REPORT.md](MODERNIZATION_REPORT.md) for:
- Detailed analysis of code improvements
- List of deprecated API fixes
- Asset compatibility issues
- Testing procedures

---

## 🛠️ Development

### Requirements

- Unity 2023 LTS or Unity 6
- Visual Studio or compatible IDE
- Windows / macOS / Linux

### Building

1. Open project in Unity
2. `File > Build Settings`
3. Select target platform
4. Click `Build` or `Build and Run`

### Contributing

This is an archived academic project. For reference or educational purposes only.

---

## 📝 Notes

- This project was originally developed in 2015 with Unity 5.1.2f1
- Modernization completed in February 2026
- All scripts updated for compatibility with Unity 2023+
- No gameplay changes made during modernization
- Original game design and mechanics preserved

---

## 📄 License

Final Year Project - Educational purposes

---

## 👤 Author

Stefanos Irodotou

---

## 🔗 Related

- [Unity Documentation](https://docs.unity3d.com/)
- [Unity Hub](https://unity.com/download)
- [Unity Upgrade Guides](https://docs.unity3d.com/Manual/UpgradeGuides.html)

---

**Last Updated:** February 15, 2026
