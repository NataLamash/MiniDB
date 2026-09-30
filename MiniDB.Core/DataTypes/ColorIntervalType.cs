using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDB.Core.DataTypes;

public class ColorIntervalType : DataType
{
    public override string Name => "colorInvl";

    private readonly ColorType _colorType = new();

    public override bool Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        string[] parts = value.Split("..");

        if (parts.Length != 2)
            return false;

        string start = parts[0].Trim();
        string end = parts[1].Trim();

        if (!_colorType.Validate(start) ||
            !_colorType.Validate(end))
        {
            return false;
        }

        var startRgb = ParseColor(start);
        var endRgb = ParseColor(end);

        return startRgb.R <= endRgb.R &&
               startRgb.G <= endRgb.G &&
               startRgb.B <= endRgb.B;
    }

    private static (int R, int G, int B) ParseColor(string value)
    {
        int r = Convert.ToInt32(value.Substring(1, 2), 16);
        int g = Convert.ToInt32(value.Substring(3, 2), 16);
        int b = Convert.ToInt32(value.Substring(5, 2), 16);

        return (r, g, b);
    }
}
