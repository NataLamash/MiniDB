using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MiniDB.Core.Models;

    public class Database
    {
        public string Name { get; set; } = string.Empty;

        public List<Table> Tables { get; set; } = new();

        public Database()
        {
        }

        public Database(string name)
        {
            Name = name;
        }

        public void CreateTable(Table table)
        {
            if (Tables.Any(t =>
                    t.Name.Equals(table.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"Таблиця з назвою '{table.Name}' вже існує.");
            }

            Tables.Add(table);
        }

        public void DeleteTable(string name)
        {
            var table = GetTable(name);

            if (table == null)
                throw new InvalidOperationException("Таблицю не знайдено.");

            Tables.Remove(table);
        }

        public Table? GetTable(string name)
        {
            return Tables.FirstOrDefault(t =>
                t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
