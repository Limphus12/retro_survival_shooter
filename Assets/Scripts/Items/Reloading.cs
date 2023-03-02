using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Reloading : MonoBehaviour
    {
        //this class will act as the reloading function for the firearms.

        [Header("Attributes - Reloading")]
        [SerializeField] private float reloadTime;

        [Space]
        [SerializeField] private bool clipReloads; //i guess we're assuming single-bullet reloads are the norm?
        [SerializeField] private int clipReloadAmount; //how many bullets we reload in a clip
        [SerializeField] private float clipReloadTime;

        [Space]
        [SerializeField] private int maxMagazineSize; //our internal mag size (how many bullets we can hold at once)
        private int currentMagazineCount; //the current amount of bullets we have in our mag.

        [Space]
        [SerializeField] private int maxAmmoReserves; //our maximum ammo reserves for this weapon
        private int currentAmmoReserves; //our current ammo reserves

        //what are the types of reloading we can do?

        //single bullet reloads - for revolvers i.e. nagant

        //clip reloads - a special behaviour that allows for a full top-up of ammo
        //ususally when dry
        //examples (from H:S) - scottfield swift, mosin nagant, winfield swift, bornheim.

        //magazine - uses magazines to reload
        //i dont think there's any guns in H:S that have proper box mags?

        //hmm what if we had mags as consumables? that'd be interesting lol.


        //we're gonna try this syntax
        public bool IsReloading { get; private set; }

        void StartReload()
        {
            IsReloading = true;

            if (clipReloads) Invoke(nameof(ClipReload), clipReloadTime);

            else Invoke(nameof(Reload), reloadTime);
        }

        void Reload()
        {
            //reload!
            currentMagazineCount++;


            //we'd have to do some logic here to determine if we need to keep on reloading

            //actually, we could just check if our internal mag is full?
            //if not, just cycle back to the start reload.
            if (currentMagazineCount < maxMagazineSize)
            {
                //dont end reloading
            }

            else EndReload();

            




        }

        void ClipReload()
        {
            //reload via a clip!
            currentMagazineCount += clipReloadAmount;

            //check if we need to keep on reloading
            //i.e. the bornheim reloads a 5 mag clip, then tops up an extra bullet in the top.
            //so we cant just end the reload.



            EndReload();
        }

        void EndReload()
        {
            IsReloading = false;
        }

        //we can interrupt the reload to then fire straight after or something
        //or to start sprinting
        public void InterruptReload()
        {
            CancelInvoke(nameof(Reload));
            CancelInvoke(nameof(ClipReload));

            EndReload();
        }
    }
}