using System;
using System.Collections.Generic;
using System.Text;

using MiniDB.Core.Models;

namespace MiniDB.Core.Services;

public class Validator
{
    public bool ValidateValue(string value, Column column)
    {
        var dataType = column.GetDataType();

        return dataType.Validate(value);
    }

    public bool ValidateRow(Row row, Table table)
    {
        if (row.Values.Count != table.Columns.Count)
            return false;

        for (int i = 0; i < table.Columns.Count; i++)
        {
            if (!ValidateValue(
                    row.Values[i],
                    table.Columns[i]))
            {
                return false;
            }
        }

        return true;
    }
}
