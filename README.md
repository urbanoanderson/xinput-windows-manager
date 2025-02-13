[![MIT license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/urbanoanderson/xinput-windows-manager/blob/master/LICENSE)
[![Platform](https://img.shields.io/badge/platform-win--32%20%7C%20win--64-lightgrey)](https://www.microsoft.com/pt-br/software-download/windows10)


# Xinput Windows Manager

<img src="Images/icon.svg" width="25%">

## About

This system tray Windows application allows the user to toggle a desktop manager mode by pressing a special button combination on an Xinput gamepad such as the Xbox One Controller. In this mode, the gamepad is able to freely move the cursor around the screen, perform left and right click actions as well as some useful keyboard presses.

The software was developed so that users with a desktop setup with only gamepad input or at least inconvenient access to keyboard and mouse can perform basic tasks that require these input methods using a gamepad. An example would be my case, where I use a desktop PC with Steam Big Picture in my living room as a DIY game console.

<img src="Images/screenshot1.png" width="100%">

## Installation

1. Download the lastest release of this software in the [official github page](https://github.com/urbanoanderson/xinput-windows-manager/releases);
2. Extract the contents of the zip file. You can run the main executable file at anytime to start the app;

Optional - If you want the app to start with windows do the following:
1. Create a shortcut of `"XinputWindowsManager.exe"`
2. Place the shortcut inside the following folder: `"C:\Users\<YOURUSERNAME>\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup"`

## Usage

You can toggle manager mode by right-clicking the system tray icon and selecting the toggle option or by pressing the toggle button combination on your xinput controller (player one). Combination can be set in App.Config and it defaults to:
- `BACK` + `A` + `X`

While in mouse mode, the following actions can be performed:
- `Left analog stick`: controls mouse cursor movement;
- `A`: performs left click;
- `X`: performs right click;
- `Y`: opens Windows Task Manager (CTRL+SHIFT+ESC);
- `B`: presses ESC key;
- `LS`: opens OnScreen Keyboard;
- `RS`: sends mute command;
- `LT`: presses Windows key;
- `RT`: minimizes all apps (Win+D);
- `LB+RB`: performs performs ALT+TAB;
- `DPAD`: sends arrow keys from keyboard;
- `Right analog stick UP/DOWN`: controls system volume;
- `Right analog stick LEFT/RIGHT`: controls prev/next song;

Other settings can be changes on App.Config but default values should sufice on most cases.

## Release Notes

### 2.0.0

- Refactors codebase
- Upgrade to .NET 8
- Utilizes XInputium instead of SharpDX for Xinput control
- Utilizes InputSimulatorCore for Keyboard/Mouse control

### 1.0.0

- Initial version
- Enables mouse movement, left and right click actions

## Acknowledgements

<div><a href="https://www.flaticon.com/free-icon/laptop_1577244" title="Kiranshastry">Icon</a> made by Kiranshastry from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>