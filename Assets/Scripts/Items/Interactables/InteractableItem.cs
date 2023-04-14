using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public abstract class InteractableItem : MonoBehaviour, IInteractable
    {
        public abstract void Interact();
        public abstract bool CanInteract();
    }
}