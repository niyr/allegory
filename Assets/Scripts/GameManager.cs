using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    [Header("Cards / Windows")]
    [SerializeField]
    private Card sourceCard;
    [SerializeField]
    private Card[] cardClones = new Card[3];

    [Header("Options")]
    [SerializeField]
    private List<GameObject> memoryPrefabs = new List<GameObject>();
    private int currentMemory = 0;

    [Header("For Debugging")]
    [SerializeField]
    private bool repeatMemory;
    [SerializeField]
    private bool loopMemories;

    // Score tracking
    private int correctChoices = 0;

	#region MonoBehaviour Lifecycle
	protected void Awake()
	{
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
        {
            chosen.RotateAway(ShowClonedCards);
        }
        else
        {
            if (chosen.IsClosest)
                correctChoices++;

            StartCoroutine(CR_HideClones());
        }
    }
    #endregion

    public void NextMemory()
    {
        currentMemory++;
        if(currentMemory >= memoryPrefabs.Count)
        {
            if (loopMemories)
            {
                Debug.Log("[GameManager]::Reached end of memory sequence. Restarting.");
                currentMemory = 0;
            }
            else
            {
                Debug.Log("[GameManager]::End of memory sequence.");
                return;
            }
        }

        // Create a clone of the memory and insert it into the original card
        GameObject original = Instantiate(memoryPrefabs[currentMemory]);
        sourceCard.AttachMemory(original, 0f);

        // Create clones of the memory to insert into the cloned cards,
        //  with varying degrees of difference
        foreach (Card card in cardClones)
        {
            GameObject clone = Instantiate(memoryPrefabs[currentMemory]);
            // TODO: get this as a spread of values rather than pure random
            float difference = Random.Range(0f, 1f);
            card.AttachMemory(clone, difference);
            card.gameObject.SetActive(false);
        }

        // Set the card with the least difference to be the 'right' choice
        cardClones.OrderByDescending(card => card.Difference).Last().IsClosest = true;

        ShowOriginalCard();
    }

    public void ShowOriginalCard()
    {
        sourceCard.RotateTowards();
    }

    public void ShowClonedCards()
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

        // TODO: debugging only
        if (repeatMemory)
            currentMemory--;

        NextMemory();
    }
}
