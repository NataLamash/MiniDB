using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDB.Core.DataTypes;

public class IntegerType : DataType
{
    public override string Name => "integer";

    public override bool Validate(string value)
    {
        return int.TryParse(value, out _);
    }
}
