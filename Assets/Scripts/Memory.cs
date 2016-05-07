using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class Memory : MonoBehaviour, IPointerClickHandler
{
    public List<GameObject> pieces = new List<GameObject>();

    private List<Fragment> fragments = new List<Fragment>();

    public void Shatter()
    {
        for(int i = 0; i < pieces.Count; i++)
        {
            GameObject orig = pieces[i];

            for (int j = 0; j < 3; j++)
            {
                GameObject go = Instantiate(orig);
                Fragment f = Instantiate(GameManager.Instance.fragmentPrefab).GetComponent<Fragment>();
                go.transform.SetParent(f.transform);

                f.memory = this;
                f.target = orig.transform;
                f.groupId = i;

                f.Explode();

                fragments.Add(f);
            }

            orig.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Shatter();
    }
}
