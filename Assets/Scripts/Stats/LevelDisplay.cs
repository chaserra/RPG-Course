using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour {

        private BaseStats baseStats;
        private TextMeshProUGUI levelText = null;

        void Awake() {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            levelText = GetComponent<TextMeshProUGUI>();
        }

        void Update() {
            DisplayLevelValue();
        }

        private void DisplayLevelValue() {
            levelText.SetText(String.Format("{0:0}", baseStats.GetLevel()));
        }

    }
}