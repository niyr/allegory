using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Reticule : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // CACHED COMPONENTS
    private Transform _transform;

    protected void Awake()
    {
        _transform = GetComponent<Transform>();

        //Cursor.visible = false;
    }

    protected void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        _transform.position = mousePos;
    }

    #region Interfaces
    public void OnPointerDown(PointerEventData ped)
    {

    }

    public void OnPointerUp(PointerEventData ped)
    {

    }

    public void OnPointerClick(PointerEventData ped)
    {

    }
    #endregion
}
