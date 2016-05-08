using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Reticule : MonoBehaviour
{
    // CACHED COMPONENTS
    private Transform _transform;
    private Animator _animator;

    [SerializeField]
    private bool disableCursor = true;

    private static readonly int HOVER_PARAM = Animator.StringToHash("isHovering");
    private static readonly int MOUSEDOWN_PARAM = Animator.StringToHash("isMouseDown");

    protected void Awake()
    {
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = !disableCursor;
    }

    protected void Update()
    {
        // Update position based on mouse position
        //_transform.localPosition = GetWorldPositionOnPlane(Input.mousePosition, 0f);
        // If hovering over something, set Animator
        _animator.SetBool(HOVER_PARAM, IsHoveringOverCollider());
        // If holding the left mouse button down, set Animator
        _animator.SetBool(MOUSEDOWN_PARAM, Input.GetMouseButton(0));
    }

    private Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    private bool IsHoveringOverCollider()
    {
#if UNITY_EDITOR
        Color lineColor = Input.GetMouseButton(0) ? Color.red : Color.green;
        Debug.DrawRay(_transform.position, Camera.main.transform.forward * Camera.main.farClipPlane, lineColor, 0.016f);
#endif

        return Physics.Raycast(_transform.position, Camera.main.transform.forward, Camera.main.farClipPlane);
    }
}
