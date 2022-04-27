using TMPro;
using UnityEngine;

public class TogglableButton : MonoBehaviour
{
    private bool active;

    public TMP_Text text;

    private Color currentColor;

    public Color activeColor, inactiveColor;
    public float colorSmoothing;

    private void Awake()
    {
        currentColor = active ? activeColor : inactiveColor;
    }

    private void Update()
    {
        currentColor = Color.Lerp(currentColor, active ? activeColor : inactiveColor, colorSmoothing * Time.deltaTime);
        text.color = currentColor;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }
}
