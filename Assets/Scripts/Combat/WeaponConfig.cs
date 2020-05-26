using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat {
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject {

        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1.2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator) {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if(equippedPrefab != null) {
                weapon = Instantiate(equippedPrefab, GetTransform(rightHand, leftHand));
                weapon.name = weaponName;
            }

            //Checks if the animator's controller has changed from the initial controller
            //Returns null if the animator controller has NOT changed through an override (weapons)
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animatorOverride != null) {
                animator.runtimeAnimatorController = animatorOverride;
            } else if (overrideController != null) {
                //If it's already an override, find parent and use that to assign as the controller
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand) {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null) {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) { return; }

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        public bool HasProjectile() {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage) {
            if (target.IsDead()) { return; }
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand) {
            return isRightHanded ? rightHand : leftHand;
        }

        public float GetDamage() {
            return weaponDamage;
        }

        public float GetPercentageBonus() {
            return percentageBonus;
        }

        public float GetRange() {
            return weaponRange;
        }

        public float GetTimeBetweenAttacks() {
            return timeBetweenAttacks;
        }

    }
}