using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SOLAP
{
    [BepInPlugin(Constants.PluginGuid, Constants.PluginName, Constants.PluginVersion)]

    public class SolapPlugin : BaseUnityPlugin
    {
        internal static Harmony Harmony;
        internal static ManualLogSource Log;
        internal static bool ModDataLoaded { get; set; }
        
        private void Awake()//unity calls this during startup of the game
        {
            Log = Logger;

            Log.LogInfo("Awake woke up");

            Harmony = new Harmony(Constants.PluginGuid);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());

        }
    }
}