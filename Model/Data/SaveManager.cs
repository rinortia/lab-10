using System;
using System.IO;
using Newtonsoft.Json;

namespace Model.Data
{
    public static class SaveManager
    {
        public static string SavePath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DoodleJump",
            "save.json");

        public static bool SaveExists()
        {
            try
            {
                return File.Exists(SavePath) && new FileInfo(SavePath).Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public static void Save(GameState state)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
                File.WriteAllText(SavePath, JsonConvert.SerializeObject(state, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
            }
        }

        public static GameState Load()
        {
            try
            {
                return JsonConvert.DeserializeObject<GameState>(File.ReadAllText(SavePath));
            }
            catch
            {
                return null;
            }
        }
    }
}