using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;

namespace DuckLoot
{
    [BepInPlugin(modGUID, modName, modVersion)] // Creating the plugin
    public class DuckLootMod : BaseUnityPlugin // MODNAME : BaseUnityPlugin
    {
        public const string modGUID = "LuckyLeaf333.DuckLoot"; // a unique name for your mod
        public const string modName = "DuckLoot"; // the name of your mod
        public const string modVersion = "1.0.22"; // the version of your mod

        internal new static ManualLogSource Logger;
        private readonly Harmony harmony = new Harmony(modGUID); // Creating a Harmony instance which will run the mods
        public static ConfigEntry<float> configMultiplier;

        void Awake() // runs when Lethal Company is launched
        {
            configMultiplier = base.Config.Bind(
                "Duck Spawn Modification",                          // Config section
                "DuckSpawnMultiplier",                     // Key of this config
                1f,                    // Default value
                "Multiplies the amount of ducks by the given multiplier."    // Description
            );

            Logger = base.Logger;

            Logger.LogInfo($"Plugin {modGUID} is loaded!");
            harmony.PatchAll(typeof(StartOfRoundPatch)); // run the mod class as a plugin
            harmony.PatchAll(typeof(RoundManagerPatch));
        }
    }

    [HarmonyPatch(typeof(StartOfRound))] // selecting the Lethal Company script you want to mod
    [HarmonyPatch("StartGame")]
    class StartOfRoundPatch // This is your mod if you use this is the harmony.PatchAll() command
    {
        // Store duck item info
        private static SpawnableItemWithRarity duckItem = null;

        [HarmonyPrefix]
        // refer to variables in the Lethal Company script to manipulate them. Example: (ref int ___health). Use the 3 underscores to refer.
        static void Prefix(ref SelectableLevel[] ___levels)
        {
            if (duckItem == null)
            {
                DuckLootMod.Logger.LogDebug("Attempting to grab duck item info");

                // Search for a duck item within the moon level info
                foreach (var moon in ___levels)
                {
                    if (moon.PlanetName == "56 Vow")
                    {
                        foreach (var scrap in moon.spawnableScrap)
                        {
                            if (scrap.spawnableItem.itemName == "Rubber Ducky")
                            {
                                duckItem = scrap;
                                break;
                            }
                        }
                        break;
                    }
                }
                if(duckItem == null)
                {
                    DuckLootMod.Logger.LogDebug("Error: Duck item info not found");
                }
                else
                {
                    DuckLootMod.Logger.LogDebug("Success: Duck item info found");
                }

                // Set the spawableScrap for all levels to only Rubber Duckies
                DuckLootMod.Logger.LogDebug("Attempting to set spawnableScrap to only rubber duckies for all levels");
                if (duckItem != null)
                {
                    List<SpawnableItemWithRarity> duckList = new List<SpawnableItemWithRarity>();
                    duckList.Add(duckItem);
                    foreach (var moon in ___levels)
                    {
                        DuckLootMod.Logger.LogDebug($"Attempting to set {moon.PlanetName} spawnableScrap to ducks only");
                        moon.spawnableScrap = duckList;
                        DuckLootMod.Logger.LogDebug($"Length of {moon.PlanetName} spawnableScrap is now {moon.spawnableScrap.Count}");
                    }
                    DuckLootMod.Logger.LogDebug("Success: All levels' spawnable scrap set to duck");
                }
                else
                {
                    DuckLootMod.Logger.LogDebug("Error: Duck info is null");
                }
            }
            else
            {
                DuckLootMod.Logger.LogDebug("StartOfRoundPatch skipped: duck item info already found");
            }
        }
    }

    [HarmonyPatch(typeof(RoundManager))] // selecting the Lethal Company script you want to mod
    [HarmonyPatch("SpawnScrapInLevel")]
    class RoundManagerPatch // This is your mod if you use this is the harmony.PatchAll() command
    {
        [HarmonyPrefix]
        // refer to variables in the Lethal Company script to manipulate them. Example: (ref int ___health). Use the 3 underscores to refer.
        static void Prefix(ref float ___scrapAmountMultiplier)
        {
            DuckLootMod.Logger.LogDebug("Attempting to set scrapAmountMultiplier");
            ___scrapAmountMultiplier = DuckLootMod.configMultiplier.Value;
            DuckLootMod.Logger.LogDebug($"scrapAmountMultiplier set to {___scrapAmountMultiplier}");
        }
    }
}