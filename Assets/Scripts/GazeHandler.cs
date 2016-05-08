using UnityEngine;
using System.Collections;

public class GazeHandler : MonoBehaviour
{
    public delegate void GazeDelegate(GazeHandler target);
    public event GazeDelegate OnGazeEnter = delegate { };
    public event GazeDelegate OnGazeExit = delegate { };
    public event GazeDelegate OnGazeLocked = delegate { };

    #region MonoBehaviour Lifecycle
    protected void Awake()
    {
        enabled = false;
    }

    protected void OnEnable()
    {
        OnGazeEnter(this);
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnGazeLocked(this);

        if (this != GazeManager.Instance.CurrentTarget)
            enabled = false;
    }

    protected void OnDisable()
    {
        OnGazeExit(this);
    }
    #endregion

    public void GazeEntered()
    {
        enabled = true;
    }

    public void GazeExited()
    {
        OnGazeExit(this);
    }
}
