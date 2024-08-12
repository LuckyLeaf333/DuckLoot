using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using System.IO;

namespace DuckLoot
{
    [BepInPlugin(modGUID, modName, modVersion)] // Creating the plugin
    public class DuckLootMod : BaseUnityPlugin // MODNAME : BaseUnityPlugin
    {
        public const string modGUID = "LuckyLeaf333.DuckLoot"; // a unique name for your mod
        public const string modName = "DuckLoot"; // the name of your mod
        public const string modVersion = "1.0.0.1"; // the version of your mod

        private readonly Harmony harmony = new Harmony(modGUID); // Creating a Harmony instance which will run the mods

        void Awake() // runs when Lethal Company is launched
        {
            var BepInExLogSource = BepInEx.Logging.Logger.CreateLogSource(modGUID); // creates a logger for the BepInEx console
            BepInExLogSource.LogMessage(modGUID + " has loaded succesfully."); // show the successful loading of the mod in the BepInEx console

            harmony.PatchAll(typeof(duckLoot)); // run the mod class as a plugin
        }
    }

    [HarmonyPatch(typeof(RoundManager))] // selecting the Lethal Company script you want to mod
    [HarmonyPatch("LoadNewLevel")] // select during which Lethal Company void in the choosen script the mod will execute
    class duckLoot // This is your mod if you use this is the harmony.PatchAll() command
    {
        [HarmonyPostfix] // Postfix means execute the plugin after the Lethal Company script. Prefix means execute plugin before.
        static void Postfix(ref SelectableLevel ___currentLevel) // refer to variables in the Lethal Company script to manipulate them. Example: (ref int ___health). Use the 3 underscores to refer.
        {
            int i; // Track the index of the duck, if it exists
            bool duck_exists = false;

            for (i = 0; i < ___currentLevel.spawnableScrap.Count; i++)
            {
                if (___currentLevel.spawnableScrap[i].spawnableItem.itemName != "Rubber Ducky")
                {
                    continue;
                }
                else
                {
                    duck_exists = true;
                    break;
                }
            }
            if (duck_exists)
            {
                for(int j = 0; j < ___currentLevel.spawnableScrap.Count; j++)
                {
                    ___currentLevel.spawnableScrap[j] = ___currentLevel.spawnableScrap[i];   
                }
            }
        }
    }
}