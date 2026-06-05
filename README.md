# Stardew_Mod_RemixFinder
This is a mod skeleton of a Seedfinder for the game Stardew Valley to find the desired remix bundle configuration.
Stardew_Mod_RemixFinder is designed to be created as it's own mod and runs in the game itself.

## Why seed finding as a mod?
To get the bundles generated correctly. With RNG involved it's important that calls are made in the exact order and amount.<br>
That's guaranteed if the the game's own logic and data handling is used.<br>
As a side effect this mod might be able to handle content mods if they only change the raw data used for remixed bundles.<br>

## How to use?
### If you don't know much about programming or creating a mod: <br>
  It should be simple enough to understand what to do. <br>
### If you do know a lot about programming or creating a mod: <br>
  I'm sorry, I don't know much about programming nor creating a mod.
  Maybe you can clone this repository to make it work?

### Step 0 - Requirements:
  1. install Stardew Valley
  2. install SMAPI. https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started#Install_SMAPI <br>
    SMAPI is the thing that will load your mod into the game.
  3. install an IDE (integrated development environment)
    which you'll use to edit and compile your mod code <br>
    To create this I used Windows Virtual Stuido Community (it's free) https://visualstudio.microsoft.com/de/vs/community/
  4. install the .NET 6 SDK. https://dotnet.microsoft.com/en-us/download/dotnet/6.0
    You need .NET 6 because it's the version used by Stardew. (yes it is end of life)

### Step 1 - Create the project - Setup
  1. open your IDE
  2. Create a solution with a C# Class Library project  (Don't select Class Library (.NET Framework)! That's a separate thing with a similar name.)
  3. Use "Stardew_Mod_RemixFinder" as the project name to avoid naming/reference problems
  4. Target .Net 6.0 (you may need to install and change it later, see below)
  5. Reference the Pathoschild.Stardew.ModBuildConfig NuGet package (see Stardew Wiki how to add the package) https://stardewvalleywiki.com/Modding:IDE_reference#In_Visual_Studio_2019.2F2022

### Step 2 - Create the project - copy the code
  1. delete Class1.cs
  2. add a C# class file called "ModEntry.cs" and a .json file called "manifest.json" <br>
    2.1 open Project Explorer (View -> Project Explorer or press shortcut Ctrl+Alt+L) <br>
    2.2 right-click "Stardew_Mod_RemixFinder" -> Add > new element (around the center of the menu) <br>
  3. copy the code from the files of the same name in this repository into them
  4. try to build the project 
    4.1 Shortcut Ctrl+B, or Menu Create -> create Stardew_Mod_Remixfinder <br>
  5. if successful without raising an error and SMAPI was installed correctly you should now find the mod in your mod folder <br>
     5.1 Folder location if you have it through steam: right-click Stardew Valley -> Browse Local Files <br>
     5.2 if not open windows search bar and type %appdata% to look into the Roaming folder <br>
     5.3 if it's not there either I don't know <br>

### Step 3 - Setup and Run your search
Hint: Start small and with only a few restrictions (or in range of one of the known seeds I gave as a comment inside the code)
  1. open the ModEntry.cs file and locate the getConditions()-method (line 63 or use CTRL+F to search)
  2. modifiy the Conditions_List.Add(getListItem()); in accordance to your conditions (don't forget the trailing ';' if you add lines) <br>
    2.1 Argeuments in order are <br>
     2.1.1 bundle name - required and must match spelling as used by Stardew (see the getRoomByBundle() method for valid bundle names) <br>
     2.1.2 if bundle should be missing - "true" or "false" in lower case <br>
     2.1.3 item name - if irrelevant use "None" (use exact spelling), for information which and how many items can be excluded see getRoomByBundle() and get_ObjectID_by_Itemname <br>
     2.1.3.1 only items that can be missing in vanilla Stardew 1.6 are supported, if you need to check a different item you need to add their object id in get_ObjectID_by_Itemname <br>
     2.1.4 if item  should be missing - "true" or "false" in lower case <br>
  3. set your search depth and starting seed in checkConditionsWrapper()
  4. build the project (CTRL+B)
  5. run StardewModdingAPI.exe and wait for the title screen <br>
    5.1 you can technically run it while the game is still loading, but Stardew will crash if it needs attention before the search is through
  6. press B on your keyboard to run the search, the output will be in the SMAPI console window
  7. write down your seed, remove the Mod from the Mod folder, create your world and enjoy your challenge run
  
## Change .Net version of solution:
  1. open Project Explorer (View -> Project Explorer or press shortcut Ctrl+Alt+L)
  2. right-click "Stardew_Mod_RemixFinder" -> Properties (at the bottom of the menu)
  3. search the header Target framework in tab generel (should bethe second thing in the one already selected) -> choose .Net 6.0 in the dropdown
  4. if the .net version is not installed VS will show a Warning and install button in the project explorer
  
