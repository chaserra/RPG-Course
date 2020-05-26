using System;
using System.Collections;
using UnityEngine;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour {

        //Cache
        private Experience experience;

        //Parameters
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpVFX = null;
        [SerializeField] bool shouldUseModifiers = false;

        //State
        private LazyValue<int> currentLevel;

        //Events
        public event Action onLevelUp;

        private void Awake() {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start() {
            //currentLevel = CalculateLevel();
            currentLevel.ForceInit();
        }

        private void OnEnable() {
            if (experience != null) {
                //Subscribe UpdateLevel method to the event onExperienceGained, hence no ()
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if (experience != null) {
                //Subscribe UpdateLevel method to the event onExperienceGained, hence no ()
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value) {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect() {
            Instantiate(levelUpVFX, transform);
        }

        public float GetStat(Stat stat) {
            return (GetBaseStat(stat) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifier(stat)/100);
        }

        private float GetBaseStat(Stat stat) {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel() {
            //if(currentLevel.value < 1) {
            //    //Failsafe in case level is not initialized before Health script runs
            //    currentLevel.value = CalculateLevel();
            //}
            return currentLevel.value;
        }

        private float GetAdditiveModifiers(Stat stat) {
            if(!shouldUseModifiers) { return 0; }
            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach(float modifier in provider.GetAdditiveModifiers(stat)) {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat) {
            if (!shouldUseModifiers) { return 0; }
            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in provider.GetPercentageModifiers(stat)) {
                    total += modifier;
                }
            }
            return total;
        }

        private int CalculateLevel() {
            if (experience == null) { return startingLevel; }

            float currentXP = experience.GetExperienceValue();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for (int level = 1; level <= penultimateLevel; level++) {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (xpToLevelUp > currentXP) {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }

    }
}