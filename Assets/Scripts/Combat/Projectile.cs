using UnityEngine;
using UnityEngine.Events;
using RPG.Attributes;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour {

        //Parameters
        [SerializeField] float projectileSpeed = 15f;
        [SerializeField] bool homingProjectile = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onHit;

        //State
        Health target = null;
        GameObject instigator = null;
        float damage = 0;
        bool hasHit = false;

        void Start() {
            if (target == null) { return; }
            if (homingProjectile) { return; }
            transform.LookAt(GetAimLocation());
        }

        void Update() {
            if (target.IsDead()) { return; }
            if (homingProjectile && !target.IsDead()) {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

        private Vector3 GetAimLocation() {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * (targetCapsule.height / 2);
        }

        public void SetTarget(Health target, GameObject instigator, float damage) {
            this.target = target;
            this.instigator = instigator;
            this.damage = damage;
            Destroy(gameObject, maxLifeTime);
        }

        private void OnTriggerEnter(Collider other) {
            if (hasHit) { return; }
            if (other.GetComponent<Health>() != target) { return; }
            if (target.IsDead()) { return; }

            target.TakeDamage(instigator, damage);
            projectileSpeed = 0;
            hasHit = true;
            onHit.Invoke();

            if (hitEffect != null) {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }
            foreach(GameObject toDestroy in destroyOnHit) {
                Destroy(toDestroy);
            }
            Destroy(gameObject, lifeAfterImpact);
        }

    }
}