using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour
{
    public float speed = 1f;

    [Tooltip("When enabled - Only activates for non-mobile builds. This is because GearVR or Cardboard has it's own system")]
    public bool nonMobileOnly = true;
    
    private Vector3 lookatRotation = Vector3.zero;

    private float mouseClickX;
    private float mouseClickY;

    private float clickedYaw;
    private float clickedPitch;

    private float yaw;
    private float pitch;

    private float dpiFactor;
    private const float REF_DPI = 72f;

    void Awake()
    {
        if (Application.isMobilePlatform && nonMobileOnly)
        {
            Destroy(this);
            return;
        }
    }

    void Start()
    {
        dpiFactor = Screen.dpi / REF_DPI;
        UpdateStartDragValues();
    }
    
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.LeftAlt))
            //UpdateStartDragValues();

        //if (Input.GetKey(KeyCode.LeftAlt))
            UpdateRotation();
    }

    private void UpdateStartDragValues()
    {
        mouseClickX = Input.mousePosition.x;
        mouseClickY = Input.mousePosition.y;

        clickedYaw = yaw;
        clickedPitch = pitch;
    }

    private void UpdateRotation()
    {
        // Reset the mouse positions to discard initial mouse offsets from app startup
        float deltaX = Input.mousePosition.x - mouseClickX;
        float deltaY = Input.mousePosition.y - mouseClickY;

        yaw = clickedYaw + (deltaX * speed * dpiFactor);
        pitch = clickedPitch - (deltaY * speed * dpiFactor);

        lookatRotation.y = yaw;
        lookatRotation.x = pitch;

        transform.localEulerAngles = lookatRotation;
    }
}