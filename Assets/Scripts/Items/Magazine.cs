using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Magazine : MonoBehaviour
    {
        //this class will act as the magazine function for the firearms;
        //enabling reloading and ammo usage

        [Header("Attributes - Reloading")]
        [SerializeField] private bool infiniteAmmo;
        [SerializeField] private bool infiniteClip;

        public bool InfinteAmmo { get; private set; }
        public bool InfinteClip { get; private set; }

        [SerializeField] private float reloadTime;

        [Space]
        [SerializeField] private bool clipReloads; //i guess we're assuming single-bullet reloads are the norm?
        [SerializeField] private int clipReloadAmount; //how many bullets we reload in a clip
        [SerializeField] private float clipReloadTime;

        [Space]
        [SerializeField] private int maxMagazineSize; //our internal mag size (how many bullets we can hold at once)
        public int CurrentMagazineCount { get; private set; } //the current amount of bullets we have in our mag.

        [Space]
        [SerializeField] private int maxAmmoReserves; //our maximum ammo reserves for this weapon
        public int CurrentAmmoReserves { get; private set; } //our current ammo reserves

        public bool IsReloading { get; private set; } //we're gonna try this syntax

        private void Awake() => Init();

        private void Init()
        {
            InfinteAmmo = infiniteAmmo;
            InfinteClip = infiniteClip;

            CurrentMagazineCount = maxMagazineSize;
            CurrentAmmoReserves = maxAmmoReserves;
        }

        //a method to compare ammo reserves to maxMagazineSize
        private int CheckAmmoReserves(int amount)
        {
            if (CurrentAmmoReserves <= 0) return 0; //if we have no reserve ammo

            else if (CurrentAmmoReserves < amount) return 1; //if we dont have enough to perform a reload with the amount required

            else if (CurrentAmmoReserves >= amount) return 2; //if we have enough to perform a reload with the amount required

            else return 3;
        }

        public int CheckMagazine()
        {
            if (CurrentMagazineCount <= 0) return 0; //if we have no ammo loaded
            if (CurrentMagazineCount > 0) return 1; //if we have ammo loaded
            else return 2;
        }

        public int CheckReload()
        {
            //KEY - 2 = Clip Reload, 1 = Single Reload, 0 = No Reload

            if (clipReloads)
            {
                //if we have enough ammo in our reserves for a clip reload
                if (CheckAmmoReserves(clipReloadAmount) == 2)
                {
                    //if we have enough mag space
                    if ((maxMagazineSize - CurrentMagazineCount) >= clipReloadAmount) return 2;

                    //if we don't have enough space for a clip, but we have enough for a single bullet
                    else if ((maxMagazineSize - CurrentMagazineCount) >= 1) return 1;

                    //else we cannot reload!
                    else return 0;
                }

                //if we only have enough for a single bullet reload
                else if (CheckAmmoReserves(1) == 2 && ((maxMagazineSize - CurrentMagazineCount) <= 1)) return 1;

                //else we cannot reload!
                else return 0;
            }

            //if we are not doing clip reloads, we have enough ammo for a single bullet reload, and we have the mag space
            else if (!clipReloads && CheckAmmoReserves(1) == 2 && ((maxMagazineSize - CurrentMagazineCount) <= 1)) return 1;

            //else we cannot reload!
            else return 0;
        }

        public void StartReload()
        {
            Debug.Log("Started Single Bullet Reload");

            IsReloading = true; Invoke(nameof(Reload), reloadTime);
        }

        public void StartClipReload()
        {
            Debug.Log("Started Clip Reload");

            IsReloading = true; Invoke(nameof(ClipReload), clipReloadTime);
        }

        void Reload()
        {
            //reload!
            SetCurrentMagazineCount(CurrentMagazineCount += 1);

            //if we still need to reload
            if (CurrentMagazineCount < maxMagazineSize) StartReload();
            
            else EndReload();
        }

        void ClipReload()
        {
            //reload via a clip!

            //if we're not using infinite ammo, reload by the clip amount
            if (!InfinteAmmo)
            {
                SetCurrentMagazineCount(CurrentMagazineCount += clipReloadAmount);
                SetCurrentAmmoReserves(CurrentAmmoReserves -= clipReloadAmount);
            }

            else SetCurrentMagazineCount(maxMagazineSize);

            //if we still have ammo to reload
            if (CurrentMagazineCount < maxMagazineSize) StartReload();

            else EndReload();
        }

        void EndReload() => IsReloading = false;

        //we can interrupt the reload to then fire straight after or something
        //or to start sprinting
        public void InterruptReload()
        {
            CancelInvoke(nameof(Reload)); CancelInvoke(nameof(ClipReload));

            EndReload();
        }

        private void SetCurrentMagazineCount(int amount) => CurrentMagazineCount = Mathf.Clamp(amount, 0, maxMagazineSize);
        private void SetCurrentAmmoReserves(int amount) => CurrentAmmoReserves = Mathf.Clamp(amount, 0, maxAmmoReserves);
        public void UseAmmo(int amount)
        {
            if (!InfinteClip) SetCurrentMagazineCount(CurrentMagazineCount -= amount);
        }
    }
}