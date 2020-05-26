using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour {

        //Cache
        private Mover mover;
        private Fighter fighter;
        private Health health;
        private ActionScheduler actionScheduler;
        private GameObject player;

        //Parameters
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = .5f;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] float shoutDistance = 5f;

        //State
        private LazyValue<Vector3> guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private int currentWaypointIndex = 0;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float timeSinceAggrevated = Mathf.Infinity;
        private bool hasShouted = false;

        //Called by Unity
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private void Awake() {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            player = GameObject.FindWithTag("Player");
            guardPosition = new LazyValue<Vector3>(SetInitialPosition);
        }

        private Vector3 SetInitialPosition() {
            return transform.position;
        }

        private void Start() {
            guardPosition.ForceInit();
        }

        private void Update() {
            if (health.IsDead()) { return; }
            if (IsAggrevated() && fighter.CanAttack(player)) {
                AttackBehaviour();
            } else if (timeSinceLastSawPlayer < suspicionTime) {
                SuspicionBehaviour();
            } else {
                PatrolBehaviour();
            }
            UpdateTimers();
        }

        public void Aggrevate() {
            timeSinceAggrevated = 0;
        }

        private bool IsAggrevated() {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer <= chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }

        private void UpdateTimers() {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        //Behaviours
        private void AttackBehaviour() {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies() {
            if (hasShouted) { return; }
            hasShouted = true;
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0f);

            foreach (RaycastHit hit in hits) {
                AIController aiController = hit.collider.GetComponent<AIController>();
                if (aiController == null) { continue; }
                aiController.Aggrevate();
            }
        }

        private void SuspicionBehaviour() {
            actionScheduler.CancelCurrentAction();
            hasShouted = false;
        }

        private void PatrolBehaviour() {
            Vector3 nextPosition = guardPosition.value;
            
            if (patrolPath != null) {
                if (AtWaypoint()) {
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime) {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        //Getters and Setters
        private bool AtWaypoint() {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint() {
            timeSinceArrivedAtWaypoint = 0;
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint() {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

    }
}