﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected Transform _transform;
    protected Collider _collider;
    protected Animator _animator;

    [SerializeField]
    private bool isSourceCard;
    private bool isRotating;

    protected static readonly int CLICK_PARAM = Animator.StringToHash("ClickTrig");
    protected static readonly int TURN_PARAM = Animator.StringToHash("isShown");
    protected static readonly int HIGHLIGHTED_PARAM = Animator.StringToHash("isHighlighted");

    private static Quaternion towardsRotation = Quaternion.Euler(0, 0, 0);
    private static Quaternion awayRotation = Quaternion.Euler(0, 180, 0);

    public delegate void CardClickedDelegate(Card chosen);
    public static event CardClickedDelegate OnCardClicked = delegate { };

    #region Properties
    public bool IsSourceCard { get { return isSourceCard; } }
    public bool IsRotating { get { return isRotating; } }
    #endregion

    #region MonoBehaviour Lifecycle
    protected void Awake()
    {
        _transform = GetComponent<Transform>();
        _collider = GetComponent<Collider>();
        _animator = GetComponent<Animator>();

        _transform.rotation = awayRotation;
        _collider.enabled = false;

        OnCardClicked += Card_OnCardClicked;

        gameObject.SetActive(false);
    }

    protected void Start()
    {

    }
    #endregion

    #region Interfaces
    public virtual void OnPointerClick(PointerEventData ped)
    {
        OnCardClicked(this);
        Debug.Log(gameObject.name + " clicked");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //_animator.SetBool(HIGHLIGHTED_PARAM, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //_animator.SetBool(HIGHLIGHTED_PARAM, false);
    }
    #endregion

    #region Events
    private void Card_OnCardClicked(Card chosen)
    {
        //StartCoroutine(CR_RotateAway());
    }
    #endregion

    public void RotateAway(Action callback = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(CR_RotateAway(callback));
    }

    private IEnumerator CR_RotateAway(Action callback = null)
    {
        isRotating = true;

        _collider.enabled = false;
        yield return StartCoroutine(CR_LerpRotation(towardsRotation, awayRotation, 1f));
        gameObject.SetActive(false);

        isRotating = false;

        if (callback != null)
            callback.Invoke();
    }

    public void RotateTowards(Action callback = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(CR_RotateTowards(callback));
    }

    private IEnumerator CR_RotateTowards(Action callback = null)
    {
        isRotating = true;

        _collider.enabled = false;
        yield return StartCoroutine(CR_LerpRotation(awayRotation, towardsRotation, 1f));
        _collider.enabled = true;

        isRotating = false;

        if (callback != null)
            callback.Invoke();
    }

    private IEnumerator CR_LerpRotation(Quaternion from, Quaternion to, float duration)
    {
        float speed = 1f / duration;

        for(float t = 0f; t < 1f; t += Time.deltaTime * speed)
        {
            _transform.rotation = Quaternion.Lerp(from, to, t);
            yield return null;
        }

        _transform.rotation = to;
    }
}
