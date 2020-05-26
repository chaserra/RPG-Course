using System;
using UnityEngine;
using TMPro;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour {

        //Cache
        Health health;

        //Parameters
        TextMeshProUGUI healthValue = null;

        private void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            healthValue = GetComponent<TextMeshProUGUI>();
        }

        private void Update() {
            DisplayHealthValue();
        }

        private void DisplayHealthValue() {
            healthValue.SetText(String.Format("{0:0}/{1:0} ({2:0}%)", health.GetHealthPoints(), health.GetMaxHealth(), health.GetPercentage()));
        }

    }
}