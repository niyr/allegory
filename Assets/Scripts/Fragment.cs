using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vectrosity;

using Random = UnityEngine.Random;

public class Fragment : MonoBehaviour
{
    // CACHED COMPONENTS
    private Collider _collider;
    private GazeHandler _gazeHandler;

    public float explosionLerpTime = 0.3f;
    public float reassembleLerpTime = 1f;
    public float fadeOutTime = 0.6f;

    private Memory parent;
    private Transform target;
    private int groupId;

    private bool isMoving = false;
    private Vector3 spin;

    public Material lineMaterial;
    private VectorLine line;

    // Events
    public delegate void SelectedDelegate(Fragment selectedFragment);
    public static event SelectedDelegate OnSelected = delegate { };

    #region MonoBehaviour Lifecycle
    protected void Awake()
    {
        _collider = GetComponent<Collider>();
        _gazeHandler = GetComponent<GazeHandler>();

        _gazeHandler.OnGazeEnter += OnGazeEnter;
        _gazeHandler.OnGazeExit += OnGazeExit;
        _gazeHandler.OnGazeLocked += OnGazeLocked;
    }

    protected void Start()
    {
        spin = Vector3.one * Random.Range(-0.03f, 0.03f);

        OnSelected += OnFragmentSelected;
    }

    protected void Update()
    {
        if (!isMoving)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    protected void OnDestroy()
    {
        VectorLine.Destroy(ref line);
        OnSelected -= OnFragmentSelected;
    }
    #endregion

    #region Events
    private void OnGazeEnter(GazeHandler target)
    {
        //_animator.SetBool(HOVER_PARAM, true);
    }

    private void OnGazeExit(GazeHandler target)
    {
        //_animator.SetBool(HOVER_PARAM, false);
    }

    private void OnGazeLocked(GazeHandler target)
    {
        Reassemble();
        OnSelected(this);
    }

    private void OnFragmentSelected(Fragment selected)
    {
        if (selected.groupId == groupId && selected != this)
            FadeOut();
    }
    #endregion

    public void Init(Memory parent, int groupId, Transform target, Vector3 destination)
    {
        this.parent = parent;
        this.target = target;
        this.groupId = groupId;

        Shuffler[] shufflers = GetComponentsInChildren<Shuffler>();
        foreach (Shuffler s in shufflers)
            s.Shuffle();

        Explode(destination);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Explode(Vector3 targetPosition)
    {
        float duration = Random.Range(0.9f, 1.1f) * explosionLerpTime;
        StartCoroutine(CR_Explode(targetPosition, duration));
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

        /*
        List<Vector3> linePoints = new List<Vector3> { transform.position, target.position };
        line = new VectorLine("fragment_line", linePoints, 1f, LineType.Continuous, Joins.Weld);
        line.material = lineMaterial;
        line.continuousTexture = true;
        */

        //line.Draw3DAuto();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Reassemble()
    {
        StartCoroutine(CR_ReturnToBase(reassembleLerpTime));
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
        StartCoroutine(CR_FadeOut(fadeOutTime));
    }

    private IEnumerator CR_FadeOut(float duration = 1f)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Color[] originalColors = renderers.Select(x => x.color).ToArray();
        Color[] targetColors = new Color[originalColors.Length];
        for(int i = 0; i < originalColors.Length; i++)
        {
            originalColors[i].a *= 0.8f;
            targetColors[i] = originalColors[i];
            targetColors[i].a = 0f;
        }

        float speed = 1f / duration;
        for (float t = 0f; t < 1f; t += Time.deltaTime * speed)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].color = Color.Lerp(originalColors[i], targetColors[i], t);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
