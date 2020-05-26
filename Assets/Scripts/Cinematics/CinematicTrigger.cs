using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable {

        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other) {
            if(!hasTriggered && other.gameObject.tag == "Player") {
                hasTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }

        //Saving and Loading
        public object CaptureState() {
            return hasTriggered;
        }

        public void RestoreState(object state) {
            hasTriggered = (bool)state;
        }

    }
}