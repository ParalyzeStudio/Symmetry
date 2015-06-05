using UnityEngine;
using System;

public class ColorUtils
{
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
            throw new Exception("Percentage has to be a float number between 0 and 100");

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
}

