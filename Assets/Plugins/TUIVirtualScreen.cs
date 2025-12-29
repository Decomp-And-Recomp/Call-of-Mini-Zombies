using UnityEngine;

public static class TUIVirtualScreen
{
    public static Vector2Int res;
    public static bool use;
    public static float multiplier;

    public static void Calc()
    {
        Vector2Int target = new Vector2Int(960, 640);

        float aspectRatio = Screen.height / (float)Screen.width;

        float targetArea = target.x * target.y;

        // From aspect ratio: w = aspectRatio * h
        // => area = w * h = aspectRatio * h^2
        // => h = sqrt(area / aspectRatio)
        float h = Mathf.Sqrt(targetArea / aspectRatio);
        float w = aspectRatio * h;

        res.Set(Mathf.RoundToInt(h), Mathf.RoundToInt(w));
    }
}
