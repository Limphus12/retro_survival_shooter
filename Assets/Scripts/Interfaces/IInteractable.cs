using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public interface IInteractable
    {
        public void Interact();
        public bool CanInteract();
    }
}