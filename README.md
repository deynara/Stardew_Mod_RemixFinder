# Stardew_Mod_RemixFinder
README is WIP 

This is a mod skeleton of a Seedfinder for the game Stardew Valley to find the desired remix bundle configuration.

How to use?
Stardew_Mod_RemixFinder is designed to be created as it's own mod and runs in the game itself.

If you don't know much about programming or creating a mod:
  It should be simple enough to understand what to do.
if you do know much about programming or creating a mod:
  I'm sorry, I don't know much about programming or creating a mod.

Step 0 - Requirements:
  1.install Stardew Valley
  2.Install SMAPI. https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started#Install_SMAPI
    SMAPI is the thign that will load your mod into the game.
  3.Install an IDE (integrated development environment)
    which you'll use to edit and compile your mod code
    To create this I used Windows Virtual Stuido Community (it's free) https://visualstudio.microsoft.com/de/vs/community/
  4.Install the .NET 6 SDK. https://dotnet.microsoft.com/en-us/download/dotnet/6.0
    You need .NET 6 because it's the version used by Stardew. (yes it is end of life)

Step 1 - Create the project
  1.open your IDE
  2.Create a solution with a C# Class Library project  (Don't select Class Library (.NET Framework)! That's a separate thing with a similar name.)
  3. Use "Stardew_Mod_RemixFinder" as the project name
  4. Target .Net 6.0 (you may need to intall and change it later, see below)
  5. delete Class1.cs

Why seed finding as a mod?
To get the bundles generated correctly. With RNG involved it's important that calls are made in the exact order and amount. 
That's guaranteed if the the game's own logic and data handling is used.

Change .Net version of solution:
  1.open Project Explorer (View -> Project Explorer or press shortcut Ctrl+Alt+L)
  2.right-click "Stardew_Mod_RemixFinder" -> Properties (at the bottom of the menu)
  3.search the header Target framework in tab generel (should bethe second thing in the one already selected) -> choose .Net 6.0 in the dropdown
  4.if the .net version is not installed VS will show a Warning and install button in the project explorer
  
