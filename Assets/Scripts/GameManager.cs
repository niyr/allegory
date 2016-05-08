using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    [Header("Memories")]
    [SerializeField]
    private List<Memory> memories = new List<Memory>();
    private int currentMemoryIndex = 0;
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("For Debugging")]
    [SerializeField]
    private bool repeatMemory;
    [SerializeField]
    private bool loopMemories;

    public AnimationCurve explosionCurve;
    public AnimationCurve rtbCurve;
    public GameObject fragmentPrefab;

    // Score tracking
    private int correctChoices = 0;

    #region Properties
    public Memory CurrentMemory { get { return memories[currentMemoryIndex]; } }
    #endregion

    #region MonoBehaviour Lifecycle
    protected void Awake()
	{
        Memory.OnComplete += OnMemoryComplete;
	}

    protected void Start()
    {
        currentMemoryIndex = -1;
        NextMemory();
    }
    #endregion

    #region Events
    private void OnMemoryComplete(Memory completed, float accuracy)
    {
        StartCoroutine(CR_NextMemory());
    }
    #endregion

    private IEnumerator CR_NextMemory(float delay = 5f)
    {
        yield return new WaitForSeconds(delay);
        NextMemory();
    }

    public void NextMemory()
    {
        currentMemoryIndex++;
        if(currentMemoryIndex == memories.Count)
        {
            if (loopMemories)
            {
                Debug.Log("[GameManager]::Reached end of memory sequence. Restarting.");
                currentMemoryIndex = 0;
            }
            else
            {
                Debug.Log("[GameManager]::End of memory sequence.");
                return;
            }
        }

        // Create the memory, it handles the rest
        Instantiate(memories[currentMemoryIndex].gameObject);
    }
}
