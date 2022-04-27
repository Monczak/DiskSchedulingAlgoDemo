using UnityEngine;

public class ColorUtility
{
    public static float CalculateLuminance(Color color) => 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
}

