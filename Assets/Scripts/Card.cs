using ChaosTheoryGames.Audio;
using UnityEngine;
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

    private GameObject attachedMemory;

    [Header("Sounds")]
    [SerializeField]
    private SoundHelper flipSound;

    // Animator variables
    protected static readonly int CLICK_PARAM = Animator.StringToHash("clickTrig");
    protected static readonly int SHOWN_PARAM = Animator.StringToHash("isShown");
    protected static readonly int HIGHLIGHTED_PARAM = Animator.StringToHash("isHighlighted");

    private static Quaternion towardsRotation = Quaternion.Euler(0, 0, 0);
    private static Quaternion awayRotation = Quaternion.Euler(0, 180, 0); 

    // Events
    public delegate void CardClickedDelegate(Card chosenCard);
    public static event CardClickedDelegate OnCardClicked = delegate { };
    public delegate void CardHighlightedDelegate(Card highlightedCard, bool isHighlighted);
    public static event CardHighlightedDelegate OnCardHighlighted = delegate { };

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
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animator.SetBool(HIGHLIGHTED_PARAM, true);
        OnCardHighlighted(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animator.SetBool(HIGHLIGHTED_PARAM, false);
        OnCardHighlighted(this, false);
    }
    #endregion

    #region Events
    private void Card_OnCardClicked(Card chosen)
    {
        if (this == chosen)
            _animator.SetTrigger(CLICK_PARAM);
    }
    #endregion

    public void Clear()
    {
        if (attachedMemory != null)
            Destroy(attachedMemory);
    }

    public void AttachMemory(GameObject memory)
    {
        if(memory == null)
        {
            Debug.LogWarning("[Card]::Trying to attach a null memory.");
            return;
        }

        if (attachedMemory != null)
            Clear();

        memory.transform.SetParent(_transform, false);
        Shuffler[] shufflers = memory.GetComponentsInChildren<Shuffler>();
        foreach (Shuffler s in shufflers)
            s.Shuffle();

        attachedMemory = memory;
    }

    #region Movement
    public void RotateAway(Action callback = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(CR_RotateAway(callback));
    }

    private IEnumerator CR_RotateAway(Action callback = null)
    {
        isRotating = true;
        _animator.SetBool(SHOWN_PARAM, false);

        _collider.enabled = false;

        if (flipSound != null)
            flipSound.Play();

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
        _animator.SetBool(SHOWN_PARAM, true);

        if (flipSound != null)
            flipSound.Play();

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
    #endregion
}
