using System;
using UnityEngine;
using RPG.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable {

        [SerializeField] float experiencePoints = 0;

        //Events for Observer Pattern
        //public delegate void ExperienceGainedDelegate();
        //===========================
        //Commented out above code since Action does the same as above.
        //Use Action (see below) if your delegate returns void and does not take any arguments
        //===========================
        public event Action onExperienceGained; //this creates a list of events

        public void GainExperience(float experience) {
            experiencePoints += experience;
            onExperienceGained(); //calls all methods subscribed in the list of events
        }

        //Saving
        public object CaptureState() {
            return experiencePoints;
        }

        public void RestoreState(object state) {
            experiencePoints = (float)state;
        }

        //Getters and Setters
        public float GetExperienceValue() {
            return experiencePoints;
        }
        
    }
}