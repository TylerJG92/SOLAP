using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Enums;
using HarmonyLib;
using UnityEngine;

namespace SOLAP
{
    [HarmonyPatch]
    internal class ScriptCommandPatches
    {
        [HarmonyPatch(typeof(MCommand), "StringToOp")]
        [HarmonyPostfix]
        static void StringToOpPatch(string s, ref MCommand.Op __result)
        {
            //Unused MCommand.Op values are limited and patching around the enum would be complicated, so adding this one base command whose first argument specifies the actual command to run
            //Thankfully I was able to ALSO find one in SOL that was unused so I can also do the same thing here in SOLAP
            if (s == "customcommand") __result = MCommand.Op.SUPERDISABLESKILL; //Unused Stadia-exclusive command
        }

        [HarmonyPatch(typeof(MCommand), "Execute")]
        [HarmonyPrefix]
        static bool ExecutePatch(MCommand __instance, ref enum(Result), Action<MCommand> callback)
        {
            //insert new patch code for start of customcommand here
        }
    }
}