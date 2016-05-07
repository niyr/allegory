using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Memory : MonoBehaviour, IPointerClickHandler
{
    private Transform _transform;

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

        Fragment.OnSelected += OnFragmentSelected;
    }

    protected void Start()
    {
        // TODO: Animate into position, then turn collider on
    }

    protected void OnDestroy()
    {
        Fragment.OnSelected -= OnFragmentSelected;
    }
    #endregion

    #region Interfaces
    public void OnPointerClick(PointerEventData eventData)
    {
        Shatter();
    }
    #endregion

    #region Events
    private void OnFragmentSelected(Fragment selected)
    {
        assembledFragments++;

        if(IsComplete)
        {

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

                fragment.memory = this;
                fragment.target = orig.transform;
                fragment.groupId = i;

                Vector3 moveTo = groupLoc + Random.insideUnitSphere * 5f;
                fragment.Explode(moveTo);

                fragments.Add(fragment);
            }

            orig.SetActive(false);
        }
    }
}
