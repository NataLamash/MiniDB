using System;
using System.Collections.Generic;
using System.Text;

using System.Globalization;

namespace MiniDB.Core.DataTypes;

public class RealType : DataType
{
    public override string Name => "real";

    public override bool Validate(string value)
    {
        return double.TryParse(
                   value,
                   NumberStyles.Float,
                   CultureInfo.InvariantCulture,
                   out _)
               ||
               double.TryParse(value, out _);
    }
}
