using System;
using System.Text;
using HarmonyLib;


namespace SOLAP
{
    [HarmonyPatch]
    internal class LoadCustomData
    {
        [HarmonyPatch(typeof(ModelManager), "ParseJson", new Type[0])]
        [HarmonyPostfix]
        private static async void LoadAndInjectSOLAPData()
        {
            byte[] fileData;
            using (var dataStream = typeof(SolapPlugin).Assembly.GetManifestResourceStream("SOLAP.src.custom_data.json"))
            {
                fileData = new byte[dataStream.Length];
                await dataStream.ReadAsync(fileData, 0, (int)dataStream.Length);
            }

            if (fileData == null || fileData.Length == 0) SolapPlugin.Log.LogError("Mod data file failed to load!");
            else
            {
                string fileText = Encoding.UTF8.GetString(fileData);
                SolapPlugin.Log.LogInfo("Parsing custom JSON data...");
                JSONObject jsonFile = JSONObject.ParseStream(fileText);
                foreach(string outerField in jsonFile.keys)
                {
                    SolapPlugin.Log.LogInfo($"Checking {outerField} for code to inject...");
                    JSONObject indId = jsonFile.GetField(outerField);
                    if(indId.Count == 0){
                        SolapPlugin.Log.LogWarning($"No code in {outerField} to inject, skipping.");
                        continue;
                    }
                    else
                    {
                        foreach(string strId in indId.keys)
                        {
                            string strCode = indId.GetField(strId).str;
                            ModelManager.FTryParseAndLoadScript(strId, strCode);
                        }
                    }
                }
                SolapPlugin.Log.LogInfo("All custom JSON data parsed and merged.");
            }
        }
    }
}