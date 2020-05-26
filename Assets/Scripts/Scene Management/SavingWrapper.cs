using System.Collections;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement 
{
    public class SavingWrapper : MonoBehaviour {

        const string defaultSaveFile = "save";
        [SerializeField] float fadeInTime = .5f;

        private void Awake() {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene() {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        void Update() {
            if(Input.GetKeyDown(KeyCode.L)) {
                //Load();
                QuickLoad();
            }
            if(Input.GetKeyDown(KeyCode.S)) {
                Save();
            }
            if(Input.GetKeyDown(KeyCode.Delete)) {
                Delete();
            }
        }

        public void Save() {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load() {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        //Reloads the save and whole scene
        //Fixes bugs like reviving dead enemies, level staying the same while experience is rerolled, etc.
        public void QuickLoad() {
            StartCoroutine(LoadLastScene());
        }

        public void Delete() {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }

    }
}