using Unity.Collections;
using UnityEngine;

public interface IAgent
{
    int Act(ref SpaceInvadersGameState gs, NativeArray<int> availableActions, int plyId);
}

public struct RandomAgent : IAgent
{
    public Unity.Mathematics.Random rdm;
    public int Act(ref SpaceInvadersGameState gs, NativeArray<int> availableActions, int plyId)
    {
        rdm = new Unity.Mathematics.Random((uint)Random.Range(1, 100000));
        return availableActions[rdm.NextInt(0, availableActions.Length)];
    }
    
}

