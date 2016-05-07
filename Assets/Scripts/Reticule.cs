using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Reticule : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // CACHED COMPONENTS
    private Transform _transform;
    private Animator _animator;

    private float zDepth;

    private static readonly int HOVER_PARAM = Animator.StringToHash("isHovering");
    private static readonly int CLICK_PARAM = Animator.StringToHash("isMouseDown");

    protected void Awake()
    {
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();

        zDepth = _transform.localPosition.z;

        //Cursor.visible = false;
    }

    protected void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = zDepth;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        //Debug.Log(mousePos);
        //Debug.Log(worldPos);
        _transform.localPosition = GetWorldPositionOnPlane(Input.mousePosition, 0f);
    }

    protected void OnMouseEnter()
    {
        //_animator.SetBool(HOVER_PARAM, true);
    }

    protected void OnMouseExit()
    {
        //_animator.SetBool(HOVER_PARAM, false);
    }

    #region Interfaces
    public void OnPointerDown(PointerEventData ped)
    {
        //_animator.SetBool(CLICK_PARAM, true);
    }

    public void OnPointerUp(PointerEventData ped)
    {
        //_animator.SetBool(CLICK_PARAM, false);
    }

    public void OnPointerClick(PointerEventData ped)
    {

    }
    #endregion

    private Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}
