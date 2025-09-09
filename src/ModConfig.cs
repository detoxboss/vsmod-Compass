using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;

namespace Compass
{
    public class ModConfig
    {
        public string EnableScrapRecipeDesc = "Enable additional recipe for the Magnetic Compass. Uses Metal Scraps instead of Magnetite.";
        public bool EnableScrapRecipe = true;

        public string EnableOriginRecipeDesc = "Allow the Origin Compass to be crafted. <REQUIRED TO CRAFT THE RELATIVE COMPASS>";
        public bool EnableOriginRecipe = true;

        public string EnableRelativeRecipeDesc = "Allow the Relative Compass to be crafted.";
        public bool EnableRelativeRecipe = true;

        public string OriginCompassGearsDesc = "Number of Temporal Gears required to craft the Origin Compass. Min: 1, Max: 8";
        public int OriginCompassGears = 2;

        public string RelativeCompassGearsDesc = "Number of Temporal Gears required to craft the Relative Compass. Min: 1, Max: 8";
        public int RelativeCompassGears = 2;

        // static helper methods
        public static string filename = "CompassMod.json";

        public static ModConfig Load(ICoreAPI api)
        {
            ModConfig config = null;
            string configDir = api.GetOrCreateDataPath("Compass");
            string configFilepath = Path.Combine(configDir, filename);
            string errorLogPath = Path.Combine(configDir, "compass-mod-logs.txt");
            string badConfigFilepath = Path.Combine(configDir, "ERROR_" + filename);

            try
            {
                for (int attempts = 1; attempts < 4; attempts++)
                {
                    try
                    {
                        config = api.LoadModConfig<ModConfig>(filename);
                        break; // success
                    }
                    catch (JsonReaderException e)
                    {
                        var badLineNum = e.LineNumber;
                        api.Logger.Error($"[CompassMod Error] Unable to parse config JSON. Attempt {attempts} to salvage the file...");

                        if (attempts == 1)
                        {
                            if (File.Exists(badConfigFilepath)) File.Delete(badConfigFilepath);
                            if (File.Exists(configFilepath)) File.Copy(configFilepath, badConfigFilepath);
                            File.WriteAllText(errorLogPath, e.ToString());
                        }

                        if (attempts != 3 && File.Exists(configFilepath))
                        {
                            var lines = new List<string>(File.ReadAllLines(configFilepath));
                            if (badLineNum > 0 && badLineNum - 1 < lines.Count)
                            {
                                lines.RemoveAt(badLineNum - 1);
                                File.WriteAllText(configFilepath, string.Join("\n", lines.ToArray()));
                            }
                        }
                    }
                }

                try
                {
                    config = api.LoadModConfig<ModConfig>(filename);
                }
                catch (JsonReaderException)
                {
                    api.Logger.Error("[CompassMod Error] Unable to salvage config.");
                }
            }
            catch (System.Exception e)
            {
                api.Logger.Error("[CompassMod Error] Something went really wrong with reading the config file.");
                File.WriteAllText(errorLogPath, e.ToString());
            }

            if (config == null)
            {
                api.Logger.Warning("[CompassMod Warning] Unable to load valid config file. Generating default config.");
                config = new ModConfig();
            }

            config.OriginCompassGears = GameMath.Clamp(config.OriginCompassGears, 1, 8);
            config.RelativeCompassGears = GameMath.Clamp(config.RelativeCompassGears, 1, 8);

            Save(api, config);
            return config;
        }

        public static void Save(ICoreAPI api, ModConfig config)
        {
            api.StoreModConfig(config, filename);
        }
    }
}
