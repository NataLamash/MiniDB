using MiniDB.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MiniDB.Core.Models;

public class Column
{
    public string Name { get; set; } = string.Empty;

    public string TypeName { get; set; } = "string";

    public Column()
    {
    }

    public Column(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    public MiniDB.Core.DataTypes.DataType GetDataType()
    {
        return DataTypeFactory.Create(TypeName);
    }
}
