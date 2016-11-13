using UnityEngine;
using System.Collections;

namespace GroundControl
{
    public class Earth : MonoBehaviour
    {
        [SerializeField]
        private float m_rotationSpeed;

        private Transform m_transform;

        private void Awake()
        {
            m_transform = this.transform;
        }

        private void Update()
        {
            m_transform.Rotate(Vector3.forward * m_rotationSpeed * Time.deltaTime);
        }
    }
}
