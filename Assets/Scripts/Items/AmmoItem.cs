using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AmmoItem : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            NewPlayerInventory npi = other.GetComponent<NewPlayerInventory>();

            if (npi)
            {
                GameObject item = npi.GetCurrentHandItem();

                if (item)
                {
                    NewFirearm newFirearm = item.GetComponent<NewFirearm>();

                    if (newFirearm)
                    {
                        newFirearm.SetAmmoCount(newFirearm.GetMaxAmmoCount() + newFirearm.GetAmmoCount());

                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
