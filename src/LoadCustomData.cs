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
        private static async void LoadSOLAPData()
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
                JSONObject jsonFile = JSONObject.ParseStream(fileText);
                JSONObject tScripts = jsonFile.GetField("textscripts");
                SolapPlugin.Log.LogInfo("Parsing custom JSON data...");
                foreach(string strId in tScripts.keys)
                {
                    string strCode = tScripts.GetField(strId).str;          
                    ModelManager.FTryParseAndLoadScript(strId, strCode);
                }                
                SolapPlugin.Log.LogInfo("Custom JSON data parsed and merged.");
            }
        }
    }
}