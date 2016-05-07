using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class RendererSortingLayer : MonoBehaviour
{
    public string sortingLayer;

    protected void Awake()
    {
        Renderer r = GetComponent<Renderer>();
        r.sortingLayerName = sortingLayer;

        Destroy(this);
    }
}
