using UnityEngine;
using UnityEngine.AI;
using SilentExit.Audio;
using SilentExit.GameManager;

namespace SilentExit.Monster
{
    // The states the Golem AI can be in
    public enum GolemState
    {
        Patrol,
        Investigate,
        Chase,
        Search,
        Return,
        Attack
    }

    /// <summary>
    /// AI controller for the blind Golem monster.
    /// "It can't see you. It hears the water."
    /// Uses a basic state machine, NavMeshAgent, and an explicit delegate event for sound detection.
    /// Written in a simple student style without coroutines.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class GolemAI : MonoBehaviour
    {
        [Header("AI State")]
        public GolemState currentState = GolemState.Patrol;

        [Header("Movement Speeds")]
        public float patrolSpeed = 2.5f;
        public float investigateSpeed = 3.2f;
        public float chaseSpeed = 4.8f;

        [Header("Distances and Timing (seconds)")]
        public float attackRange = 1.8f;
        public float attackDuration = 1.8f;
        public float searchDuration = 4.0f;
        public float waypointWaitTime = 1.5f;

        [Header("Patrol Waypoints")]
        public Transform[] patrolWaypoints;
        private int currentWaypointIndex = 0;

        [Header("Component References")]
        public Transform playerTransform;
        public Animator animator;
        public GameManager.GameManager gameManager;

        [Header("Audio Output")]
        public AudioSource audioSource;
        public AudioClip[] footstepClips;
        public AudioClip growlClip;
        public AudioClip attackClip;
        public float footstepInterval = 0.65f;
        private float footstepTimer = 0f;

        // Private AI variables
        private NavMeshAgent agent;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;

        private Vector3 lastHeardSoundPos;
        private int recentFootstepCount = 0;

        // Simple timer for tracking elapsed time in states
        private float stateTimer = 0f;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            // Ensure no root motion conflicts with NavMeshAgent
            if (animator != null)
            {
                animator.applyRootMotion = false;
            }

            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        void Start()
        {
            // Find player if not assigned in Inspector
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.Find("FirstPersonController");
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                }
            }

