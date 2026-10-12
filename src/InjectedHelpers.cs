using System;
using System.Collections.Generic;
using System.Linq;

namespace SOLAP
{
    public class InjectedHelpers //SOL has its own script overwriteing behavior and the way the data is structured, im currently thinking it would be easier to copy everything inside a block and not use this especially since its not structured the same in the data.json
    {
        public static void OverwriteScriptStates(Dictionary<string, MScript> scripts, MScript newScript)
        {
            if (scripts == null || newScript == null) return;

            //if (!LoadSaveDataPatches.ShouldLoadDataWithID(newScript.id)) return;

            if (scripts.Keys.Contains(newScript.id))
            {
                MScript origScript = scripts[newScript.id];
                var stateIDs = newScript.states.Keys.ToArray();
                foreach (string stateID in stateIDs)
                {
                    origScript.states[stateID] = newScript.states[stateID];
                }
            }
            else
            {
                scripts.Add(newScript.id, newScript);
            }
        }
    }
}
