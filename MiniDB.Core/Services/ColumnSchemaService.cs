using System;
using System.Collections.Generic;
using System.Text;

using MiniDB.Core.Models;

namespace MiniDB.Core.Services;

public class ColumnSchemaService
{
    public void RenameColumn(
        Table table,
        string oldName,
        string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException(
                "Назва колонки не може бути порожньою.");
        }

        var column = table.Columns.FirstOrDefault(c =>
            c.Name.Equals(
                oldName,
                StringComparison.OrdinalIgnoreCase));

        if (column == null)
        {
            throw new InvalidOperationException(
                $"Колонку '{oldName}' не знайдено.");
        }

        bool alreadyExists = table.Columns.Any(c =>
            c != column &&
            c.Name.Equals(
                newName,
                StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                $"Колонка '{newName}' вже існує.");
        }

        column.Name = newName;
    }

    public void MoveColumn(
        Table table,
        int oldIndex,
        int newIndex)
    {
        if (oldIndex < 0 ||
            oldIndex >= table.Columns.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(oldIndex));
        }

        if (newIndex < 0 ||
            newIndex >= table.Columns.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(newIndex));
        }

        if (oldIndex == newIndex)
            return;

        var column = table.Columns[oldIndex];

        table.Columns.RemoveAt(oldIndex);
        table.Columns.Insert(newIndex, column);

        foreach (var row in table.Rows)
        {
            string value = row.Values[oldIndex];

            row.Values.RemoveAt(oldIndex);
            row.Values.Insert(newIndex, value);
        }
    }
}