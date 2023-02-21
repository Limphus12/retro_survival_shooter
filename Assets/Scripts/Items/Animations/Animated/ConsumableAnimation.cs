using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ConsumableAnimation : ItemAnimation
    {
        const string CONSUMABLE_CONSUME = "consumable_consume";
        const string CONSUMABLE_CONSUMING = "consumable_consuming";

        public void PlayConsumableConsume() => PlayAnimation(CONSUMABLE_CONSUME);
        public void PlayConsumableConsuming() => PlayAnimation(CONSUMABLE_CONSUMING);
    }
}