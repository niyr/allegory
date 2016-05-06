using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    private Transform _transform;
    private Collider _collider;
    private Animator _animator;

    private static readonly int CLICK_PARAM = Animator.StringToHash("ClickTrig");
    private static readonly int TURN_PARAM = Animator.StringToHash("isShown");

    protected void Awake()
    {
        _transform = GetComponent<Transform>();
        _collider = GetComponent<Collider>();
        _animator = GetComponent<Animator>();
    }

    #region Interfaces
    public void OnPointerClick(PointerEventData ped)
    {

    }
    #endregion

    private void OnShowClones()
    {
        _animator.SetBool(TURN_PARAM, true);
    }
}
