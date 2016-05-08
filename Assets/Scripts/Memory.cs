using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Memory : MonoBehaviour
{
    // CACHED COMPONENTS
    private Transform _transform;
    private Collider _collider;
    private Animator _animator;
    private GazeHandler _gazeHandler;

    public List<GameObject> pieces = new List<GameObject>();
    private List<Fragment> fragments = new List<Fragment>();

    private int assembledFragments = 0;

    private static readonly int HOVER_PARAM = Animator.StringToHash("isHovering");

    // Events
    public delegate void CompleteDelegate(Memory completedMemory, float accuracy);
    public static event CompleteDelegate OnComplete = delegate { };

    #region Properties
    private bool IsComplete { get { return assembledFragments == pieces.Count; } }
    #endregion

    #region MonoBehaviour Lifecycle
    protected void Awake()
    {
        _transform = GetComponent<Transform>();
        _collider = GetComponent<Collider>();
        _animator = GetComponent<Animator>();
        _gazeHandler = GetComponent<GazeHandler>();

        _gazeHandler.OnGazeEnter += OnGazeEnter;
        _gazeHandler.OnGazeExit += OnGazeExit;
        _gazeHandler.OnGazeLocked += OnGazeLocked;

        Fragment.OnSelected += OnFragmentSelected;
    }

    protected void Start()
    {
        // TODO: Animate into position, then turn collider on
        AnimateIn();
    }

    protected void OnDestroy()
    {
        Fragment.OnSelected -= OnFragmentSelected;
    }
    #endregion

    #region Events
    private void OnGazeEnter(GazeHandler target)
    {
        //_animator.SetBool(HOVER_PARAM, true);
        Debug.Log("gaze entered");
    }

    private void OnGazeExit(GazeHandler target)
    {
        //_animator.SetBool(HOVER_PARAM, false);
    }

    private void OnGazeLocked(GazeHandler target)
    {
        if (!IsComplete)
            Shatter();
    }

    private void OnFragmentSelected(Fragment selected)
    {
        assembledFragments++;

        if(IsComplete)
        {
            OnComplete(this, 0f);
            AnimateOut();
        }
    }
    #endregion

    public void Shatter()
    {
        //Vector3 center = (_transform.position - Camera.main.transform.position) * 0.5f;
        Vector3 center = _transform.position;
        for(int i = 0; i < pieces.Count; i++)
        {
            GameObject orig = pieces[i];

            Vector2 random = Random.insideUnitCircle;
            Vector3 groupLoc = center + new Vector3(random.x, random.y, 0) * 10f;

            for (int j = 0; j < 3; j++)
            {
                GameObject go = Instantiate(orig);
                GameObject newFragment = (GameObject)Instantiate(GameManager.Instance.fragmentPrefab, _transform.position, Quaternion.identity);
                Fragment fragment = newFragment.GetComponent<Fragment>();
                go.transform.SetParent(fragment.transform, false);

                Vector3 moveTo = groupLoc + Random.insideUnitSphere * 5f;
                fragment.Init(this, i, orig.transform, moveTo);

                fragments.Add(fragment);
            }

            orig.SetActive(false);
        }

        _collider.enabled = false;
    }

    public void AnimateIn()
    {
        StartCoroutine(CR_AnimateIn());
    }

    private IEnumerator CR_AnimateIn()
    {
        _collider.enabled = false;

        Vector3 startPos = _transform.position + new Vector3(0, 0, 60);
        Vector3 endPos = _transform.position;

        for (float t = 0f; t < 1f; t += Time.deltaTime * 0.5f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        _collider.enabled = true;
    }

    public void AnimateOut()
    {
        StartCoroutine(CR_AnimateOut());
    }

    private IEnumerator CR_AnimateOut()
    {
        _collider.enabled = false;

        Vector3 startPos = _transform.position;
        Vector3 endPos = Camera.main.transform.position - new Vector3(0, 0, 1);

        for(float t = 0f; t < 1f; t += Time.deltaTime * 0.5f)
        {
            _transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
