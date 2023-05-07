# UnchartedSector SpaceEngineers Stuff

## Mod Development

### Mod Environment Setup 

You will need to complete the following steps to configure your local environment for developing SpaceEngineers mods. 

1. Install Space Engineers - Mod SDK from Steam (available under Tools)
1. Install VSCode
1. Install VSCode C# Extension
1. Install VSCode XML Extension
1. Set an environment variable `MODSDK` pointing to the location where the MOD SDK was installed from Steam (available under "Local Files" in the Steam properties)

### Local Mod Installation

Assuming you have cloned this repository to a location outside of the SpaceEngineers mods folder (`%APPDATA\SpaceEngineers\Mods`), the you will need to setup a symbolic link to so that you can run the mod locally without having first publish it to Steam. To do that you will need to run the command below in PowerShell (as an Administrator).

```
New-Item -ItemType SymbolicLink -Target "<PATH_TO_REPO>\Mods\Upkeep\" -Path "$env:APPDATA\SpaceEngineers\Mods\Upkeep"
```

You will need to run this command for each mod you wish to make available to Space Engineers. Once the mods have been linked, you can then add them to a game in SpaceEngineers. Locally installed mods should will up in the top of the mod list with a house icon.