using UnityEngine;

namespace EbayVR
{
    public class ConstantRotation : MonoBehaviour
    {
        private Transform _transform;

        [SerializeField]
        private Vector3 rotationSpeed;

        #region MonoBehaviour Lifecycle
        protected void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        protected void Update()
        {
            _transform.Rotate(rotationSpeed * Time.deltaTime);
        }
        #endregion
    }
}