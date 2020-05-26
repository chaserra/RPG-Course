using System;
using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText {
    public class DamageText : MonoBehaviour {

        [SerializeField] TextMeshProUGUI damageText = null;

        public void SetDamageText(float damageAmount) {
            damageText.SetText(String.Format("{0:n0}", damageAmount));
        }

    }
}