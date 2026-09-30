using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDB.Core.DataTypes;

public class CharType : DataType
{
    public override string Name => "char";

    public override bool Validate(string value)
    {
        return value.Length == 1;
    }
}
