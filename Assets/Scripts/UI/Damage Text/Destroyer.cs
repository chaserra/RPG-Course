using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText 
{
    public class Destroyer : MonoBehaviour {

        public void DestroyObject() {
            Destroy(transform.parent.gameObject);
        }

    }
}