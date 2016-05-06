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
			{
				Shuffle(t);
				children.Add(t);
			}
		}
		else
		{
			Shuffle(_transform);
		}
	}

	private void Shuffle(Transform target)
	{
		/*
		Vector3w pos = new Vector3(Random.Range(-maxPositionDelta.x, maxPositionDelta.x),
			Random.Range(-maxPositionDelta.y, maxPositionDelta.y),
			Random.Range(-maxPositionDelta.z, maxPositionDelta.z));
		*/

		Vector3 pos = Vector3.Scale(Random.insideUnitSphere, maxPositionDelta);
		Quaternion rot = Quaternion.Euler(Vector3.Scale(Random.insideUnitSphere, maxRotationDelta));
		Vector3 scale = Vector3.Scale(Random.insideUnitSphere, maxScaleDelta);

		target.position += pos;
		target.rotation *= rot;
		target.localScale += scale;
	}
}
