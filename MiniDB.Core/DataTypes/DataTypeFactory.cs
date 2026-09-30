using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDB.Core.DataTypes;

public static class DataTypeFactory
{
    public static DataType Create(string typeName)
    {
        return typeName.ToLowerInvariant() switch
        {
            "integer" => new IntegerType(),
            "real" => new RealType(),
            "char" => new CharType(),
            "string" => new StringType(),
            "color" => new ColorType(),
            "colorinvl" => new ColorIntervalType(),

            _ => throw new ArgumentException(
                $"Невідомий тип даних: {typeName}")
        };
    }

    public static IReadOnlyList<string> SupportedTypes =>
        new[]
        {
            "integer",
            "real",
            "char",
            "string",
            "color",
            "colorInvl"
        };
}
