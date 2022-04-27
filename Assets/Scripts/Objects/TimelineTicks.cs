using UnityEngine;

public class TimelineTicks : MonoBehaviour
{
    public float tickSpacing;
    public float tickWidth;
    public float tickFalloff;
    public Color color;

    private Material material;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        SetMaterialProperties();
    }

    private void SetMaterialProperties()
    {
        material.SetFloat("_TickSpacing", tickSpacing);
        material.SetFloat("_TickWidth", tickWidth);
        material.SetFloat("_TickFalloff", tickFalloff);
        material.SetColor("_Color", color);
    }
}
