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
        public const string modVersion = "1.0.2"; // the version of your mod

        internal new static ManualLogSource Logger;
        private readonly Harmony harmony = new Harmony(modGUID); // Creating a Harmony instance which will run the mods
        public static ConfigEntry<int> configMultiplier;

        void Awake() // runs when Lethal Company is launched
        {
            configMultiplier = base.Config.Bind(
                "Duck Spawn Modification",                          // Config section
                "DuckSpawnMultiplier",                     // Key of this config
                1,                    // Default value
                "Multiplies the amount of ducks by the given multiplier."    // Description
            );

            Logger = base.Logger;

            Logger.LogInfo($"Plugin {modGUID} is loaded!");
            harmony.PatchAll(typeof(StartOfRoundPatch)); // run the mod class as a plugin
        }
    }

    [HarmonyPatch(typeof(StartOfRound))] // selecting the Lethal Company script you want to mod
    [HarmonyPatch("StartGame")]
    class StartOfRoundPatch // This is your mod if you use this is the harmony.PatchAll() command
    {
        [HarmonyPrefix]
        // refer to variables in the Lethal Company script to manipulate them. Example: (ref int ___health). Use the 3 underscores to refer.
        static void Prefix(ref SelectableLevel[] ___levels, ref SelectableLevel ___currentLevel)
        {
            DuckLootMod.Logger.LogDebug("Attempting to change duck spawn info");

            SpawnableItemWithRarity duckItem = null;

            // Search for a duck item within the moon level info
            foreach (var moon in ___levels)
            {
                DuckLootMod.Logger.LogDebug(moon.PlanetName);
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

            // Set the spawableScrap for the current level to only Rubber Duckies
            if (duckItem != null)
            {
                List<SpawnableItemWithRarity> duckList = new List<SpawnableItemWithRarity>();
                duckList.Add(duckItem);
                ___currentLevel.spawnableScrap = duckList;
                DuckLootMod.Logger.LogDebug("Current level spawnable scrap set to duck");
            }
            else
            {
                DuckLootMod.Logger.LogDebug("Error: duck not found");
            }

            // Modify the amount of scrap that can spawn on the level
            DuckLootMod.Logger.LogDebug($"Attempting to modify scrap spawn amount by {DuckLootMod.configMultiplier.Value}.");
            ___currentLevel.minScrap = ___currentLevel.minScrap * DuckLootMod.configMultiplier.Value;
            ___currentLevel.maxScrap = ___currentLevel.maxScrap * DuckLootMod.configMultiplier.Value;
            ___currentLevel.minTotalScrapValue = ___currentLevel.minTotalScrapValue * DuckLootMod.configMultiplier.Value;
            ___currentLevel.maxTotalScrapValue = ___currentLevel.maxTotalScrapValue * DuckLootMod.configMultiplier.Value;
            DuckLootMod.Logger.LogDebug("Completed modification of scrap spawn amount.");
        }
    }
}