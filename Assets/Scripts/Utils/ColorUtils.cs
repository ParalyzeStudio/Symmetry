using UnityEngine;

public class ColorUtils
{
    public static Color DarkenColor(Color inColor, float percentage)
    {
        Color outColor = Color.Lerp(inColor, Color.black, percentage);
        return outColor;
    }

    public static Color LightenColor(Color inColor, float percentage)
    {
        Color outColor = Color.Lerp(inColor, Color.white, percentage);
        return outColor;
    }
}

