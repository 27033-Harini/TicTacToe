using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicTacToe.FileHandlers
{
    public class FileHandler<T>
    {
        private readonly string _filePath;
        
        public FileHandler(string filePath)
        {
            this._filePath = filePath;
        }

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public T? Load()
        {
            if (!File.Exists(this._filePath))
            {
                return default(T);
            }
            try
            {
                string json = File.ReadAllText(this._filePath);
                return JsonSerializer.Deserialize<T>(json, Options) ?? default(T);
            }
            catch
            {
                return default(T);
            }
        }

        public void Save(T? data)
        {
            try
            {
                string json = JsonSerializer.Serialize
                (
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                }
                );

                File.WriteAllText(this._filePath, json);
            }
            catch
            {
                //nothing implemented yet..review later
            }
        }
    }
}
