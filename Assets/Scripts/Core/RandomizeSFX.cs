using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core 
{
    public class RandomizeSFX : MonoBehaviour {

        //Cache
        AudioSource source;

        //Parameters
        [SerializeField] AudioClip[] audioClips;

        private void Awake() {
            source = GetComponent<AudioSource>();
        }

        public void PlaySFX() {
            int randomSFX = Random.Range(0, audioClips.Length);
            source.clip = audioClips[randomSFX];
            source.Play();
        }

    }
}