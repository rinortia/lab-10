using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Model.Data
{
    public class SerializerJSON : Serializer
    {
        public override void Serialize<T>(T data, string filePath)
        {
            ValidateFilePath(filePath);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto, // Позволяет сериализовать производные классы
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(data, settings);
            File.WriteAllText(filePath, json);
        }

        public override T Deserialize<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto // Необходимо для десериализации производных классов
            };
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}