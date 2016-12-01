using UnityEngine;
using System;
using System.Collections;

namespace GroundControl
{
    [RequireComponent(typeof(Orbit2D))]
    public class ISS2D : MonoBehaviour
    {
        [SerializeField]
        private float m_scaleFactor = 1.0f;
        [SerializeField]
        private float m_scaleTime = 1.0f;

        [Serializable]
        private class ISSSfx
        {
            public AudioClip collectCargoShipSfx;
        }

        [SerializeField]
        private ISSSfx m_sfx;
        [SerializeField]
        private AudioSource m_audioSource;

        private Orbit2D m_orbit;

        private Transform m_transform;
        private Vector3 m_intitialScale;

        private void Awake()
        {
            m_orbit = this.GetComponent<Orbit2D>();
            m_transform = this.transform;
            m_intitialScale = m_transform.localScale;
        }

        private void Update()
        {
            m_orbit.Orbit();
        }

        private void CollectCargoShip(CargoShip2D cargoShip)
        {
            cargoShip.WasCollected();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CargoShip2D cargoShip = other.GetComponent<CargoShip2D>();
            if (cargoShip != null)
            {
                CollectCargoShip(cargoShip);
                StartCoroutine(CollectFeedbackRoutine());
            }
        }

        private IEnumerator CollectFeedbackRoutine()
        {
            Vector3 targetScale = m_intitialScale * m_scaleFactor;
            float halfTime = m_scaleTime * 0.5f;
            float timer = 0.0f;
            m_audioSource.PlayOneShot(m_sfx.collectCargoShipSfx);
            while (timer <= halfTime)
            {
                Vector3 currentScale = Vector3.Lerp(m_intitialScale, targetScale, timer / halfTime);
                m_transform.localScale = currentScale;
                timer += Time.deltaTime;
                yield return null;
            }
            while (timer >= 0.0f)
            {
                Vector3 currentScale = Vector3.Lerp(m_intitialScale, targetScale, timer / halfTime);
                m_transform.localScale = currentScale;
                timer -= Time.deltaTime;
                yield return null;
            }
            m_transform.localScale = m_intitialScale;
        }
    }
}
