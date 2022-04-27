using UnityEngine;

public class TimeMarker : MonoBehaviour
{
    public SpriteRenderer arrow, line;
    public float lineHeight = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        arrow.transform.position = Vector3.right * (CameraUtility.GetScreenWidthInUnits(0.5f) / 2 - (arrow.size.x / 2 * arrow.transform.localScale.x));
        line.transform.position = Vector3.right * (CameraUtility.GetScreenWidthInUnits(0.5f) - UIManager.Instance.topMask.GetDiskSectorLineSize()) / 2;
        line.transform.localScale = new Vector3(CameraUtility.GetScreenWidthInUnits(0.5f), lineHeight, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
