using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour {

        private ParticleSystem particleSystem;

        [SerializeField] GameObject targetToDestroy = null;

        private void Awake() {
            particleSystem = GetComponent<ParticleSystem>();
        }

        void Update() {
            if (!particleSystem.IsAlive()) {
                if(targetToDestroy == null) {
                    Destroy(gameObject);
                } else {
                    Destroy(targetToDestroy);
                }
            }
        }

    }
}