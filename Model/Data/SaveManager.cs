using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Model.Data;

public static class SaveManager
{
    public static void Save(GameState state, string format)
    {
        string path = GetSavePath(format);

        if (format == "JSON")
        {
            string json = JsonConvert.SerializeObject(state, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        else if (format == "XML")
        {
            var serializer = new XmlSerializer(typeof(GameState));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, state);
            }
        }
        else
        {
            throw new NotSupportedException($"Формат {format} не поддерживается.");
        }
    }

    public static GameState Load(string format)
    {
        string path = GetSavePath(format);
        if (!File.Exists(path)) return null;

        if (format == "JSON")
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<GameState>(json);
        }
        else if (format == "XML")
        {
            var serializer = new XmlSerializer(typeof(GameState));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (GameState)serializer.Deserialize(stream);
            }
        }

        throw new NotSupportedException($"Формат {format} не поддерживается.");
    }

    private static string GetSavePath(string format)
    {
        string fileName = format == "JSON" ? "savegame.json" : "savegame.xml";
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
    }

    public static bool SaveExists(string format)
    {
        string path = GetSavePath(format);
        return File.Exists(path);
    }
}
