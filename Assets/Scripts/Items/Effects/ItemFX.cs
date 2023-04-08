using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ItemFX : MonoBehaviour
    {
        protected void PlayFX(GameObject fx, Transform transform, float time)
        {
            GameObject gameObject = Instantiate(fx, transform.position, transform.rotation);

            Destroy(gameObject, time);
        }
    }
}