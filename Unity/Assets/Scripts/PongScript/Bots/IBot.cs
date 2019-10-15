using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBot
{
    int Act(ref GameStateScr gs, int[] usableActions);
}
