using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDB.Core.DataTypes;

public class StringType : DataType
{
    public override string Name => "string";

    public override bool Validate(string value)
    {
        return value != null;
    }
}