            // Find game manager if not assigned in Inspector
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager.GameManager>();
            }

            SetState(GolemState.Patrol);
        }

        // Subscribe to sound events using the explicit named delegate taught in class
        void OnEnable()
        {
            SoundSystem.OnSoundEmitted += HandleSoundEmitted;
        }

        void OnDisable()
        {
            SoundSystem.OnSoundEmitted -= HandleSoundEmitted;
        }

        void Update()
        {
            // Update Animator "Walk" parameter based on actual movement speed
            if (animator != null && agent != null)
            {
                bool isMoving = agent.velocity.magnitude > 0.15f && !agent.isStopped;
                if (isMoving)
                {
                    animator.SetFloat("Walk", 1f);

                    // Footstep sound timing while walking
                    footstepTimer += Time.deltaTime;
                    if (footstepTimer >= footstepInterval)
                    {
                        footstepTimer = 0f;
                        PlayFootstepSound();
                    }
                }
                else
                {
                    animator.SetFloat("Walk", 0f);
                    footstepTimer = 0f;
                }
            }

            // Attack state handles its own timer
            if (currentState == GolemState.Attack)
            {
                UpdateAttack();
                return;
            }

            // Check if physically touching / catching the player
            if (playerTransform != null)
            {
                float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                if (distToPlayer <= attackRange)
                {
                    SetState(GolemState.Attack);
                    return;
                }
            }

            // Count elapsed time in the current state
            stateTimer += Time.deltaTime;

            // Basic AI State Machine using a switch statement
            switch (currentState)
            {
                case GolemState.Patrol:
                    UpdatePatrol();
                    break;
                case GolemState.Investigate:
                    UpdateInvestigate();
                    break;
                case GolemState.Chase:
                    UpdateChase();
                    break;
                case GolemState.Search:
                    UpdateSearch();
                    break;
                case GolemState.Return:
                    UpdateReturn();
                    break;
            }
        }

        // Delegate listener: called whenever the player emits a sound
        private void HandleSoundEmitted(Vector3 soundPos, float soundRadius, SoundType soundType)
        {
            // Ignore sounds while in the middle of attacking
            if (currentState == GolemState.Attack)
            {
                return;
            }

            // Check if the Golem is close enough to hear the sound
            float dist = Vector3.Distance(transform.position, soundPos);
            if (dist <= soundRadius)
            {
                lastHeardSoundPos = soundPos;

                if (soundType == SoundType.WaterSplash)
                {
                    // "It hears the water" - Immediate aggressive chase!
                    if (animator != null && currentState != GolemState.Chase)
                    {
                        animator.SetTrigger("Rage");
                    }
                    SetState(GolemState.Chase);
                    agent.SetDestination(soundPos);
                }
                else if (soundType == SoundType.SprintFootstep)
                {
                    // Running footsteps trigger chase
                    SetState(GolemState.Chase);
                    agent.SetDestination(soundPos);
                }
                else // Normal walking footstep
                {
                    if (currentState == GolemState.Chase)
                    {
                        // Update chase target
                        agent.SetDestination(soundPos);
                    }
                    else if (currentState == GolemState.Investigate)
                    {
                        recentFootstepCount += 1;
                        agent.SetDestination(soundPos);
                        if (recentFootstepCount >= 2)
                        {
                            SetState(GolemState.Chase);
                        }
                    }
                    else
                    {
                        recentFootstepCount = 1;
                        SetState(GolemState.Investigate);
                        agent.SetDestination(soundPos);
                    }
                }
            }
        }

        // Patrol along waypoints using the modulo (%) operator
        private void UpdatePatrol()
        {
            if (patrolWaypoints == null || patrolWaypoints.Length == 0) return;

            Transform targetWp = patrolWaypoints[currentWaypointIndex];
            if (targetWp == null) return;

            if (!agent.pathPending && agent.remainingDistance <= 1.0f)
            {
                if (stateTimer >= waypointWaitTime)
                {
                    // Modulo operator cycling to next waypoint
                    currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
                    agent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
                    stateTimer = 0f;
                }
            }
        }

        // Moving towards an investigated footstep
        private void UpdateInvestigate()
        {
            if (!agent.pathPending && agent.remainingDistance <= 1.2f)
            {
                SetState(GolemState.Search);
            }
        }

        // Chasing a splash or loud sprinting
        private void UpdateChase()
        {
            if (!agent.pathPending && agent.remainingDistance <= 1.2f)
            {
                SetState(GolemState.Search);
            }
        }

        // Searching the last heard sound location for 4 seconds
        private void UpdateSearch()
        {
            if (stateTimer >= searchDuration)
            {
                SetState(GolemState.Return);
            }
        }

        // Returning to the nearest patrol route
        private void UpdateReturn()
        {
            if (!agent.pathPending && agent.remainingDistance <= 1.2f)
            {
                SetState(GolemState.Patrol);
            }
        }

        // Attacking the caught player (simple timer instead of a coroutine)
        private void UpdateAttack()
        {
            stateTimer += Time.deltaTime;

            // Turn to face the player during attack
            if (playerTransform != null)
            {
                Vector3 lookPos = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
                transform.LookAt(lookPos);
            }

            // Once the attack duration has elapsed, return to patrol
            if (stateTimer >= attackDuration)
            {
                ResetToPatrol();
            }
        }

        // Changes the AI state and configures agent movement
        private void SetState(GolemState newState)
        {
            currentState = newState;
            stateTimer = 0f;

            if (agent == null || !agent.isOnNavMesh) return;

            switch (newState)
            {
                case GolemState.Patrol:
                    agent.isStopped = false;
                    agent.speed = patrolSpeed;
                    if (patrolWaypoints != null && patrolWaypoints.Length > 0 && patrolWaypoints[currentWaypointIndex] != null)
                    {
                        agent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
                    }
                    break;

                case GolemState.Investigate:
                    agent.isStopped = false;
                    agent.speed = investigateSpeed;
                    agent.SetDestination(lastHeardSoundPos);
                    PlayGrowlSound();
                    break;

                case GolemState.Chase:
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(lastHeardSoundPos);
                    PlayGrowlSound();
                    break;

                case GolemState.Search:
                    agent.isStopped = true;
                    if (animator != null)
                    {
                        animator.SetTrigger("IdleAction");
                    }
                    break;

                case GolemState.Return:
                    agent.isStopped = false;
                    agent.speed = patrolSpeed;
                    Vector3 returnPos = FindClosestPatrolPoint();
                    agent.SetDestination(returnPos);
                    break;

                case GolemState.Attack:
                    agent.isStopped = true;
                    if (animator != null)
                    {
                        animator.SetTrigger("Hit");
                    }
                    PlayAttackSound();
                    if (gameManager != null)
                    {
                        gameManager.OnPlayerCaught();
                    }
                    break;
            }
        }

        // Finds the closest patrol waypoint to resume patrol
        private Vector3 FindClosestPatrolPoint()
        {
            if (patrolWaypoints == null || patrolWaypoints.Length == 0) return spawnPosition;

            Vector3 closest = patrolWaypoints[0].position;
            float minDist = Vector3.Distance(transform.position, closest);

            for (int i = 1; i < patrolWaypoints.Length; i++)
            {
                if (patrolWaypoints[i] != null)
                {
                    float d = Vector3.Distance(transform.position, patrolWaypoints[i].position);
                    if (d < minDist)
                    {
                        minDist = d;
                        closest = patrolWaypoints[i].position;
                        currentWaypointIndex = i;
                    }
                }
            }
            return closest;
        }

        // Resets the monster to its spawn point and returns to patrol
        public void ResetToPatrol()
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.Warp(spawnPosition);
            }
            else
            {
                transform.position = spawnPosition;
                transform.rotation = spawnRotation;
            }
            currentWaypointIndex = 0;
            recentFootstepCount = 0;
            SetState(GolemState.Patrol);
        }

        // Plays a footstep sound from the assigned footstep clips
        private void PlayFootstepSound()
        {
            if (audioSource != null && footstepClips != null && footstepClips.Length > 0)
            {
                int idx = Random.Range(0, footstepClips.Length);
                if (footstepClips[idx] != null)
                {
                    audioSource.PlayOneShot(footstepClips[idx], 0.7f);
                }
            }
        }

        // Plays the growl / breathing sound
        private void PlayGrowlSound()
        {
            if (audioSource != null && growlClip != null)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(growlClip, 0.9f);
                }
            }
        }

        // Plays the aggressive attack sound
        private void PlayAttackSound()
        {
            if (audioSource != null && attackClip != null)
            {
                audioSource.PlayOneShot(attackClip, 1.0f);
            }
        }
    }
}
