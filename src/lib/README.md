# Game Dependencies

This folder contains proprietary game dependencies required to build the mod. These files are **NOT included in the repository** for legal reasons.

## Required Files

You must copy these files from your Sirocco game installation:

### From `<GAME_PATH>\MelonLoader\`:
- `MelonLoader.dll`
- `0Harmony.dll`
- `Il2CppInterop.Runtime.dll`
- `Il2CppInterop.Common.dll`
- `Il2Cppmscorlib.dll`

### From `<GAME_PATH>\Sirocco_Data\Managed\`:
- `UnityEngine.CoreModule.dll`
- `UnityEngine.IMGUIModule.dll`
- `UnityEngine.InputLegacyModule.dll`
- `UnityEngine.TextRenderingModule.dll`
- `UnityEngine.UI.dll`
- `Unity.TextMeshPro.dll`

## How to Populate This Folder

Run this PowerShell script (adjust the game path as needed):

```powershell
$gamePath = "D:\Spell\Installed\Steam\steamapps\common\Sirocco"

# Copy MelonLoader dependencies
Copy-Item "$gamePath\MelonLoader\MelonLoader.dll" src\lib\
Copy-Item "$gamePath\MelonLoader\0Harmony.dll" src\lib\
Copy-Item "$gamePath\MelonLoader\Il2CppInterop.Runtime.dll" src\lib\
Copy-Item "$gamePath\MelonLoader\Il2CppInterop.Common.dll" src\lib\
Copy-Item "$gamePath\MelonLoader\Il2Cppmscorlib.dll" src\lib\

# Copy Unity assemblies
Copy-Item "$gamePath\Sirocco_Data\Managed\UnityEngine.CoreModule.dll" src\lib\
Copy-Item "$gamePath\Sirocco_Data\Managed\UnityEngine.IMGUIModule.dll" src\lib\
Copy-Item "$gamePath\Sirocco_Data\Managed\UnityEngine.InputLegacyModule.dll" src\lib\
Copy-Item "$gamePath\Sirocco_Data\Managed\UnityEngine.TextRenderingModule.dll" src\lib\
Copy-Item "$gamePath\Sirocco_Data\Managed\UnityEngine.UI.dll" src\lib\
Copy-Item "$gamePath\Sirocco_Data\Managed\Unity.TextMeshPro.dll" src\lib\
```

## Legal Notice

These files are proprietary and copyrighted by their respective owners:
- **MelonLoader**: Apache License 2.0
- **Unity Engine**: Proprietary (Unity Technologies)
- **Game Assemblies**: Proprietary (Game Developer)

**Do NOT commit these files to git.** They are already listed in `.gitignore`.
