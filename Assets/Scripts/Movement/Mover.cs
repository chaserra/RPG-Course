using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable {

        //Cache
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private ActionScheduler actionScheduler;
        private Health health;

        //Parameters
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 20f;

        //State

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
        }

        void Update() {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction) {
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction) {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        private void UpdateAnimator() {
            //Vector3 localVelocity = transform.InverseTransformDirection(navMeshAgent.velocity);
            //float characterVelocity = localVelocity.z;
            float characterVelocity = navMeshAgent.velocity.magnitude;
            animator.SetFloat("forwardSpeed", characterVelocity);
        }

        //Check if navmesh path: exists, is connected, and is too long
        public bool CanMoveTo(Vector3 destination) {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) { return false; }
            if (path.status != NavMeshPathStatus.PathComplete) { return false; }
            if (GetPathLength(path) > maxNavPathLength) { return false; }

            return true;
        }

        private float GetPathLength(NavMeshPath path) {
            float total = 0;
            if (path.corners.Length < 2) { return total; }
            for (int i = 0; i < path.corners.Length - 1; i++) {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        public void Cancel() {
            navMeshAgent.isStopped = true;
        }

        //Saving and Loading
        [System.Serializable]
        struct MoverSaveData {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState() {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state) {
            MoverSaveData data = (MoverSaveData)state;
            navMeshAgent.enabled = false;
            navMeshAgent.Warp(data.position.ToVector());
            transform.eulerAngles = data.rotation.ToVector();
            navMeshAgent.enabled = true;
        }
    }
}