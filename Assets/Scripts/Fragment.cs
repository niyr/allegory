using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using Random = UnityEngine.Random;

public class Fragment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Collider _collider;

    private Vector3 spin;

    public Memory memory;
    public Transform target;
    public int groupId;

    private bool isMoving = false;

    public delegate void SelectedDelegate(Fragment selectedFragment);
    public static event SelectedDelegate OnSelected = delegate { };

    #region MonoBehaviour Lifecycle
    protected void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    protected void Start()
    {
        spin = Vector3.one * Random.Range(-0.03f, 0.03f);

        OnSelected += OnFragmentSelected;
    }

    protected void Update()
    {
        if(!isMoving)
            transform.Rotate(spin);
    }

    protected void OnDestroy()
    {
        OnSelected -= OnFragmentSelected;
    }
    #endregion

    #region Interfaces
    public void OnPointerClick(PointerEventData eventData)
    {
        ReturnToBase();

        OnSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // TODO: highlight animation
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // TODO: unhighlight animation
    }
    #endregion

    #region Events
    private void OnFragmentSelected(Fragment selected)
    {
        Debug.Log("fragment " + groupId);
        if (selected.groupId == groupId && selected != this)
            FadeOut();
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public void Explode()
    {
        Vector3 targetPos = transform.position + Random.insideUnitSphere * 20f;
        StartCoroutine(CR_Explode(targetPos, 0.2f));
    }

    private IEnumerator CR_Explode(Vector3 targetPos, float duration = 0.2f)
    {
        _collider.enabled = false;

        Vector3 startPos = transform.position;
        AnimationCurve movement = GameManager.Instance.explosionCurve;

        float speed = 1f / duration;
        for(float t = 0f; t < 1f; t += Time.deltaTime * speed)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, movement.Evaluate(t));
            yield return null;
        }

        transform.position = targetPos;
        _collider.enabled = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ReturnToBase()
    {
        StartCoroutine(CR_ReturnToBase());
    }

    private IEnumerator CR_ReturnToBase(float duration = 1f)
    {
        isMoving = true;
        _collider.enabled = false;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        AnimationCurve movement = GameManager.Instance.rtbCurve;

        float speed = 1f / duration;
        for (float t = 0f; t < 1f; t += Time.deltaTime * speed)
        {
            transform.position = Vector3.Lerp(startPos, target.position, movement.Evaluate(t));
            transform.rotation = Quaternion.Lerp(startRot, target.rotation, movement.Evaluate(t));
            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;

        isMoving = false;
        enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void FadeOut()
    {
        StartCoroutine(CR_FadeOut());
    }

    private IEnumerator CR_FadeOut()
    {
        Debug.Log(gameObject.name + " fading");

        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
}
