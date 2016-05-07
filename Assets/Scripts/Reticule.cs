using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Reticule : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // CACHED COMPONENTS
    private Transform _transform;
    private Animator _animator;

    private static readonly int HOVER_PARAM = Animator.StringToHash("isHovering");
    private static readonly int CLICK_PARAM = Animator.StringToHash("isMouseDown");

    protected void Awake()
    {
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();

        //Cursor.visible = false;
    }

    protected void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        _transform.position = mousePos;
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
}
