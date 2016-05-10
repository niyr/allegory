using UnityEngine;
using System.Collections;

public class GazeManager : Singleton<GazeManager>
{
    private GazeHandler currentTarget;
    public GazeHandler CurrentTarget { get { return currentTarget; } }

    protected void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Camera.main.farClipPlane))
        {
            GazeHandler target = hit.transform.GetComponent<GazeHandler>();
            if(target != null && target != currentTarget)
            {
                currentTarget = target;
                currentTarget.enabled = true;
            }
        }
        else
        {
            currentTarget = null;
        }
    }
}
