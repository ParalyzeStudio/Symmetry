using UnityEngine;

public class ScreenUtils
{
    public static Vector2 GetScreenSize()
    {
        float fCameraSize = Camera.main.orthographicSize;

        float fScreenWidth = (float)Screen.width;
        float fScreenHeight = (float)Screen.height;
        float fScreenRatio = fScreenWidth / fScreenHeight;
        float fScreenHeightInUnits = 2.0f * fCameraSize;
        float fScreenWidthInUnits = fScreenRatio * fScreenHeightInUnits;

        return new Vector2(fScreenWidthInUnits, fScreenHeightInUnits);
    }

    public static float GetDiagonalLength()
    {
        return Mathf.Sqrt(GetDiagonalSquareLength());
    }

    public static float GetDiagonalSquareLength()
    {
        Vector2 screenSize = GetScreenSize();

        return screenSize.x * screenSize.x + screenSize.y * screenSize.y;
    }
}
