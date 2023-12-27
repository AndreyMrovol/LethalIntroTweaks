# IntroTweaks
A configurable, quality of life mod for Lethal Company intro/menu screens.

## Installation
1. Install [BepInEx](https://github.com/BepInEx/BepInEx/releases) v5 into your game.
2. Download `IntroTweaks.dll` and drop it into `Lethal Company\BepInEx\plugins`.

## Goals
- Skip splash screen (zeekers, not Unity)

## Changelog History
### v1.0.0
- Skips straight into the selected mode. (Online/LAN)
- Can automatically press the 'Host' button once in the menu.
- Removes the LAN warning - no need to hit 'Confirm' every time.

### v1.1.0
- Menu 'OS boot' animation is now skipped. You can revert this via the config.
- Replaced menu version text - able to be customized.
- Fixed LAN warning not being removed when AutoSelectHost was false.
- Changed mod name from 'LC-IntroTweaks' to 'IntroTweaks' in PluginMetadata.

### v1.2.0
**General**
- Launch skipping is now more seamless - the greenish "transition" panel has been disabled.
- Stopped panels overlapping when pressing the 'Host' button. (how other buttons work already)
- Added ability to remove the 'News' panel. Defaults to Off.
- Added ability to remove the 'Launched in LAN mode' text. Defaults to On.
- Added config option for removing the LAN warning. Defaults to On.
- Config option `bAutoSelectHost` is now Off by default.

**Version Text**
- Fixed version text being wrapped.
- Added config options (X, Y) to move the position of the version text. 
- Setting the anchor position of version text is now done once instead of every frame.
- Version text now ONLY shows on the main menu. Not settings, credits etc.

### v1.2.1
- Added config option to toggle the version text. Credit - RectangularObject.
- BepinExPack now specified as a dependency.
- New VCR-style icon.

### v1.2.2
- The ship speaker 'first day' SFX can now be disabled via `bDisableFirstDaySFX`.
- Added config option `sVersionTextFormat` to display either the full game version or a shortened one.
- Added config option `fVersionTextSize` to control the font size of the version text.
- Fixed custom version text not showing - it was accidentally off by default (oops).

### v1.3.0
- Centered the menu panel so the whitespace at the edges are equal. (OCD havers rejoice)
- Menu buttons are now aligned with each other and will overflow instead of wrap.
- Menu canvas now has the correct settings that a menu should have.
    - **Pixel Perfect**: true
    - **Render Mode**: Screen Space Overlay
    > This means changing brightness no longer affects menu elements.
    
<p><p>

- Added new config option `iGameStartupDisplay` to control which monitor the game is displayed on when starting (after the splash screen).
    > Any negative value will disable this setting.
    > Defaults to `0` - the main display.
- Removing UI elements now sets them to inactive. Destroying them is bad practice.
- Added try-catch blocks in multiple places to make future debugging easier.
- Fixed a null reference exception when trying to clone the version text.