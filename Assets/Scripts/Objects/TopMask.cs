using TMPro;
using UnityEngine;

public class TopMask : MonoBehaviour
{
    public Transform maskPlane;

    public SpriteRenderer diskSectorLineRenderer;
    public float diskSectorLineSize;
    public float diskSectorLineSizeMargin;

    public TimelineTicks ticks;
    public float tickCount;
    public float tickHeight;

    private float diskSectorLineActualSize;

    public RectTransform textCanvas;
    public TMP_Text leftBoundText, rightBoundText;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialize()
    {
        float screenWidth = CameraUtility.GetScreenWidthInUnits(1);
        diskSectorLineActualSize = screenWidth * diskSectorLineSize;
        diskSectorLineRenderer.size = new Vector2(diskSectorLineActualSize + screenWidth * diskSectorLineSizeMargin * 2, 1);

        ticks.tickSpacing = 1 / diskSectorLineActualSize * tickCount;

        float tickSpaceLength = diskSectorLineActualSize / tickCount;
        ticks.transform.localScale = new Vector2(diskSectorLineActualSize - tickSpaceLength, tickHeight);
        textCanvas.sizeDelta = new Vector2(diskSectorLineActualSize, 1);

        maskPlane.localScale = new Vector3(diskSectorLineRenderer.size.x / 10, 1, 3);
    }

    public float GetDiskSectorLineSize()
    {
        return diskSectorLineActualSize;
    }
}
