using System;
using System.Collections.Generic;
using System.Text;

using System.Text.Json;
using MiniDB.Core.Models;

namespace MiniDB.Core.Services;

public class DatabaseSerializer
{
    private readonly JsonSerializerOptions _options =
        new()
        {
            WriteIndented = true
        };

    public void Save(Database database, string path)
    {
        string json =
            JsonSerializer.Serialize(database, _options);

        File.WriteAllText(path, json);
    }

    public Database Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Файл бази даних не знайдено.",
                path);
        }

        string json = File.ReadAllText(path);

        var database =
            JsonSerializer.Deserialize<Database>(
                json,
                _options);

        if (database == null)
        {
            throw new InvalidOperationException(
                "Не вдалося завантажити базу даних.");
        }

        return database;
    }
}