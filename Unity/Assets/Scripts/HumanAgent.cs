using System;
using Unity.Collections;
using UnityEngine;

public class HumanAgent : IAgent
{
    public int Act(ref SpaceInvadersGameState gs, NativeArray<int> availableActions)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            return 3;
        }
        
        if (Input.GetKey(KeyCode.Z))
        {
            return 1;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            return 2;
        }

        return 0;
    }
}