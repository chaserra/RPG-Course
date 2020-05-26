using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject {

        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level) {
            BuildLookup();
            //lookupTable[characterClass][stat][level]; //How to display a value of a dictionary value

            float[] levels = lookupTable[characterClass][stat];
            if(levels.Length < level) {
                return 0;
            }
            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass) {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup() {
            if (lookupTable != null) { return; }

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            //Iterate through all items inside ProgressionCharacterClass array
            foreach (ProgressionCharacterClass progressionClass in characterClasses) {
                //Create new dictionary reference used as value for each Character Classes
                var statLookupTable = new Dictionary<Stat, float[]>();

                //Iterate through Stat inside ProgressionCharacterClass stats[]
                foreach (ProgressionStat progressionStat in progressionClass.stats) {
                    //Assign stat->levels[] pair to statLookupTable dictionary
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                //Assign dictionary reference (statLookupTable) as value for each Character Classes
                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat {
            public Stat stat;
            public float[] levels;
        }

    }
}