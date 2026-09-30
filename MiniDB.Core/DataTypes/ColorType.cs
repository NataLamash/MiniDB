using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniDB.Core.DataTypes;

public class ColorType : DataType
{
    public override string Name => "color";

    private static readonly Regex ColorRegex =
        new(@"^#[0-9A-Fa-f]{6}$");

    public override bool Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return ColorRegex.IsMatch(value);
    }
}
