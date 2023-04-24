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

        [Header("Audio - Interacting")]
        [SerializeField] private SoundStruct eatingSounds;
        [SerializeField] private SoundStruct drinkingSounds, medicineSounds, ammoSounds;

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
        private PlayerInteraction playerInteraction;

        private void Awake() => Init();

        private void Init()
        {
            playerController = GetComponent<PlayerController>();
            playerStats = GetComponent<PlayerStats>();
            playerInteraction = GetComponent<PlayerInteraction>();
        }

        private void Start()
        {
            playerInteraction.OnInteract += PlayerInteraction_OnInteract;
        }

        private void OnEnable()
        {
            playerInteraction.OnInteract += PlayerInteraction_OnInteract;
        }

        private void OnDisable()
        {
            playerInteraction.OnInteract -= PlayerInteraction_OnInteract;
        }

        private void OnDestroy()
        {
            playerInteraction.OnInteract -= PlayerInteraction_OnInteract;
        }

        private void PlayerInteraction_OnInteract(object sender, PlayerInteraction.OnInteractEventArgs e)
        {
            Food food = e.i.GetComponent<Food>(); if (food) { PlayEatingSounds(); return; }
            Beverage beverage = e.i.GetComponent<Beverage>(); if (beverage) { PlayDrinkingSounds(); return; }
            Medicine medicine = e.i.GetComponent<Medicine>(); if (medicine) { PlayMedicineSounds(); return; }
            Ammo ammo = e.i.GetComponent<Ammo>(); if (ammo) { PlayAmmoSounds(); return; }
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

                            PlayRandomSound(walkSounds);

                            break;

                        case PlayerMovementState.RUNNING:

                            PlayRandomSound(runSounds);

                            break;

                        case PlayerMovementState.CROUCHING:

                            PlayRandomSound(crouchSounds);

                            break;
                    }

                    footstepTimer = CurrentOffset();
                }
            }
        }

        private void PlayRandomSound(SoundStruct sounds)
        {
            if (sounds.clips.Length == 0) return;

            //pick a random walk sound out of the array, and play one shot of it...
            PlayOneShotSound(sounds.clips[Random.Range(0, sounds.clips.Length)], sounds.transform.position, sounds.maxDistance, sounds.pitch);
        }

        private void PlayEatingSounds() => PlayRandomSound(eatingSounds);
        private void PlayDrinkingSounds() => PlayRandomSound(drinkingSounds);
        private void PlayMedicineSounds() => PlayRandomSound(medicineSounds);
        private void PlayAmmoSounds() => PlayRandomSound(ammoSounds);
    }
}