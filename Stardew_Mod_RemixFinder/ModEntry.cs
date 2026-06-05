using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Bundles;
using StardewValley.Minigames;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;


namespace Stardew_Mod_RemixFinder
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>


        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/

        //DebugHelper
        private void debug_PrintNameID(string input)
        {
            string result = get_ObjectID_by_Itemname(input);
            this.Monitor.Log($"Input: |{input}| result: |{result}|.", LogLevel.Debug);
        }

        private void debug_getItemDataFromRoomdata(string roomData)
        {
            this.Monitor.Log($"debug_getItemDataFromRoomdata -Input: \n|{roomData}|", LogLevel.Debug);
            string itemdata = null;
            itemdata = getItemDataFromRoomdata(roomData, true);
            this.Monitor.Log($"result: |{itemdata}|.", LogLevel.Debug);
        }

        private void debug_getItemDictFromRoomdata(string roomData)
        {
            this.Monitor.Log($"debug_getItemDataFromRoomdata -Input: \n|{roomData}|", LogLevel.Debug);
            Dictionary<string, string> itemDict = new Dictionary<string, string>();
            itemDict = getItemDictFromRoomdata(roomData, true);
            //this.Monitor.Log($"result: |{itemdata}|.", LogLevel.Debug);
        }

        //setup conditions
        List<List<String>> getConditions()
        {
            //set the desired bans/items in here by adding the following line and change information about name and missing accordingly:
            //Conditions_List.Add(getListItem(<bundle name>,<true/false>,<item name>, <true/false>))

            //some bundles are guaranteed to show up, make sure you set the second argument to "false" for them if you need to ban an item in them
            //see the getRoomByBundle() method for valid bundle names and information how many items can be missing from them

            List<List<String>> Conditions_List = new List<List<String>>();
            //Conditions_List.Add(getListItem("Home Cook's")); //seed without the Bulletin Board Bundle: Homecook, found seeds: (0,4)
            //Conditions_List.Add(getListItem("Home Cook's", "false")); //seed with the Bulletin Board Bundle: Homecook, found seeds: (1,2,3)

            //Conditions_List.Add(getListItem("Spring Crops", "false", "Green Bean", "true")); //seed without a Green Bean (ID 188) in the Spring Crops bundle, found seed:(1,3)
            //Conditions_List.Add(getListItem("Spring Crops", "false", "Green Bean", "false")); //seed with a Green Bean in the Spring Crops bundle, found seeds: (0,2,4)

            Conditions_List.Add(getListItem("Spring Crops", "false", "Green Bean", "true"));
            Conditions_List.Add(getListItem("Spring Crops", "false", "Kale", "true"));
            Conditions_List.Add(getListItem("Summer Crops", "false", "Blueberry", "true"));
            Conditions_List.Add(getListItem("Animal"));
            Conditions_List.Add(getListItem("Home Cook's"));
            Conditions_List.Add(getListItem("Chef's"));
            Conditions_List.Add(getListItem("Dye", "true", "Beet", "true")); //bundle cam be missing, if exits: only without Beet
            Conditions_List.Add(getListItem("Dye", "true", "Amaranth", "true"));
            Conditions_List.Add(getListItem("Dye", "true", "Blueberry", "true"));
            Conditions_List.Add(getListItem("Dye", "true", "Red Cabbage", "true"));
            // valid seeds:3148, 4429, 4847 

            return Conditions_List;
        }

        private List<double> checkConditionsWrapper(List<List<String>> List_Conditions)
        {
            int max_depth = 5000; //not inclusive the last seed //210 manually checked
            int start_at_seed = 2000;
            bool debug_mode = false;

            return checkConditions(List_Conditions, max_depth, start_at_seed, debug_mode);

        }

        List<String> getListItem(string bundle, string bundle_should_be_missing = "true", string item = "None", string item_should_be_missing = "true")
        {
            List<String> listItem = new List<String>();
            listItem.Add(getRoomByBundle(bundle));
            listItem.Add(bundle);
            listItem.Add(bundle_should_be_missing);
            listItem.Add(item);
            listItem.Add(item_should_be_missing);
            return listItem;
        }

        //trigger the search
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (Context.IsWorldReady)
                return;

            // print button presses to the console window
            //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
            bool isBPressed = this.Helper.Input.IsDown(SButton.B);
            if (isBPressed)
            { 
                PressedB();
                
                List<List<String>> conditions = getConditions();
                this.Monitor.Log($"Searching a seed with these conditions:", LogLevel.Debug);
                if (conditions.Count == 0)
                {
                    this.Monitor.Log($"No conditions given, search terminated", LogLevel.Debug);
                    return;
                }
                
                foreach (List<String> condition in conditions)
                {
                    //room, bundle, bundle_should_be_missing, item, item_shoud_be_missing
                    if (condition[3] == "None")
                    {
                        //only bundle
                        if (condition[2] == "true") 
                        {
                            this.Monitor.Log($"No |{condition[1]}|-bundle in {condition[0]} ", LogLevel.Debug);
                        } //bundle_should_be_missing
                        else 
                        { 
                            this.Monitor.Log($"|{condition[1]}|-bundle in {condition[0]} ", LogLevel.Debug); 
                        }
                    }
                    else
                    {
                        //specific item
                        if (condition[4] == "true") 
                        {
                            this.Monitor.Log($"No |{condition[3]}|({get_ObjectID_by_Itemname(condition[3])}) in the |{condition[1]}|-bundle of {condition[0]}", LogLevel.Debug);
                        } //item_should_be_missing
                        else 
                        {
                            this.Monitor.Log($"|{condition[3]}|({get_ObjectID_by_Itemname(condition[3])}) in the |{condition[1]}|-bundle of {condition[0]}", LogLevel.Debug); 
                        }
                    }
                }

                List<double> validSeeds= checkConditionsWrapper(conditions);
                this.Monitor.Log($"-------End of Search----------", LogLevel.Debug);
                this.Monitor.Log($"valid Seeds: ", LogLevel.Debug);
                if (validSeeds.Count == 0)
                {
                    this.Monitor.Log($"Found None", LogLevel.Debug);
                }
                else
                {
                    this.Monitor.Log($"Found {validSeeds.Count} valid seeds:", LogLevel.Debug);
                    foreach (double seed in validSeeds)
                    {
                        this.Monitor.Log($"{seed}", LogLevel.Debug);
                    }
                }
            }
            else
                return;
                //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button} instead. Run with B", LogLevel.Debug);

        }

        private void PressedB()
        {
            this.Monitor.Log($"{Game1.player.Name} pressed B. Search started", LogLevel.Debug);
            //debugPrintNameID("Parsnip");
            //debugPrintNameID("parsnip");
            //debugPrintNameID("SummerSquash");

            //string roomdata = null;
            //roomdata = "Spring Crops/O 465 20/188 1 0 190 1 0 250 1 0 Carrot 1 0/0/4/0/Spring Crops";
            //debug_getItemDataFromRoomdata(roomdata);
            //debug_getItemDictFromRoomdata(roomdata);
            //roomdata = "Field Research/BO 20 1/422 1 0 392 1 0 702 1 0 536 1 0/5/4/32/Field Research";
            //debug_getItemDataFromRoomdata(roomdata);
            //debug_getItemDictFromRoomdata(roomdata);
            //roomdata = "Quality Crops/BO 15 1/192 5 2 258 5 2 272 5 2 270 5 2/6/3/3/Quality Crops";
            //debug_getItemDataFromRoomdata(roomdata);
            //debug_getItemDictFromRoomdata(roomdata);


            //for (int i = 0; i < 5; i++)
            //{
            //    generateBundle(i);
            //}
        }

        //bundle data
        private Dictionary<string, string> generateBundle(double seed, bool debug = false)
        {
            //content = CreateContentManager(StardewValley.InstanceGame.Content.ServiceProvider, StardewValley.InstanceGame.Content.RootDirectory);
            
            Random r = CreateRandom((double)seed * 9.0);
            Dictionary<string, string> bundle_data = new BundleGenerator().Generate(DataLoader.RandomBundles(Game1.content), r);
            if (debug)
            {
                this.Monitor.Log($"Generate Bundles for seed {seed}", LogLevel.Debug);
                foreach (var key in bundle_data.Keys)
                { 
                    this.Monitor.Log($"{key}: is {bundle_data[key]}", LogLevel.Debug); 
                }
            }
            return bundle_data;

        }

        //search and compare logic
        private List<double> checkConditions(List<List<String>> List_Conditions, int max_seed = 5, int min_seed = 0, bool debug = false)
        {
            List<double> valid_seed = new List<double>();

            if (max_seed <= min_seed)
            {
                this.Monitor.Log($"Error checkConditions(): maximal searched seed |{max_seed}|  must be bigger than minimum seed |{min_seed}|", LogLevel.Debug);
                return valid_seed;
            }

            this.Monitor.Log($"searching through seeds {min_seed} to {max_seed}", LogLevel.Debug);
            string room;
            string bundle;
            string bundle_should_be_missing;
            string item;
            string item_should_be_missing;

            string room_data;
            bool valid_CC = true;
            bool found_bundle_room;
            

            //Dictionary<string, string> itemDict = new Dictionary<string, string>();
            bool item_is_missing;

            Dictionary<string, string> unKnownItemDict = new Dictionary<string, string>();

            int count_start = min_seed;
            for (int i = min_seed; i < max_seed; i++)
            {
                if (debug) 
                {
                    this.Monitor.Log($"\n", LogLevel.Debug); //blank line
                    this.Monitor.Log($"\ncheck of seed {i}", LogLevel.Debug); 
                }

                //generate bundle for seed
                Dictionary<string, string> bundle_data = generateBundle(i, debug && true);
                //loop through conditions, if condition fail -> set valid_CC to false & exit loop
                found_bundle_room = false;
                foreach (List<String> condition in List_Conditions)
                {
                    //check roomn exists, missing room == missing bundle - nonesense, the room must exits
                    //check if bundle exists
                    room = condition[0];
                    bundle = condition[1];

                    if (room == "Unknown Bundle")
                    {
                        this.Monitor.Log($"Error. Encountered an unknown bundle, please check spelling of given conditions. Search will be terminated.", LogLevel.Debug);
                        return valid_seed;

                    }
                    if (room == "Boiler Room" || room == "Bulletin Board") //bundles in these have no fixed position
                    {
                        //loop through dictionary to get exact key
                        foreach (string key in bundle_data.Keys)
                        {
                            //if (found_bundle_room) {break;} //exit dictionary loop
                            if (debug && false)
                            {
                                this.Monitor.Log($"Debug room-key compare: room: {room}, key: {key}, begin: {compare_string(room, key, true)}", LogLevel.Debug);
                            }
                            if (compare_string(room, key, true))
                            {
                                
                                //valid room type, now check if bundle
                                if (debug && false)
                                {
                                    this.Monitor.Log($"Debug bundle-bundle_data compare: \nbundle: {bundle}, bundle_data: {bundle_data[key]}, begin: {compare_string(bundle, bundle_data[key], true)}", LogLevel.Debug);
                                }
                                if (compare_string(bundle, bundle_data[key], true))
                                {
                                    if (debug && false)
                                    {
                                        this.Monitor.Log($"Found bundle in room. Room: {room}, key: {key}", LogLevel.Debug);
                                    }
                                    //found bundle
                                    found_bundle_room = true;
                                    room = key;

                                    if (debug && false)
                                    {
                                        this.Monitor.Log($"new key: {room}", LogLevel.Debug);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else //!(room == "Boiler Room" || room == "Bulletin Board")
                    {
                        //check if correct bundle
                        found_bundle_room = compare_string(bundle, bundle_data[room], true);
                        
                    }

                    //room changing mystery - after contion loop
                    if (debug && false)
                    {
                        this.Monitor.Log($"---Debug: key after condtion : {room}", LogLevel.Debug);
                    }

                    //specified bundle is missing 
                    if (!found_bundle_room)
                    {
                        //didn't find room
                        bundle_should_be_missing = condition[2];
                        if (bundle_should_be_missing == "false") //bundle suppossed to be there, but no room found
                        {
                            valid_CC = false;
                            goto End_of_Loop;
                        }
                        else 
                        { //no room found -> item is guaranteed missing
                            item_should_be_missing = condition[4];
                            if (item_should_be_missing == "false") 
                            { //item suppossed to be there, not found for lack of room -> invalid seed
                                valid_CC = false;
                                goto End_of_Loop;
                            }
                            else
                            {//item should notbe there, not founhd for lack of room -> check next condition
                                found_bundle_room = false; //90% sure this is redundant
                                continue;
                            }
                        }
                    }
                    else //found room
                    {
                        bundle_should_be_missing = condition[2];
                        if (bundle_should_be_missing == "true" && condition[3] == "None") //found room, no item specified -> bundle should not be here
                        {
                            valid_CC = false;
                            goto End_of_Loop;
                        }
                    }

                    //check if correct bundle (already done in the if (room == "Boiler Room" || room == "Bulletin Board") & else)
                    //check if item exits
                    item = get_ObjectID_by_Itemname(condition[3]);
                    if (item == "Unknown Item" && item != "None")
                    {
                        item = condition[3];
                        if (!unKnownItemDict.ContainsKey(item))
                        {
                            this.Monitor.Log($"Unknown item |{item}|, search results may be inaccurate", LogLevel.Debug);
                            unKnownItemDict.Add(item, null);
                        }
                    }

                    if (debug)
                    {
                        this.Monitor.Log($"Key right before try to use: {room}, current item: {item} ({condition[3]})", LogLevel.Debug);
                    }
                    room_data = bundle_data[room];
                    if (debug)
                    {
                        this.Monitor.Log($"associated roomdata: {room_data}", LogLevel.Debug);
                    }
                    //this.Monitor.Log($"--Debug item: |{item}|, roomdata:\n{room_data}", LogLevel.Debug);
                    if (item != "None")
                    {
                        item_should_be_missing = condition[4];
                        //itemDict = getItemDictFromRoomdata(room_data);
                        item_is_missing = !getItemDictFromRoomdata(room_data, false).ContainsKey(item); //inverse of (true if item in dict, false if item not in dict)
                        if (item_is_missing && (item_should_be_missing == "false")) //item missing, should be there
                        {
                            valid_CC = false;
                            goto End_of_Loop;
                        }
                        if (!item_is_missing && (item_should_be_missing == "true")) //item found, should'nt be there
                        {
                            valid_CC = false;
                            goto End_of_Loop;
                        }

                    }



                }
            End_of_Loop:
                // if all conditions true -> print seed, else do nothing special
                if (valid_CC) {
                    this.Monitor.Log($"Found a valid seed: {i}", LogLevel.Debug);
                    valid_seed.Add(i);
                }
                //reset variables to default
                room = "";
                bundle = "";
                bundle_should_be_missing = "";
                item = "";
                item_should_be_missing = "";
                room_data = "";
                valid_CC = true;
                found_bundle_room = false;
                //item_is_missing = null; //can't reset a Boolean to nothing
                if (i % 20 == 0)
                {
                    this.Monitor.Log($"Current Progress: searched seeds {count_start} to {i}", LogLevel.Debug);
                }
            }
            return valid_seed;
        }
        
        private bool compare_string(string search, string text, bool is_begin = false) 
        {
            int string_pos = text.IndexOf(search);
            if (string_pos < 0) { return false;} //search not in text
            if (string_pos > 0 && is_begin ) {return false;} // search is in text, but not at the start
            return true;
        }

        private Dictionary<string, string> getItemDictFromRoomdata(string roomdata, bool debug = false) 
        {
            Dictionary<string, string> itemDict = new Dictionary<string, string>();
            string itemdata = getItemDataFromRoomdata(roomdata);
            string[] ItemAmountQuality = itemdata.Split(" "); //itemdata format: repeats {item amount quality}
            int index = 0;
            foreach(string entry in ItemAmountQuality) 
            {
                if ((index % 3) == 0) //index modulo 3 is 0 for the item position -> add to dict
                {
                    if (!itemDict.ContainsKey(entry)) { itemDict.Add(entry, null); } //add item to dict if not already exits (to prevent error if multiple same items are in a bundle, example: wood in Construction)
                } 
                index++;
            }

            if (debug)
            { 
                this.Monitor.Log($"\nSplit itemdata |{itemdata}|", LogLevel.Debug);
                foreach(string key in itemDict.Keys) {this.Monitor.Log($"|{key}|", LogLevel.Debug);}
            }
            return itemDict;
        }

        private string getItemDataFromRoomdata(string roomdata, bool debug = false)
        {
            string itemdata ="";
            int pos1;
            int pos2;
            int pos3;

            if ((roomdata.Length - roomdata.Replace("/","").Length)>=3) //make sure there are at least three / roomdata format: "romm id or name/rewards/items/unknown/required items?/sprite/room id or name" (room id and name is always identical)
            {
                pos1 = roomdata.IndexOf("/");
                if (debug) {this.Monitor.Log($"pos1 {pos1}", LogLevel.Debug);}
                pos2 = roomdata.IndexOf('/', pos1+1);
                if (debug) { this.Monitor.Log($"pos2 {pos2}", LogLevel.Debug); }
                pos3 = roomdata.IndexOf('/', pos2 + 1);
                if (debug) { this.Monitor.Log($"pos3 {pos3}", LogLevel.Debug); }

                itemdata = roomdata.Substring(pos2+1,pos3-pos2-1);
            }
            return itemdata;
        }

        //convert names
        string getRoomByBundle(string bundle)
        {
            switch (bundle)
            {
                //Pantry

                case "Spring Crops": //4 of 6
                    return "Pantry/0";
                case "Summer Crops": //4 of 5
                    return "Pantry/1";
                case "Fall Crops": //4 of 5
                    return "Pantry/2";
                case "Quality Crops":
                    //slot dependend:
                    //slot 1 Spring Crop of 4
                    //slot 2 Summer Crop of 3
                    //slot 3 Fall Crop of 3
                    //slot 4 Corn
                    return "Pantry/3";
                case "Rare Crops": //fix
                    return "Pantry/3";
                case "Animal": //fix
                    return "Pantry/4";
                case "Fish Farmer's": //fix
                    return "Pantry/4";
                case "Garden": //fix
                    return "Pantry/4";
                case "Artisan": //fix
                    return "Pantry/5";
                case "Brewer's": //fix
                    return "Pantry/5";
                //Fish Tank - other bundle slots are fixed
                case "Specialty Fish": //fix
                    return "Fish Tank/11";
                case "Quality Fish": //fix
                    return "Fish Tank/11";
                case "Master Fisher's": //fix
                    return "Fish Tank/11";
                //Crafts Room
                case "Spring Foraging": //4 of 5
                    return "Crafts Room/13";
                case "Summer Foraging": //fix
                    return "Crafts Room/14";
                case "Fall Foraging": //fix
                    return "Crafts Room/15";
                case "Winter Foraging": //4 of 5
                    return "Crafts Room/16";
                case "Construction": //fix
                    return "Crafts Room/17";
                case "Sticky": //fix
                    return "Crafts Room/17";
                case "Forest": //fix
                    return "Crafts Room/17";
                case "Exotic Foraging": //fix
                    return "Crafts Room/19";
                case "Wild Medicine": //fix
                    return "Crafts Room/19";
                //Boiler Room - no predetermined bundle positions
                case "Blacksmith's": //fix
                case "Geologist's": //fix
                case "Adventurer's": //4 of 5; 2 to deliver
                case "Treasure Hunter's": //fix
                case "Engineer's": //fix
                    return "Boiler Room";
                //Vault - always the same
                //Bulletin Board - no predetermined bundle positions
                case "Chef's": //fix
                case "Dye": //slot dependend 1 of 2 for each of the 6 slots
                case "Field Research": //fix
                case "Fodder": //fix
                case "Enchanter's": //fix
                case "Children's": //fix
                case "Forager's": //fix
                case "Home Cook's": //fix
                case "Helper's": //fix
                case "Spirit's Eve": //fix
                case "Winter Star": //fix
                    return "Bulletin Board";

                default:
                    return "Unknown Bundle";
            }
        }

        private string get_ObjectID_by_Itemname(string itemname)
        {
            switch (itemname.ToLower())
                //only items listed which can be missing from a bundle
            {
                //early break
                case "none":
                    return "None";
            //Pantry
                //Spring Crops
                case "parsnip":
                    return "24";
                case "green bean":
                    return "188";
                case "cauliflower":
                    return "190";
                case "potato":
                    return "192";
                case "kale":
                    return "250";
                case "carrot":
                    return "Carrot";
                //Summer Crops
                case "tomato":
                    return "256";
                case "hot pepper":
                    return "260";
                case "blueberry":
                    return "258";
                case "melon":
                    return "254";
                case "summer squash":
                    return "SummerSquash";
                //Fall Crops
                case "corn":
                    return "270";
                case "eggplant":
                    return "272";
                case "pumpkin":
                    return "276";
                case "yam":
                    return "280";
                case "broccoli":
                    return "Broccoli";

            //Crafts Room
                //Spring Forage
                case "wild horseradish":
                    return "16";
                case "daffodil":
                    return "18";
                case "leek":
                    return "20";
                case "dandelion":
                    return "22";
                case "spring onion":
                    return "399";
                //Winter Forage
                case "winter root":
                    return "412";
                case "crystal fruit":
                    return "414";
                case "snow yam":
                    return "416";
                case "crocus":
                    return "418";
                case "holly":
                    return "283";
                //Forest
                case "moss":
                    return "Moss";
                case "fiber":
                    return "771";
                case "acorn":
                    return "309";
                case "maple seed":
                    return "310";

            //Boiler Room
                //Adventurer's
                case "slime":
                    return "766";
                case "bat wing":
                    return "767";
                case "solar essence":
                    return "768";
                case "void essence":
                    return "769";
                case "bone fragment":
                    return "881";

            //Bulletin Board
                //Dye
                case "red mushroom":
                    return "420";
                case "beet":
                    return "284";
                case "sea urchin":
                    return "397";
                case "amaranth":
                    return "300";
                case "sunflower":
                    return "421";
                case "starfruit":
                    return "268";
                case "duck feather":
                    return "444";
                case "cactus fruit":
                    return "90";
                case "aquamarine": //other option = blueberry
                    return "62";
                case "red cabbage":
                    return "266";
                case "iridium bar":
                    return "337";

                default:
                    return "Unknown Item";
            }
            //return "test";
        }

        //clone of Stardew 1.6's Random Generator
        public static Random CreateRandom(double seedA, double seedB = 0.0, double seedC = 0.0, double seedD = 0.0, double seedE = 0.0)
        {
            return new Random(CreateRandomSeed(seedA, seedB, seedC, seedD, seedE));
        }

        public static int CreateRandomSeed(double seedA, double seedB, double seedC = 0.0, double seedD = 0.0, double seedE = 0.0)
        {
            if (Game1.UseLegacyRandom)
            {
                return (int)((seedA % 2147483647.0 + seedB % 2147483647.0 + seedC % 2147483647.0 + seedD % 2147483647.0 + seedE % 2147483647.0) % 2147483647.0);
            }
            return Game1.hash.GetDeterministicHashCode((int)(seedA % 2147483647.0), (int)(seedB % 2147483647.0), (int)(seedC % 2147483647.0), (int)(seedD % 2147483647.0), (int)(seedE % 2147483647.0));
        }

    }
}