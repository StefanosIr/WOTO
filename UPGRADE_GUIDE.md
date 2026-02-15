# WOTO Unity Modernization Guide

## Current Project Status

**Current Unity Version:** 5.1.2f1 (Released 2015)  
**Target Version:** Unity 2023 LTS or Unity 6 (2024)

---

## Pre-Upgrade Checklist

### 1. Backup Your Project
- [ ] Create a complete backup of the entire project folder
- [ ] Commit all changes to Git
- [ ] Consider creating a new branch for the upgrade

### 2. Clean Up Repository (Completed via GitHub)
- [x] Remove build files (beta1.exe, beta1_Data/)
- [x] Remove auto-generated project files (.csproj, .sln, .unityproj)
- [x] Remove user-specific files (.userprefs)
- [x] Add proper .gitignore
- [x] Update controller scripts for modern Unity API

---

## Upgrade Process

### Step 1: Install Unity Hub and Target Version

1. Download and install [Unity Hub](https://unity.com/download)
2. Install **Unity 2023 LTS** (Long Term Support) - Recommended for stability
   - Or install **Unity 6** for latest features
3. Install the following modules:
   - Windows Build Support (if not default)
   - Visual Studio Community (or your preferred IDE)

### Step 2: Open Project in New Unity Version

1. Open Unity Hub
2. Click "Add" and select your WOTO project folder
3. Select the newly installed Unity version from the dropdown
4. Click on the project to open it
5. **Unity will automatically:**
   - Convert project files
   - Upgrade scenes and prefabs
   - Update asset metadata
   - Regenerate Library folder

⚠️ **This process is irreversible! Make sure you have a backup.**

### Step 3: Address Upgrade Issues

#### Expected Issues and Fixes

**A. API Deprecations**
- ✅ Already fixed in controller scripts
- Most `Input.GetAxis()` calls are still valid but consider upgrading to New Input System

**B. Standard Assets**
- Old Standard Assets package may need to be removed and reimported
- Download compatible version from Unity Asset Store if needed

**C. Shaders and Materials**
- Built-in shaders should auto-upgrade
- Check all materials in `Assets/Materials/` folder
- May need to switch from Legacy shaders to Standard shaders

**D. Animation Controllers**
- Should upgrade automatically
- Test all character animations in `Assets/Animations/`
- Verify Animator components on player GameObjects

**E. Physics and Collision**
- Physics behavior may have changed slightly
- Test character movement and collision detection
- Adjust Rigidbody settings if needed

**F. Lighting**
- Consider switching to Progressive Lightmapper (GPU)
- Rebake all lighting in scenes
- Update skybox materials if needed

---

## Post-Upgrade Tasks

### 1. Test All Scenes
- [ ] mainmenu.unity - Main menu functionality
- [ ] stage 1.unity - Stage 1 gameplay
- [ ] All character controls (Player 1 & Player 2)
- [ ] Combat system (punching, kicking, jumping)
- [ ] Camera movement

### 2. Update Input System (Optional but Recommended)

**Current:** Using Legacy Input Manager  
**Recommended:** Unity's New Input System

**Benefits:**
- Better controller support
- Rebindable controls
- Multi-platform input handling
- Action-based input

**Migration Steps:**
1. Install "Input System" package via Package Manager
2. Create Input Actions asset
3. Refactor Controls1.cs and Controls2.cs to use new system
4. Set "Active Input Handling" to "Both" during transition

### 3. Optimize Project Settings

**Graphics:**
- Quality Settings → Update for modern hardware
- Consider using Universal Render Pipeline (URP) for better performance

**Player Settings:**
- Update company name and product name
- Set appropriate icons
- Configure build settings for target platforms

**Physics:**
- Verify physics timestep settings
- Check collision matrix settings

### 4. Update Build Configuration

- Remove old build outputs from repository (already done)
- Create new build folder (not in repository)
- Test build process on target platforms
- Consider creating automated build pipeline

---

## Issues Identified in Current Code

### Assets/Scripts/Controls1.cs ✅ FIXED
**Status:** Modernized and ready for upgrade
- No deprecated API calls
- Good code structure with constants
- Proper null checks
- Compatible with Unity 2023+

### Assets/Scripts/Controls2.cs ✅ FIXED  
**Status:** Modernized and ready for upgrade
- Code restructured to match Controls1.cs quality
- Improved readability
- Added safety checks
- Compatible with Unity 2023+

### Assets/scr.cs ⚠️ REMOVED
**Status:** Empty/unused file - scheduled for deletion

---

## Recommended Modern Unity Features

### 1. New Input System
- Rebindable controls
- Better gamepad support
- Action-based input architecture

### 2. Cinemachine
- Advanced camera system
- Replace manual camera movement scripts
- Smooth camera transitions

### 3. Post-Processing Stack
- Improve visual quality
- Add effects like bloom, color grading, ambient occlusion

### 4. TextMesh Pro
- Better text rendering
- Replace old UI Text components

### 5. Universal Render Pipeline (URP)
- Better performance
- Modern rendering features
- Improved mobile support

---

## Git Workflow Recommendation

```bash
# Create upgrade branch
git checkout -b unity-upgrade

# After upgrade in Unity
git add .
git commit -m "Upgrade project to Unity 2023 LTS"

# Test thoroughly, then merge
git checkout master
git merge unity-upgrade
```

---

## Troubleshooting Common Issues

### Issue: Project won't open
**Solution:** 
- Check Unity version compatibility
- Delete Library folder and let Unity regenerate
- Check console for specific errors

### Issue: Scripts show errors
**Solution:**
- Check for namespace changes
- Update using directives
- Refer to Unity API upgrade guide

### Issue: Animations broken
**Solution:**
- Check Animator Controller connections
- Verify animation parameter names
- Reimport animation assets

### Issue: Materials appear pink
**Solution:**
- Shaders not compatible with render pipeline
- Update materials to use Standard shader
- Consider switching to URP and upgrading materials

### Issue: Performance issues
**Solution:**
- Check Profiler for bottlenecks
- Update Quality Settings
- Optimize scene lighting

---

## Resources

- [Unity 2023 LTS Documentation](https://docs.unity3d.com/2023.2/Documentation/Manual/)
- [Unity Upgrade Guides](https://docs.unity3d.com/Manual/UpgradeGuides.html)
- [New Input System Documentation](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [URP Documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)

---

## Support

If you encounter issues during the upgrade:
1. Check Unity Console for specific error messages
2. Search Unity Forums and Unity Answers
3. Refer to this guide's troubleshooting section
4. Create an issue in this repository with details

---

**Last Updated:** February 2026  
**Created for:** WOTO (Wrath Of The Olympians) Unity Project Modernization
