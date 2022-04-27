using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public bool follow;
    private new Camera camera;

    public SpriteRenderer sectorLineRenderer;

    private Transform initialParent;
    private Vector3 initialPosition;

    private Vector3 lineLocalPosition;

    public float viewportMargin;

    private void Awake()
    {
        initialParent = transform.parent;
        initialPosition = transform.position;

        camera = Camera.main;

        GetLineLocalPosition();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (follow)
            RestrictPosition();
    }

    private void GetLineLocalPosition()
    {
        transform.parent = camera.transform;
        lineLocalPosition = transform.localPosition;
        transform.parent = initialParent;
    }

    private void RestrictPosition()
    {
        if (camera.transform.position.z < initialPosition.z)
        {
            transform.parent = camera.transform;
            transform.localPosition = lineLocalPosition;
        }
        else
        {
            transform.parent = initialParent;
            transform.position = initialPosition;
        }
        transform.position = new Vector3(initialPosition.x, initialPosition.y, transform.position.z);
    }
}
