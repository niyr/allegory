using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour
{
    public float speed = 1f;

    public bool nonMobileOnly = true;

    private Vector3 lookatRotation = Vector3.zero;

    private float yaw;
    private float pitch;

    private float dpiFactor = 1f;
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
    }
    
    void Update()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        float deltaX = Input.GetAxis("Mouse X");
        float deltaY = Input.GetAxis("Mouse Y");

        yaw = deltaX * speed;
        pitch = deltaY * speed * -1f;

        lookatRotation.y = yaw;
        lookatRotation.x = pitch;

        transform.localEulerAngles += lookatRotation;
    }
}