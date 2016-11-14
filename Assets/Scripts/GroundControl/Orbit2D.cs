using UnityEngine;
using System.Collections;

namespace GroundControl
{
    public class Orbit2D : MonoBehaviour
    {
        [SerializeField]
        private float m_orbitVelocity = 1.0f;

        [SerializeField]
        private Transform m_transformToOrbit;

        private Transform m_transform;

        private void Awake()
        {
            m_transform = this.transform;
        }

        public void Orbit()
        {
            Vector3 orbitPoint = GetOrbitPoint();
            m_transform.RotateAround(orbitPoint, Vector3.forward, m_orbitVelocity * Time.deltaTime);
        }

        public float GetOrbitVelocity()
        {
            return m_orbitVelocity;
        }

        public void SetOrbitVelocity(float orbitVelocity)
        {
            m_orbitVelocity = orbitVelocity;
        }

        public Vector3 GetOrbitPoint()
        {
            Vector3 orbitPoint = Vector3.zero;
            if (m_transformToOrbit != null)
            {
                orbitPoint = m_transformToOrbit.position;
            }
            return orbitPoint;
        }

        public void SetTransformToOrbit(Transform toOrbit)
        {
            m_transformToOrbit = toOrbit;
        }
    }
}
