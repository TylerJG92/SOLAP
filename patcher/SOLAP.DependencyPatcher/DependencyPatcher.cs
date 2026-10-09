using System; //Provides AppDomain and other core .NET types
using System.Collections.Generic; //Provides IEnumerable<T>
using System.IO; //Provides Path and File for working with files and folders
using System.Linq; //Provides FirstOrDefault and other LINQ methods
using BepInEx.Logging; //Provides ManualLogSource and BepInEx logging
using Mono.Cecil; //Provides AssemblyDefinition for reading and replacing assemblies


namespace SOLAP.DependencyPatcher
{
    public static class DependencyPatcher
    {
        //Creates a shared BepInEx log source that Initialize, Patch, and Finish can all use
        private static ManualLogSource patcherLog;

        //Gets the path of the DLL containing DependencyPatcher, then gets the folder containing that DLL
        private static readonly string patcherFolder = Path.GetDirectoryName(typeof(DependencyPatcher).Assembly.Location);

        //Combines the patcher folder path with the filename of the Newtonsoft.Json v11 replacement
        private static readonly string replacementPath = Path.Combine(patcherFolder, "Newtonsoft.Json.dll");

        //Called by BepInEx when this preloader patcher is initialized
        public static void Initialize()
        {
            //Creates the log source used by this patcher
            patcherLog = Logger.CreateLogSource("DependencyPatcher");

            //Checks whether the replacement Newtonsoft.Json file exists before patching begins
            if (!File.Exists(replacementPath))
            {
                //Logs an error if the required replacement file cannot be found
                patcherLog.LogError($"Required Newtonsoft.Json replacement was not found at {replacementPath}");
            }
        }

        //Tells BepInEx which game assembly this patcher should process
        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Newtonsoft.Json.dll" };

        //Called by BepInEx when it processes the targeted Newtonsoft.Json assembly
        //The ref keyword allows this method to replace the AssemblyDefinition BepInEx will continue using
        public static void Patch(ref AssemblyDefinition targetNewton)
        {
            //Reads the profile-local Newtonsoft.Json v11 DLL with Mono.Cecil
            //This creates an AssemblyDefinition without loading the DLL into the CLR runtime
            AssemblyDefinition replacementNewton =
                AssemblyDefinition.ReadAssembly(replacementPath);

            //Logs which version BepInEx originally targeted and which version will replace it
            patcherLog.LogInfo(
                $"Replacing {targetNewton.Name.Name} v{targetNewton.Name.Version} " +
                $"with v{replacementNewton.Name.Version}.");

            //Changes BepInEx's target AssemblyDefinition from the game's Newtonsoft.Json v9
            //to the replacement Newtonsoft.Json v11 AssemblyDefinition
            targetNewton = replacementNewton;
        }

        //Called by BepInEx after the preloader has finished processing and loading assemblies
        public static void Finish()
        {
            //Searches the assemblies currently loaded in the CLR for Newtonsoft.Json
            var loadedNewtonsoft = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(asm => asm.GetName().Name == "Newtonsoft.Json");

            //Checks whether Newtonsoft.Json was successfully loaded after patching
            if (loadedNewtonsoft != null)
            {
                //Logs the actual Newtonsoft.Json version now loaded into the runtime
                patcherLog.LogInfo(
                    $"Runtime Newtonsoft.Json version: {loadedNewtonsoft.GetName().Version}");
            }
            else
            {
                //Warns if Newtonsoft.Json was unexpectedly not loaded after the preloader finished
                patcherLog.LogWarning(
                    "Newtonsoft.Json was not loaded after preloader patching.");
            }
        }
    }
}