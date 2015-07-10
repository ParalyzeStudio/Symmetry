using UnityEngine;

public class ColorUtils
{
    public static Color GetColorFromRGBAVector4(Vector4 rgbaVec4)
    {
        return new Color(rgbaVec4.x / 255.0f, rgbaVec4.y / 255.0f, rgbaVec4.z / 255.0f, rgbaVec4.w / 255.0f);
    }

    public static Color GetRGBAVector4FromColor(Color color)
    {
        return new Color(color.r * 255.0f, color.g * 255.0f, color.b * 255.0f, color.a * 255.0f);
    }

    public static Color DarkenColor(Color inColor, float t)
    {
        Color outColor = Color.Lerp(inColor, Color.black, t);
        return outColor;
    }

    public static Color LightenColor(Color inColor, float t)
    {
        Color outColor = Color.Lerp(inColor, Color.white, t);
        return outColor;
    }

    /**
     * Returns the color from the rainbow of colors (red-orange-yellow-yellow-green-cyan-blue-pink-purple-red)
     * **/
    public static Color GetRainbowColorAtPercentage(float percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new System.Exception("Percentage has to be a float number between 0 and 100");

        //List here colors that will serve as milestones (linear interpolation between two consecutive colors)
        Color[] milestones = new Color[6];
        milestones[0] = new Color(1, 0, 0, 1); //red
        milestones[1] = new Color(1, 1, 0, 1); //yellow
        milestones[2] = new Color(0, 1, 0, 1); //green
        milestones[3] = new Color(0, 1, 1, 1); //cyan
        milestones[4] = new Color(0, 0, 1, 1); //blue
        milestones[5] = new Color(1, 0, 1, 1); //purple

        //then find the correct tier where the percentage belongs
        int integerPercentage = (int)Mathf.RoundToInt(percentage * 100000);
        int tierLength = (int)Mathf.RoundToInt(10000000 / 6.0f); //6 tiers, 1 666 667 samples in each tier
        int tierIndex = integerPercentage / tierLength;
        float tierLocalPercentage = (float) integerPercentage % tierLength;
        tierLocalPercentage /= (float) tierLength;

        Color startColor = milestones[tierIndex];
        Color endColor = milestones[(tierIndex == 5) ? 0 : tierIndex + 1];

        return Color.Lerp(startColor, endColor, tierLocalPercentage);
    }

    /**
     * Gets a random color that is near the passed original color
     * The amplitude determines the maxismum values each channel R,G,B can differ from the original value
     * **/
    public static Color GetRandomNearColor(Color originalColor, float amplitude)
    {
        float randomValue = UnityEngine.Random.value;
        randomValue *= (2 * amplitude);
        randomValue -= amplitude; //[-amplitude; +amplitude]

        float nearColorRed = originalColor.r + randomValue;
        float nearColorGreen = originalColor.g + randomValue;
        float nearColorBlue = originalColor.b + randomValue;

        MathUtils.Clamp(ref nearColorRed, 0, 1);
        MathUtils.Clamp(ref nearColorGreen, 0, 1);
        MathUtils.Clamp(ref nearColorBlue, 0, 1);

        return new Color(nearColorRed, nearColorGreen, nearColorBlue, originalColor.a);
    }

    /**
     * Return a random color
     * **/
    public static Color GetRandomColor(float opacity = 1.0f)
    {
        float r = Random.value;
        float g = Random.value;
        float b = Random.value;

        return new Color(r, g, b, opacity);
    }
}

