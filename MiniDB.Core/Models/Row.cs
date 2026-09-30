using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDB.Core.Models;

public class Row
{
    public List<string> Values { get; set; } = new();

    public Row()
    {
    }

    public Row(IEnumerable<string> values)
    {
        Values = values.ToList();
    }

    public string GetValue(int index)
    {
        return Values[index];
    }

    public void SetValue(int index, string value)
    {
        Values[index] = value;
    }
}
