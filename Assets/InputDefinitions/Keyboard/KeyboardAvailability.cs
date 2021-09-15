using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAvailability : InputAvailability
{
    protected override void Start() => base.Start();

    // Nothing really needs to happen here, we assume that the keyboard is always available
    // TODO: Might want to do something here to support consoles and other non-PC platforms
    protected override bool CheckAvailability() => true;
}
