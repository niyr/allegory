using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Memory : MonoBehaviour, IPointerClickHandler
{
    // CACHED COMPONENTS
    private Transform _transform;
    private Collider _collider;

    public List<GameObject> pieces = new List<GameObject>();

    private List<Fragment> fragments = new List<Fragment>();

    private int assembledFragments = 0;

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

    #region Interfaces
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!IsComplete)
            Shatter();
    }
    #endregion

    #region Events
    private void OnFragmentSelected(Fragment selected)
    {
        assembledFragments++;

        if(IsComplete)
        {
            Debug.Log("Complete");
            OnComplete(this, 0f);
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

        Vector3 startPos = transform.position + new Vector3(0, 0, 60);
        Vector3 endPos = transform.position;

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

        Vector3 startPos = transform.position;
        Vector3 endPos = Camera.main.transform.position - new Vector3(0, 0, 1);

        for(float t = 0f; t < 1f; t += Time.deltaTime * 0.5f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }
}
