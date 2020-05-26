using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes 
{
    public class HealthBar : MonoBehaviour {

        //Parameters
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;


        void Update() {
            float healthFraction = healthComponent.GetFraction();

            if (Mathf.Approximately(healthFraction, 0f) || Mathf.Approximately(healthFraction, 1f)) {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(healthFraction, 1f ,1f);
        }

    }
}