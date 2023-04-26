using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public interface IUseable
    {
        void CheckInput();
        bool DetectKeyboardInput(KeyCode key);
        bool DetectMouseInput(int button);
    }
}