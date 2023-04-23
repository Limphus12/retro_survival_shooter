using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public struct SoundStruct
    {
        public AudioClip[] clips;
        public float maxDistance, pitch;

        public Transform transform;
    }


    public class PlayerSounds : SoundHandler
    {
        [Header("Audio - Footsteps")]
        [SerializeField] private float walkStepSpeed;
        [SerializeField] private float runStepMulti, crouchStepMulti;

        [Space]
        [SerializeField] private SoundStruct walkSounds;
        [SerializeField] private SoundStruct runSounds, crouchSounds;

        [Header("Audio - Breathing")]
        [SerializeField] private AudioSource breathingSource;

        private float footstepTimer = 0f;
        private float CurrentOffset()
        {
            PlayerMovementState state = playerController.GetMovementState();

            switch (state)
            {
                case PlayerMovementState.WALKING: 
                    return walkStepSpeed;

                case PlayerMovementState.RUNNING: 
                    return walkStepSpeed * runStepMulti;

                case PlayerMovementState.CROUCHING: 
                    return walkStepSpeed * crouchStepMulti;

                default: return 1f;
            }
        }

        private PlayerController playerController;
        private PlayerStats playerStats;

        private void Awake() => Init();

        private void Init()
        {
            playerController = gameObject.GetComponent<PlayerController>();
            playerStats = gameObject.GetComponent<PlayerStats>();
        }

        private void Update()
        {
            CheckFootstepSounds();
            CheckBreathingSounds();
        }

        private void CheckBreathingSounds()
        {
            //normalize teh stamina between 0 and 1
            float s = (float)playerStats.GetCurrentStamina() / (float)playerStats.GetMaxStamina();

            breathingSource.volume = Mathf.Abs(s - 1);
        }

        private void CheckFootstepSounds()
        {
            if (playerController.Grounded && playerController.IsMoving)
            {
                footstepTimer -= Time.deltaTime;

                if (footstepTimer <= 0)
                {
                    PlayerMovementState state = playerController.GetMovementState();

                    switch (state)
                    {
                        case PlayerMovementState.WALKING:

                            PlayFootstepSound(walkSounds);

                            break;

                        case PlayerMovementState.RUNNING:

                            PlayFootstepSound(runSounds);

                            break;

                        case PlayerMovementState.CROUCHING:

                            PlayFootstepSound(crouchSounds);

                            break;
                    }

                    footstepTimer = CurrentOffset();
                }
            }
        }

        private void PlayFootstepSound(SoundStruct sounds)
        {
            if (sounds.clips.Length == 0) return;

            //pick a random walk sound out of the array, and play one shot of it...
            PlayOneShotSound(sounds.clips[Random.Range(0, sounds.clips.Length)], sounds.transform.position, sounds.maxDistance, sounds.pitch);
        }
    }
}