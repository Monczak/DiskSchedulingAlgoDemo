using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestMarker : MonoBehaviour
{
    [Header("Properties")]
    public float markerSize;
    public Color markerColor;
    public bool hasDeadline;
    public float deadlinePercent;
    public float deadlineDuration;

    [Header("References")]
    public Transform markerTransform;
    public MeshRenderer meshRenderer;
    public TMP_Text requestIdText;
    public Image deadlineRing;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateDeadline();
        UpdateDeadlineRing();
        UpdateProperties();
    }

    private void UpdateDeadlineRing()
    {
        if (hasDeadline)
        {
            deadlineRing.fillAmount = deadlinePercent;
        }
        else
        {
            deadlineRing.fillAmount = 0;
        }
    }

    private void UpdateDeadline()
    {
        deadlinePercent = Mathf.Lerp(0, 1, (deadlineDuration - SimulationManager.Instance.currentTime) / deadlineDuration);
    }

    private void UpdateProperties()
    {
        markerTransform.localScale = new Vector3(markerSize, 1, markerSize);
        meshRenderer.material.color = (hasDeadline && deadlinePercent == 0) ? new Color(markerColor.r * 0.5f, markerColor.g * 0.5f, markerColor.b * 0.5f, markerColor.a) : markerColor;
        deadlineRing.color = new Color(markerColor.r, markerColor.g, markerColor.b, 0.8f);
    }
}
