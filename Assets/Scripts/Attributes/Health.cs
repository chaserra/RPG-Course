using UnityEngine;
using UnityEngine.Events;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable {
        //Cache
        BaseStats baseStats;

        //Parameters
        [SerializeField] float regenerationPercentage = 70f;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> {

        }

        //State
        LazyValue<float> healthPoints;
        private float maxHealth;
        private bool isDead = false;

        private void Awake() {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth() {
            return baseStats.GetStat(Stat.Health);
        }

        private void Start() {
            //if(healthPoints < 0) {
            //    healthPoints = baseStats.GetStat(Stat.Health);
            //}
            //maxHealth = baseStats.GetStat(Stat.Health);
            healthPoints.ForceInit();
            maxHealth = healthPoints.value;
        }

        private void OnEnable() {
            baseStats.onLevelUp += RegenerateHealthOnLevelUp;
        }

        private void OnDisable() {
            baseStats.onLevelUp -= RegenerateHealthOnLevelUp;
        }

        public void TakeDamage(GameObject instigator, float damage) {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            takeDamage.Invoke(damage);

            if (healthPoints.value <= 0) {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
        }

        private void Die() {
            if (isDead) { return; }
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) { return; }

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealthOnLevelUp() {
            maxHealth = baseStats.GetStat(Stat.Health);
            //healthPoints = maxHealth;
            float regenHealthPoints = maxHealth * regenerationPercentage / 100;
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        public void Heal(float healthToRestore) {
            maxHealth = baseStats.GetStat(Stat.Health);
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, maxHealth);
        }

        //Getters and Setters
        public bool IsDead() {
            return isDead;
        }

        public float GetPercentage() {
            return GetFraction() * 100;
        }

        public float GetFraction() {
            return healthPoints.value / baseStats.GetStat(Stat.Health);
        }

        public float GetHealthPoints() {
            return healthPoints.value;
        }

        public float GetMaxHealth() {
            return maxHealth;
        }

        //Saving and Loading
        public object CaptureState() {
            return healthPoints.value;
        }

        public void RestoreState(object state) {
            healthPoints.value = (float)state;
            if (healthPoints.value <= 0) {
                Die();
            } else if (isDead == true) {
                isDead = false;
                GetComponent<Animator>().ResetTrigger("die");
                GetComponent<Animator>().SetTrigger("resurrect");
            }
        }

    }
}