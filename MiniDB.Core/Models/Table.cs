using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MiniDB.Core.Models;

public class Table
{
    public string Name { get; set; } = string.Empty;

    public List<Column> Columns { get; set; } = new();

    public List<Row> Rows { get; set; } = new();

    public Table()
    {
    }

    public Table(string name)
    {
        Name = name;
    }

    public void AddColumn(Column column)
    {
        if (Columns.Any(c =>
                c.Name.Equals(column.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Колонка '{column.Name}' вже існує.");
        }

        Columns.Add(column);

        foreach (var row in Rows)
            row.Values.Add(string.Empty);
    }

    public void DeleteColumn(string name)
    {
        int index = Columns.FindIndex(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (index < 0)
            throw new InvalidOperationException("Колонку не знайдено.");

        Columns.RemoveAt(index);

        foreach (var row in Rows)
        {
            if (index < row.Values.Count)
                row.Values.RemoveAt(index);
        }
    }

    public void AddRow(Row row)
    {
        if (row.Values.Count != Columns.Count)
        {
            throw new InvalidOperationException(
                "Кількість значень рядка не відповідає кількості колонок.");
        }

        Rows.Add(row);
    }

    public void EditRow(int index, Row row)
    {
        if (index < 0 || index >= Rows.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (row.Values.Count != Columns.Count)
        {
            throw new InvalidOperationException(
                "Кількість значень рядка не відповідає кількості колонок.");
        }

        Rows[index] = row;
    }

    public void DeleteRow(int index)
    {
        if (index < 0 || index >= Rows.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        Rows.RemoveAt(index);
    }
}