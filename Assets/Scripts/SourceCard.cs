using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SourceCard : Card
{
    public delegate void SourceCardClickedDelegate(SourceCard card);
    public static event SourceCardClickedDelegate OnSourceCardClicked = delegate { };

    public override void OnPointerClick(PointerEventData ped)
    {
        OnSourceCardClicked(this);
    }
}
