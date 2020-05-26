using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider {

        //Cache
        private Mover mover;
        private ActionScheduler actionScheduler;
        private Animator animator;
        private BaseStats baseStats;

        //Parameters
        private Health target;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        //State
        private float timeSinceLastAttack = Mathf.Infinity;
        private WeaponConfig currentWeaponConfig;
        private LazyValue<Weapon> currentWeapon;

        private void Awake() {
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon() {
            return AttachWeapon(defaultWeapon);
        }

        private void Start() {
            currentWeapon.ForceInit();
        }

        private void Update() {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) { return; }
            if (target.IsDead()) { return; }

            if (!GetIsInRange(target.transform)) {
                mover.MoveTo(target.transform.position, 1f);
            } else {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void Attack(GameObject combatTarget) {
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        private void AttackBehaviour() {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > currentWeaponConfig.GetTimeBetweenAttacks()) {
                //Triggers Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack() {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        //Animation Event
        void Hit() {
            if (target == null) { return; }

            float damage = baseStats.GetStat(Stat.Damage);

            if (currentWeapon.value != null) {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile()) {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            } else {
                target.TakeDamage(gameObject, damage);
            }
        }

        void Shoot() {
            Hit();
        }

        public void Cancel() {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        private void StopAttack() {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public bool CanAttack(GameObject combatTarget) {
            if (combatTarget == null) { return false; }
            if (!mover.CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform)) 
            { 
                return false; 
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        //Weapon
        public void EquipWeapon(WeaponConfig weapon) {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon) {
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        //Getters and Setters
        private bool GetIsInRange(Transform targetTransform) {
            return Vector3.Distance(transform.position, targetTransform.transform.position) < currentWeaponConfig.GetRange();
        }

        public Health GetTarget() {
            return target;
        }

        //Saving and Loading
        public object CaptureState() {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state) {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        //TODO: BUGS!
        //1) Quickly changing targets damages the other enemy.
        //2) Due to collider added to player (from Cinematics lecture), clicking on the 
        //player now blocks movement

    }
}