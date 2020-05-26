using RPG.Control;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour {

        enum DestinationIdentifier {
            A, B, C, D, E
        }

        //Cache
        PlayerController playerController = null;

        //Parameters
        [SerializeField] Object sceneToLoad;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 1.5f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other) {
            if(other.tag == "Player") {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition() {
            if (sceneToLoad == null) {
                Debug.LogError("Scene does not exist!");
                yield break;
            }
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            GetPlayerControllerComponent();
            playerController.enabled = false;
            yield return fader.FadeOut(fadeOutTime);

            SavingWrapper saveWrap = FindObjectOfType<SavingWrapper>();
            saveWrap.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad.name);
            GetPlayerControllerComponent();
            playerController.enabled = false;

            saveWrap.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            saveWrap.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            playerController.enabled = true;
            Destroy(gameObject);
        }

        private void GetPlayerControllerComponent() {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        private Portal GetOtherPortal() {
            foreach (Portal portal in FindObjectsOfType<Portal>()) {
                if (portal == this) { continue; }
                if (destination != portal.destination) { continue; }
                return portal;
            }
            return null;
        }

        private void UpdatePlayer(Portal otherPortal) {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.transform.position);
            player.transform.rotation = otherPortal.spawnPoint.transform.rotation;
        }

    }
}