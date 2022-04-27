using UnityEngine;

public class SideMenuAnimationController : MonoBehaviour
{
    public Vector2 activationAreaSize;

    private Controls controls;

    private RectTransform canvasRect;

    [HideInInspector]
    public Animator animator;

    private Vector2 mousePos;

    private void Awake()
    {
        controls = new Controls();

        controls.Global.MousePosition.performed += OnMousePosChanged;

        controls.Global.Enable();

        animator = GetComponent<Animator>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    private void OnMousePosChanged(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        mousePos = obj.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Show",
           mousePos.x < activationAreaSize.x &&
           mousePos.y > canvasRect.sizeDelta.y / 2 - activationAreaSize.y / 2 &&
           mousePos.y < canvasRect.sizeDelta.y / 2 + activationAreaSize.y / 2 &&
           !UIManager.Instance.showOptionsMenu);
    }
}
