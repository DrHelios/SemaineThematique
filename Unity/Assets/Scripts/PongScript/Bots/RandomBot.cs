using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBot : IBot
{
    public int Act(ref GameStateScr gs, int[] usableActions)
    {
        return usableActions[Random.Range(0, usableActions.Length)];
    }
}
