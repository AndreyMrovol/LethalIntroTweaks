# Changelog History
This list is reads from bottom to top, with latest versions first.

## v1.4.6
- Implemented new config option `bAutoStartGame` which will automatically pull the lever. You can find this in the **Miscellaneous** section.
- Addressed occasional (but harmless) errors being logged by editing the menu elements too early.
- Defaulted `bAutoSelectMode` to `OFF` so it's opt-in rather than opt-out.
- Improved compatibility with **LethalLevelLoader** by loading the scene asynchronously if found.
    > This is the shittiest "solution" and should be addressed further by LLL.

## v1.4.5
- Fixed issue where `bAutoSelectHost` would cause a black screen to be shown, softlocking the menu.
- Fixed issue where the **MoreCompany** cosmetics button could not be clicked when `bFixMenuCanvas` was false.
- Implemented new `bFixMoreCompany` config option to ease any further bugs with **MoreCompany**.
    - Defaults to **true**. Disable this if you do not care about the look of your main menu.

## v1.4.4
- Minor changes to Thunderstore metadata.

## v1.4.3
### Deleting your config file is recommended!
- Fixed wrong version showing by replacing `sVersionTextFormat` with `bAlwaysShortVersion`.
- Improved consistency of button alignment by matching their `sizeDelta` with the host button.
- All config options now use `ConfigEntry` to enable runtime editing with things like [ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager).
- Version text is now updated in realtime when changing related config options.
    > Essentially, you no longer need to restart. Use **LethalConfig** to instantly see your changes.
<p>

- Added new `fVersionTextOffset` adjust the Y (up/down) position of the version text.
  - You should use this option when the text appears out of place due to a multi-line or aspect ratio issue.
<p>

- Improved compatibility with **AdvancedCompany** by disabling the canvas fix regardless of `bFixMenuCanvas`.
    - This fixes the character being drawn as a black silhouette on the cosmetics page.

## v1.4.2
- Fixed header/logo disappearing when navigating menus.
- Fixed the **LethalConfig** button being below the Quit button.
- Button alignment behaves better with other mod buttons, as well as when `bRemoveCreditsButton` is true.
- Some silly mod devs patch `PlayFirstDayShipAnimation` to implement their code, IT will no longer exit this early. 
  - The speaker SFX is now stopped at the end of the `firstDayAnimation` enumerator instead.
- Removed some dead/commented code and unused ref being set.

## v1.4.1
- Support v47 by fixing the settings issue. See [this issue](https://github.com/Owen3H/IntroTweaks/issues/6).
- Improved compatibility with future **MoreCompany** versions by detecting it's loaded rather than finding it's canvas.
- Removed `CUSTOM_VERSION_TEXT_X` and `CUSTOM_VERSION_TEXT_Y`. It was annoying to use and cluttered the config.
- Button alignment should now play nicely with mods without explicit support.
- Fixed splash screens not being skipped when mod loading takes >10s.
- Fixed all the null reference errors. (yay)

## v1.4.0
**New Features**
- Skipped **Unity** and **Zeekerss** splash screens! (Configurable)
- Removed the main menu 'Credits' button. (Configurable)

**Mod Compatibility**
- Moved the **MoreCompany** header image upwards to avoid button overlapping.
- Moved MoreCompany activate and exit buttons both to the bottom right for intuitiveness.
- Improved compatibility with both LE and MC (the 3 config options from v1.3.1 are now all `true`).

**Misc**
- Buttons now get aligned without requiring `bFixMenuCanvas` to be `true` - including MC's "Mod Settings" button.
- Fixed issue where version text would stay hidden after exiting from a panel back to the menu.
- Removed unneeded reference to `InputSystem`.

## v1.3.1
- Config now has categories.
- Added 3 new config options to help alleviate mod incompatibilities.
    - `bAlignMenuButtons` - Defaults to **false**.
    - `bFixMenuCanvas` - Defaults to **false**.
    - `bFixMenuPanels` - Defaults to **true**.

## v1.3.0
**Main Changes**
- Centered all menu panels so the whitespace at the edges are equal. (OCD havers rejoice)
- Menu buttons are now aligned with each other and will overflow instead of wrap.
- All menu panels (host, server list, loading screen) now have the same scale, offsets and anchor. 
    > Essentially, this puts the "corners" in the same place across panels.
- Menu canvas now has the correct settings that a menu should have.
    - **Pixel Perfect**: true
    - **Render Mode**: Screen Space Overlay
    > This means changing brightness no longer affects menu elements.
    
<p><p>

- Added new config option `iGameStartupDisplay` to control which monitor the game is displayed on when starting (after the splash screen).
    > Any negative value will disable this setting.
    > Defaults to `0` - the main display.

**Fixes**
- Fixed issue where setting `sAutoSelectMode` to `OFF` would cause the brightness and mic screens to show even after the first game boot.
- Fixed a null reference exception when trying to clone the version text.

**Misc**
- Removing UI elements now sets them to inactive. Destroying them is bad practice.
- Added try-catch blocks in multiple places to make future debugging easier.

## v1.2.2
- The ship speaker 'first day' SFX can now be disabled via `bDisableFirstDaySFX`.
- Added config option `sVersionTextFormat` to display either the full game version or a shortened one.
- Added config option `fVersionTextSize` to control the font size of the version text.
- Fixed custom version text not showing - it was accidentally off by default (oops).

## v1.2.1
- Added config option to toggle the version text. Credit - RectangularObject.
- BepinExPack now specified as a dependency.
- New VCR-style icon.

## v1.2.0
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

## v1.1.0
- Menu 'OS boot' animation is now skipped. You can revert this via the config.
- Replaced menu version text - able to be customized.
- Fixed LAN warning not being removed when AutoSelectHost was false.
- Changed mod name from 'LC-IntroTweaks' to 'IntroTweaks' in PluginMetadata.

## v1.0.0
- Skips straight into the selected mode. (Online/LAN)
- Can automatically press the 'Host' button once in the menu.
- Removes the LAN warning - no need to hit 'Confirm' every time.
