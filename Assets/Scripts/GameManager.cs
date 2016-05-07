using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;
	public static GameManager Instance { get { return instance; } }

    [Header("Cards / Windows")]
    [SerializeField]
    private Card sourceCard;
    [SerializeField]
    private Card[] cardClones = new Card[3];

    [Header("Options")]
    [SerializeField]
    private List<GameObject> memoryPrefabs = new List<GameObject>();
    private int currentMemory = 0;

	#region MonoBehaviour Lifecycle
	protected void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

        Card.OnCardClicked += OnCardClicked;
	}

    protected void Start()
    {
        currentMemory = -1;
        NextMemory();
    }
    #endregion

    #region Events
    private void OnCardClicked(Card chosen)
    {
        if (chosen.IsSourceCard)
            chosen.RotateAway(ShowClones);
        else
            StartCoroutine(CR_HideClones());
    }
    #endregion

    public void NextMemory()
    {
        currentMemory++;
        if(currentMemory >= memoryPrefabs.Count)
        {
            Debug.Log("Game Over");
            return;
        }

        GameObject original = Instantiate(memoryPrefabs[currentMemory]);
        original.transform.SetParent(sourceCard.transform, false);

        foreach (Card c in cardClones)
        {
            GameObject clone = Instantiate(memoryPrefabs[currentMemory]);
            clone.transform.SetParent(c.transform, false);
            Shuffler[] shufflers = clone.GetComponentsInChildren<Shuffler>();
            foreach (Shuffler s in shufflers)
                s.Shuffle();

            c.gameObject.SetActive(false);
        }

        ShowSourceCard();
    }

    public void ShowSourceCard()
    {
        sourceCard.RotateTowards();
    }

    public void ShowClones()
    {
        foreach (Card c in cardClones)
            c.RotateTowards();
    }

    private IEnumerator CR_HideClones()
    {
        foreach (Card c in cardClones)
        {
            yield return new WaitForSeconds(0.1f);
            c.RotateAway();
        }

        bool isRotating = true;
        while(isRotating)
        {
            yield return null;

            isRotating = cardClones.Where(c => c.IsRotating == true).ToArray().Length == cardClones.Length;
        }

        yield return new WaitForSeconds(2f);

        NextMemory();
    }
}
