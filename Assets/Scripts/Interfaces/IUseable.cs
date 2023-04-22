using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseable
{
    void CheckInput();
    bool DetectKeyboardInput(KeyCode key);
    bool DetectMouseInput(int button);
}
