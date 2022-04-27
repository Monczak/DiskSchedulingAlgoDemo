using UnityEngine;

public class OptionsMenuMovement : MonoBehaviour
{
    private RectTransform canvasTransform;
    private RectTransform rectTransform;
    private CameraController cameraController;
    private CameraMovement cameraMovement;

    private void Awake()
    {
        canvasTransform = GetComponentInParent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraMovement = cameraController.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(-canvasTransform.rect.width, 0, -cameraController.offset.x / cameraMovement.panOffset), 0);
    }
}
