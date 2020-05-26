using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;

namespace RPG.Control 
{
    public class PlayerController : MonoBehaviour {

        //Cache
        private Mover mover;
        //private Fighter fighter;
        private Health health;

        //Parameters
        [SerializeField] float raycastRadius = .25f;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;

        [System.Serializable]
        struct CursorMapping {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

        private void Awake() {
            mover = GetComponent<Mover>();
            //fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }


        private void Update() {
            if (InteractWithUI()) { return; }
            if (health.IsDead()) {
                SetCursor(CursorType.None);
                return; 
            }
            if (InteractWithComponent()) { return; }
            //if (InteractWithCombat()) { return; }
            if (InteractWithMovement()) { return; }
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent() {
            RaycastHit[] hits = RayCastAllSorted();

            //Iterate through all objects hit with raycast (mouse)
            foreach (RaycastHit hit in hits) {
                //Check if object has IRaycastable interface
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                //Iterate through all IRaycastables in that object
                foreach (IRaycastable raycastable in raycastables) {
                    //Check if object implements HandleRaycast
                    if (raycastable.HandleRaycast(this)) {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            //If object has no IRaycastable interface
            return false;
        }

        //Sort raycasts according to distance. Closest first.
        private RaycastHit[] RayCastAllSorted() {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];

            for(int i = 0; i < distances.Length; i++) {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        //No longer needed. Kept for reference. Generic method to interact with components now used.
        //private bool InteractWithCombat() {
        //    RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
        //    foreach (RaycastHit hit in hits) {
        //        CombatTarget target = hit.collider.GetComponent<CombatTarget>();
        //        if (target == null) { continue; }

        //        if (!fighter.CanAttack(target.gameObject)) { continue; }

        //        if (Input.GetMouseButton(0)) {
        //            fighter.Attack(target.gameObject);
        //        }
        //        SetCursor(CursorType.Combat);
        //        return true;
        //    }
        //    return false;
        //}

        private bool InteractWithMovement() {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit) {
                if (!mover.CanMoveTo(target)) { return false; }

                if (Input.GetMouseButton(0)) {
                    mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target) {
            target = new Vector3();

            //Check if raycast hits anything
            RaycastHit raycastHit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out raycastHit);
            if (!hasHit) { return false; }
            
            //Check if raycast hits a navmesh (from baked navmesh)
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(
                raycastHit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if(!hasCastToNavMesh) { return false; }

            target = navMeshHit.position;

            return true;
        }

        private void SetCursor(CursorType type) {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type) {
            foreach(CursorMapping mapping in cursorMappings) {
                if(mapping.type == type) {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        //Getters and Setters
        private static Ray GetMouseRay() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }

}