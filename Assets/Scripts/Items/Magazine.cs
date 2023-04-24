using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public enum ReloadState { CANNOT, SINGLE, CLIP }

    public class Magazine : MonoBehaviour
    {
        //this class will act as the magazine function for the firearms;
        //enabling reloading and ammo usage

        [Header("Attributes - Settings")]
        [SerializeField] private AmmoType ammoType;

        [SerializeField] private bool infiniteAmmo;
        [SerializeField] private bool infiniteClip;

        [Space]
        [SerializeField] private int maxMagazineSize; //our internal mag size (how many bullets we can hold at once)

        [Header("Attributes - Single Reloads")]
        [SerializeField] private bool singleReloads;
        [SerializeField] private float reloadTime;

        [Header("Attributes - Clip Reloads")]
        [SerializeField] private bool clipReloads; //i guess we're assuming single-bullet reloads are the norm?
        [SerializeField] private int clipReloadAmount; //how many bullets we reload in a clip
        [SerializeField] private float clipReloadTime;

        public int CurrentMagazineCount { get; private set; }
        public bool IsReloading { get; private set; } public bool InfinteAmmo { get; private set; } public bool InfinteClip { get; private set; }
        public AmmoType AmmoType { get; private set; }
        public ReloadState ReloadState { get; private set; }

        private void Awake() => Init();

        private void Init()
        {
            InfinteAmmo = infiniteAmmo;
            InfinteClip = infiniteClip;

            CurrentMagazineCount = maxMagazineSize;

            AmmoType = ammoType;
        }

        private bool CanUse(int amount)
        {
            if (amount > CurrentMagazineCount) return false;
            else return true;
        }

        private ReloadState CheckReloadState()
        {
            //if we have enough for a clip reload and enough space to insert a clip
            if (clipReloads && PlayerAmmo.HasAmmo(ammoType, clipReloadAmount)) return ReloadState.CLIP;

            //if we have at least 1 bullet of reserve ammo and we can fit a single bullet into the mag
            else if (singleReloads && PlayerAmmo.HasAmmo(ammoType) && (maxMagazineSize - CurrentMagazineCount) >= 1) return ReloadState.SINGLE;

            else return ReloadState.CANNOT;
        }

        public void CheckReload()
        {
            if (IsReloading) return;

            ReloadState = CheckReloadState();

            switch (ReloadState)
            {
                case ReloadState.CANNOT: Debug.Log("Cannot Reload!"); break;

                case ReloadState.SINGLE: if (singleReloads) StartSingleBulletReload(); break;

                case ReloadState.CLIP: if (clipReloads) StartClipReload(); else if (singleReloads) StartSingleBulletReload(); break;
            }
        }

        private void StartSingleBulletReload() { IsReloading = true; Invoke(nameof(SingleBulletReload), reloadTime); }

        private void SingleBulletReload()
        {
            //reload!
            SetCurrentMagazineCount(CurrentMagazineCount + 1);

            if (!infiniteAmmo) PlayerAmmo.RemoveAmmo(ammoType, 1);

            //if we still need to reload
            if (CurrentMagazineCount < maxMagazineSize) StartSingleBulletReload();

            else EndReload();
        }

        private void StartClipReload() { IsReloading = true; Invoke(nameof(ClipReload), clipReloadTime); }

        private void ClipReload()
        {
            //reload via a clip, losing the loaded bullets in the process!
            SetCurrentMagazineCount(clipReloadAmount);

            if (!infiniteAmmo) PlayerAmmo.RemoveAmmo(ammoType, clipReloadAmount);

            //if we still have ammo to reload - i wanna also add a check for doing another clip reload...
            if (CurrentMagazineCount < maxMagazineSize) StartSingleBulletReload();

            else EndReload();
        }

        private void EndReload() => IsReloading = false;

        //we can interrupt the reload to then i.e. fire straight after, or to start sprinting...
        public void InterruptReload() { CancelInvoke(nameof(SingleBulletReload)); CancelInvoke(nameof(ClipReload)); EndReload(); }

        private void SetCurrentMagazineCount(int amount) => CurrentMagazineCount = Mathf.Clamp(amount, 0, maxMagazineSize);

        public void DepleteAmmoFromMagazine(int amount)
        {
            CurrentMagazineCount -= amount;

            CurrentMagazineCount = Mathf.Clamp(CurrentMagazineCount, 0, maxMagazineSize);
        }
    }
}