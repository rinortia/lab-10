using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Model.Data;

public static class SaveManager
{
    public static bool Save(GameState state, string format, string saveFolder = null)
    {
        string path = GetSavePath(format, saveFolder);
        string folder = Path.GetDirectoryName(path);

        if (!IsFolderWritable(folder))
        {
            return false;
        }

        try
        {
            if (format == "JSON")
            {
                string json = JsonConvert.SerializeObject(state, Formatting.Indented);
                File.WriteAllText(path, json);
                return true;
            }
            else if (format == "XML")
            {
                var serializer = new XmlSerializer(typeof(GameState));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(stream, state);
                }
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    public static GameState Load(string format, string saveFolder = null)
    {
        string path = GetSavePath(format, saveFolder);
        if (!File.Exists(path)) return null;

        try
        {
            GameState state;
            if (format == "JSON")
            {
                string json = File.ReadAllText(path);
                state = JsonConvert.DeserializeObject<GameState>(json);
            }
            else if (format == "XML")
            {
                var serializer = new XmlSerializer(typeof(GameState));
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    state = (GameState)serializer.Deserialize(stream);
                }
            }
            else
            {
                return null;
            }

            if (state?.Platforms?.Count > 0)
            {
                return state;
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    public static bool SaveExists(string format, string saveFolder = null)
    {
        string path = GetSavePath(format, saveFolder);
        return File.Exists(path);
    }

    public static bool IsValidSaveFile(string format, string saveFolder = null)
    {
        string path = GetSavePath(format, saveFolder);
        if (!File.Exists(path)) return false;

        try
        {
            if (format == "JSON")
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<GameState>(json) != null;
            }
            else if (format == "XML")
            {
                var serializer = new XmlSerializer(typeof(GameState));
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    return serializer.Deserialize(stream) != null;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private static bool IsFolderWritable(string folderPath)
    {
        try
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string testFile = Path.Combine(folderPath, "test.tmp");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string GetSavePath(string format, string saveFolder = null)
    {
        string folder = saveFolder ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DoodleJump",
            "Saves");

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string fileName = $"savegame.{format.ToLower()}";
        return Path.Combine(folder, fileName);
    }
}