using System.Collections;
using UnityEngine;
using RPG.Control;
using RPG.Attributes;

namespace RPG.Combat {
    public class WeaponPickup : MonoBehaviour, IRaycastable {

        //Cache
        SphereCollider sphereCollider;

        //Parameters
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0f;
        [SerializeField] float respawnTime = 5f;

        private void Awake() {
            sphereCollider = GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider other) {
            if(other.tag == "Player") {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject) {
            if (weapon != null) {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healthToRestore > 0) {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds) {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool flag) {
            sphereCollider.enabled = flag;
            foreach (Transform child in transform) {
                child.gameObject.SetActive(flag);
            }
        }

        public bool HandleRaycast(PlayerController callingController) {
            if(Input.GetMouseButtonDown(0)) {
                Pickup(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType() {
            return CursorType.Pickup;
        }

    }
}