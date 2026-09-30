using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDB.Core.DataTypes;

public abstract class DataType
{
    public abstract string Name { get; }

    public abstract bool Validate(string value);
}
