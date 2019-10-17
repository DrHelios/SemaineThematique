using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Rules = SpaceInvadersGameStateRules;

public class AStar : IAgent
{
    [BurstCompile]
    struct AStarJob : IJobParallelFor
    {
        public SpaceInvadersGameState gs;

        [ReadOnly]
        public NativeArray<int> availableActions;

        //public RandomAgent rdmAgent;

        [WriteOnly]
        public NativeArray<long> summedScores;

        public void Execute(int index)
        {
            /* var epochs = 100;
             var agent = rdmAgent;
 
             var gsCopy = Rules.Clone(ref gs);
 
             for (var n = 0; n < epochs; n++)
             {
                 Rules.CopyTo(ref gs, ref gsCopy);
                 Rules.Step(ref gsCopy, availableActions[index],availableActions[index]);
 
                 var currentDepth = 0;
                 while (!gsCopy.isGameOver)
                 {
                     Rules.Step(ref gsCopy, agent.Act(ref gsCopy, availableActions, 1),agent.Act(ref gsCopy, availableActions, 2));
                     currentDepth++;
                     if (currentDepth > 500)
                     {
                         break;
                     }
                 }
 
                 summedScores[index] += gsCopy.playerScore;
             }
         }*/
    }
  
    
}

    public int nodeOpen(NativeList<int> action)
    {
        int node = 0;
        node += int.MaxValue;
        foreach (var n in  action)
        {
            if (n < node)
            {
                node = n;
            }
        }

        action.RemoveAtSwapBack(action.IndexOf(node));
        return node;
    }
    
    public int Act(ref SpaceInvadersGameState gs, NativeArray<int> availableActions, int plyId)
    {
        int nodeDepart, nodeArrive, nodePre, ndOpen;
        NativeList<int> actionArray =  new NativeList<int>();
        NativeList<int> openNode = new NativeList<int>();
        for (int i = 0; i < availableActions.Length; i++)
        {
            i = Int32.MaxValue;
            nodePre = i; 
            openNode.Add(i);

        }

        nodeDepart = 0;
        ndOpen = nodeOpen(openNode);
       /* while (ndOpen != nodeArrive )
        {
            foreach (var n in openNode)
            {
                int cost = ndOpen +
                if (n > cost)
                {
                    n = cost;
                    nodePre = ndOpen;
                }
            }

            ndOpen = nodeOpen(openNode);
        }*/
       return 0;

    }
}
