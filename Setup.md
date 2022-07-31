# Haptic-Hand

This Readme is mainly used to explain how to setup the entire project to work.

## Single setup steps
The following steps only have to be executed once to setup the right parts of the environment

### Steam VR & the HTC Vive Pro
To install SteamVR, a Steam account is needed. Searching for SteamVR and installing it should take care of the install.
When working with a computer from HMI, the chance exists that SteamVR is already installed, in that case you should be able to skip this step.
However, the version of SteamVR you have might not be up to date.

To setup the HTC Vive, make sure that everything is plugged in correctly, and that the video cable is in the Video Card (or you _WILL_ have a bad time).
Also make sure that the lighthouses show green lights, and everything needed is detected in SteamVR.

### Unity
First of all, the right version of Unity should be downloaded to edit and run the project.
This version is `2019.4.20f1`, in which at least the Major.Minor version should be the same.
This 2019.4 is the Long Term Support (LTS) version.
The installing of different versions on a PC can best be handled through the Unity Hub software.

When installed, you should be able to open the project with the right version.

### Feather ESP32
To be able to write to the Feather ESP32 wireless board.
First make sure a 8.0+ version of the Arduino app is installed.

Then follow the instructions found [here](https://github.com/espressif/arduino-esp32/blob/master/docs/arduino-ide/boards_manager.md) to add the ESP32 boards.

The arduino code is in `Haptic Hand\Assets\Scripts\Arduino-code\Feather_wireless\Feather_wireless.ino`, in which the SSID and the password of the router can be edited if needed.
The JSON decompilation can also be edited, but should work with this project as is.

#### Firewall
To make sure the packages can be picked up, the firewall should be slightly edited.
My personal choice is to turn it off and on every time I need to use the port, but other setups can also work.

**For windows**: go to the start menu and search `Firewall`, open the window, click on to `Advanced Firewall Settings`, then `Inbound Rules`.
Following this, in the top right, press `New Rule...`. This should open a new window;
Next a few steps are given with names, we will refer to the names here:
Rule Type step: select `Port` then `Next >`.
Protocol and Ports Step: select `TCP`, then `Specific local ports`, then `Next >`.
Action Step: Select `Allow the connection`, then `Next >`.
Profile Step: Select all, then `Next >`.
Name Step: Give a descriptive name an a possible description. You will need to remember this as this is used for actually enabling package accepting by your computer!

When your Firewall rule is enabled, it should let the feather communicate with the system. If not in use, it is recommended to turn this off.
You can check if the rule is working by watching the Arduino Serial Monitor with the Feather Plugged in (and setup correctly), baud rate 115200.

### Dexmo
To setup the Dexmo, you will need the Release_Dexta folder. This folder should have 4 sub-folders.
For a first setup, only the folder `1.Drivers` is required.
For this, install all the parts once. It is recommended to go for the x64 if available, as this would install the 64bit version.

## Normal usage after first Install
After having everything setup and running, we can now ignore most parts. Sadly, adding the Vive tracker (the puck like object) needs to be done every time if it isn't in the list of connected devices SteamVR shows.
This puck is needed for following the Dexmo.

### SteamVR & HTC Vive
If the setup was done correctly, you should start SteamVR and power up the Vive (there is a small button on the Vive Pro box). 
If everything is done correctly, the SteamVR window should pop up and say 'Standing by'/show nothing/Now Playing <game name>. 
This message means everything went correctly. 
You should see either a big empty space or the SteamVR Home environment.
You can also see this if needed by right clicking SteamVR and then selecting 'Display VR View'.

In any other case, google what might be causing the issue.

### Unity
Editing/running with editor: Start Unity hub and import the project.
It will take some time before Unity is ready, so you can grab a cup of coffee now if needed.
If you want to run the 'final experience', run the **<fill this in>**

### Feather
To run the feather, connect it to a power source.
A powerbank connected to the micro-b connector is recommended, as this gives enough juice and puts the logic ports on 5V.

*Due to limitations in the feathers code, you will need to pres the reset button every time you run the unity program, otherwise it won't connect.*

Pin - channel combinations:
13  -  1
12  -  2
27  -  3
33  -  4

### Dexmo
The Dexmo is a bit of a tricky device to get working as expected.
Only the Right hand Dexmo glove is being used in this program.
Fist, the tracking device (a Vive Tracker in the current code), should be power on.
Due to some unresolved issue, the channel can change every power up. 
This can be changed in the Unity Editor code if needed to a different channel. (`[Camerarig] -> Right Hand Tracker: in the inspector -> Steam VR_Tracked Object (Script) -> Index`)

The easiest way to figure this out is by running the program and changing the Index. 
If you keep moving the Tracker, it should at some point pop up and be tracked. 
It might be easier to already have the Dexmo connected as well. See below how.

To start the Dexmo, run the `ConsoleApplicationLibdexmoPlayground` in the base folder, this is shortcut to the actual run-file which is hidden a bit deeper down.
In this, you should see a list of the Dongles connected. (e.g.: `COM7 [HEX Code]`). 
The moment it finds a Search Result (a repeating message), you can press S. Press S again to start the Dexmo server.

If Unity Editor cannot connect to this server, a repeating message of 'Searching for the dexmo server!' is going to pop up.


