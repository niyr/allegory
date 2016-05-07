using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Snapshots")]
    [SerializeField]
    private AudioMixerSnapshot baseSnapshot;
    [SerializeField]
    private AudioMixerSnapshot highlightedSnapshot;
    [SerializeField]
    private float transitionTime = 1f;

    protected void Awake()
    {
        Card.OnCardHighlighted += OnCardHighlighted;
    }

    #region Events
    private void OnCardHighlighted(Card highlightedCard, bool isHighlighted)
    {
        if (isHighlighted)
            highlightedSnapshot.TransitionTo(transitionTime);
        else
            baseSnapshot.TransitionTo(transitionTime);
    }
    #endregion
}
