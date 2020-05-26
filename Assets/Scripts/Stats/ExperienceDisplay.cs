using UnityEngine;
using TMPro;
using System;

namespace RPG.Stats 
{
    public class ExperienceDisplay : MonoBehaviour {

        //Cache
        private Experience experience;
        private TextMeshProUGUI xpText;

        void Awake() {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            xpText = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update() {
            DisplayExperienceValue();
        }

        private void DisplayExperienceValue() {
            xpText.SetText(String.Format("{0:0}", experience.GetExperienceValue()));
        }

    }
}