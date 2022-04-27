using TMPro;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
    public static RequestManager Instance;

    public int diskSectorCount;

    public TMP_Text leftBoundText, rightBoundText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisk();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateDisk()
    {
        leftBoundText.text = "0";
        rightBoundText.text = diskSectorCount.ToString();
    }
}
