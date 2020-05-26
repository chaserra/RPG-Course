using System;
using UnityEngine;
using TMPro;
using RPG.Attributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour {
        //Cache
        Health health;
        Fighter playerFighter;
        TextMeshProUGUI healthValue = null;

        private void Awake() {
            playerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            healthValue = GetComponent<TextMeshProUGUI>();
        }

        private void Update() {
            DisplayHealthValue();
        }

        private void DisplayHealthValue() {
            health = playerFighter.GetTarget();

            if (health == null) {
                healthValue.SetText("N/A");
                return;
            }
            healthValue.SetText(String.Format("{0:0}/{1:0} ({2:0}%)", health.GetHealthPoints(), health.GetMaxHealth(), health.GetPercentage()));
        }

    }
}