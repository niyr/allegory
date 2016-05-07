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

    public float Shuffle(float maxDifference = 1f)
    {
        float delta = 0f;
        foreach (Transform t in children)
            delta += ShuffleChildren(t, maxDifference);

        return delta;
    }

	private float ShuffleChildren(Transform target, float maxDifference)
	{
		Vector3 pos = Vector3.Scale(Random.onUnitSphere * maxDifference, maxPositionDelta);
		Quaternion rot = Quaternion.Euler(Vector3.Scale(Random.onUnitSphere * maxDifference, maxRotationDelta));
		Vector3 scale = Vector3.Scale(Random.onUnitSphere * maxDifference, maxScaleDelta);

		target.position += pos;
		target.rotation *= rot;
		target.localScale += scale;

        // TODO: fix this calculation
        return pos.sqrMagnitude + scale.sqrMagnitude;
	}
}
