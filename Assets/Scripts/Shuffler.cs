using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Shuffler : MonoBehaviour
{
	// CACHED COMPONENTS
	private Transform _transform;

	[SerializeField]
	private bool shuffleChildren = true;
	private List<Transform> children = new List<Transform>();

	[SerializeField]
	private Vector3 maxPositionDelta = Vector3.zero;
	[SerializeField]
	private Vector3 maxRotationDelta = Vector3.zero;
	[SerializeField]
	private Vector3 maxScaleDelta = Vector3.zero;

	protected void Awake()
	{
		_transform = GetComponent<Transform>();

		if(shuffleChildren)
		{
			foreach(Transform t in _transform)
				children.Add(t);
		}
		else
		{
            children.Add(_transform);
		}
	}

    public void Shuffle()
    {
        foreach (Transform t in children)
            Shuffle(t);
    }

	private void Shuffle(Transform target)
	{
		Vector3 pos = Vector3.Scale(Random.onUnitSphere, maxPositionDelta);
		Quaternion rot = Quaternion.Euler(Vector3.Scale(Random.onUnitSphere, maxRotationDelta));
		Vector3 scale = Vector3.Scale(Random.onUnitSphere, maxScaleDelta);

		target.position += pos;
		target.rotation *= rot;
		target.localScale += scale;
	}
}
