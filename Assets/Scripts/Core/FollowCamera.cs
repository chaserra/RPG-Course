using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour {

        [SerializeField] Transform target;

        void LateUpdate() {
            FollowTarget();
        }

        private void FollowTarget() {
            transform.position = target.position;
        }

    }
}