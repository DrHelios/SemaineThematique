using System;
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
    struct AStarAgentJob : IJob
    {
        public SpaceInvadersGameState gs;

        public int playerId;

        [ReadOnly]
        public NativeArray<int> availableActions;

        public RandomAgent rdmAgent;

        [WriteOnly]
        public NativeArray<long> summedScores; 
        
        public void Execute()
        {
            var epochs = 5;
            var agent = rdmAgent;

            var gsCopy = Rules.Clone(ref gs);

            var nodeStart = new ANode
            {
                gsn = gsCopy,
                cost = 0
            };
            NativeList<ANode> listNode = new NativeList<ANode>(Allocator.Temp);
            for (int i = 0; i < availableActions.Length; i++)
            {
                listNode.Add(new ANode
                {
                    gsn = gsCopy,
                    cost = availableActions[i] ,
                });
            }
            for (var n = 0; n < epochs; n++)
            {
                Rules.CopyTo(ref gs, ref gsCopy);
                
                while (!gsCopy.isGameOver)
                {
                    var costNew = nodeStart.cost;
                    NativeList<ANode> nextListNode = new NativeList<ANode>(Allocator.Temp);
                    Rules.Step(ref gsCopy, agent.Act(ref gsCopy, availableActions, 1),agent.Act(ref gsCopy, availableActions, 2));
                    for (var i = 0; i < gs.projectiles.Length; i++)
                    {
                        var sqrDistance = (gs.projectiles[i].position - gs.playerPosition).sqrMagnitude;
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
                }
            }
        }
    }

    public int Act(ref SpaceInvadersGameState gs, NativeArray<int> availableActions, int plyId)
    {
        
        var job = new AStarAgentJob
        {
            availableActions = availableActions,
            gs = gs,
            summedScores = new NativeArray<long>(availableActions.Length, Allocator.TempJob),
            rdmAgent = new RandomAgent {rdm = new Random((uint) Time.frameCount)},
            playerId = plyId
        };

        var handle = job.Schedule();
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