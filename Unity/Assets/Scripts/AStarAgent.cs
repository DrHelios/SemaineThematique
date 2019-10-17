using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Rules = SpaceInvadersGameStateRules;
public struct ANode
{
public int cost;
public SpaceInvadersGameState gsn;
}
public class AStarAgent : IAgent
{
    [BurstCompile]
    struct AStarJob : IJobParallelFor
    {
        public SpaceInvadersGameState gs;
        
        [ReadOnly]
        public NativeArray<int> availableActions;

        public RandomAgent rdmAgent;

        [WriteOnly]
        public NativeArray<long> summedScores;

        public void Execute(int index)
        {
             var epochs = 100;
             var agent = rdmAgent;
 
             var gsCopy = Rules.Clone(ref gs);

             var nodeStart = new ANode
             {
                 gsn = gs,
                 cost = 0
             };
             NativeList<ANode> listNode = new NativeList<ANode>();
             for (int i = 0; i < availableActions.Length; i++)
             {
                 listNode.Add(new ANode
                 {
                     gsn = gs,
                     cost = availableActions[i] ,
                 });
             }

             for (var n = 0; n < epochs; n++)
             {
                 Rules.CopyTo(ref gs, ref gsCopy);
                 Rules.Step(ref gsCopy, availableActions[index],availableActions[index + 1]);
                 
                 var currentDepth = 0;
                 while (!gsCopy.isGameOver)
                 {
                     var costNew = nodeStart.cost;
                     NativeList<ANode> nextListNode = new NativeList<ANode>();
                     Rules.Step(ref gsCopy, agent.Act(ref gsCopy, availableActions, 1),agent.Act(ref gsCopy, availableActions, 2));
                     for (var i = 0; i < gs.projectiles.Length; i++)
                     {
                         var sqrDistance = (gs.projectiles[i].position - gs.playerPosition).sqrMagnitude;
                         //var sqrDistancePly2 = (gs.projectiles[i].position - gs.playerPosition2).sqrMagnitude;
                         for (var j = 0; j < listNode.Length; j++)
                         {
                             if (listNode[j].cost == nodeStart.cost )
                             {
                                 break;
                             }
                             costNew += (costNew + listNode[j].cost + (Int32)(sqrDistance / (SpaceInvadersGameState.projectileSpeed) * (3 - gs.iaScore)));
                             ANode nodeNext = new ANode
                             {
                                 gsn = gs,
                                 cost = costNew
                             };
                             for (int l = 0; l < j; l++)
                             {
                                nextListNode.Add(nodeNext);
                             }
                         }
                         int minInt = nextListNode[1].cost;
                         for (var a = 0; a < nextListNode.Length; a++)
                         {
                             if (nextListNode[a].cost < minInt)
                             {
                                 minInt = nextListNode[a].cost;
                             }
                         }

                         nodeStart = new ANode
                         {
                            gsn = gs,
                            cost = minInt
                         };
                         nextListNode.Clear();
                     }
                     if (currentDepth > 500)
                     {
                         break;
                     }
                 }
                 summedScores[index] += gsCopy.playerScore;
                 summedScores[index + 1] += gsCopy.iaScore;
             }
        }
    }


    public int Act(ref SpaceInvadersGameState gs, NativeArray<int> availableActions, int plyId)
    {
        //throw new NotImplementedException();
        var job = new AStarJob()
        {
            availableActions = availableActions,
            gs = gs,
            summedScores = new NativeArray<long>(availableActions.Length, Allocator.TempJob),
            rdmAgent = new RandomAgent {rdm = new Random((uint) Time.frameCount)}
        };

        var handle = job.Schedule(availableActions.Length, 1);
        handle.Complete();

        var bestActionIndex = -1;
        var bestScore = long.MinValue;
        for (var i = 0; i < job.summedScores.Length; i++)
        {
            if (bestScore > job.summedScores[i])
            {
                continue;
            }

            bestScore = job.summedScores[i];
            bestActionIndex = i;
        }

        var chosenAction = availableActions[bestActionIndex];

        job.summedScores.Dispose();
        return chosenAction;
    }
}
