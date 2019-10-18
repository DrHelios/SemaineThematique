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
public int action;
public SpaceInvadersGameState gsn;
}
public class AStarAgent : IAgent
{
    [BurstCompile]
    struct AStarAgentJob : IJob
    {
        public SpaceInvadersGameState gs;

        public int playerId;

        [ReadOnly] public NativeArray<int> availableActions;

        public RandomAgent rdmAgent;

        [WriteOnly] public NativeArray<long> summedScores;

        public void Execute()
        {
            var epochs = 5;
            var agent = rdmAgent;

            var gsCopy = Rules.Clone(ref gs);
            var rootHash = Rules.GetHashCode(ref gsCopy, playerId);

            // CREATION DE LA MEMOIRE (Arbre)
            var memory = new NativeHashMap<long, NativeList<ANode>>(2048, Allocator.Temp); // FOR BURSTCOMPILE

            memory.TryAdd(rootHash, new NativeList<ANode>(availableActions.Length, Allocator.Temp));

            var nodeStart = new ANode
            {
                gsn = gsCopy,
                cost = 0,
                action = 0
            };

            for (int i = 0; i < availableActions.Length; i++)
            {
                memory[rootHash]
                    .Add(new ANode
                    {
                        gsn = gsCopy,
                        cost = availableActions[i],
                        action = availableActions[i]
                    });
            }

            for (var n = 0; n < epochs; n++)
            {
                Rules.CopyTo(ref gs, ref gsCopy);
                var currentHash = rootHash;

                while (!gsCopy.isGameOver)
                {
                    var hasUnexploredNodes = false;
                    var costNew = nodeStart.cost;

                    for (var i = 0; i < memory[currentHash].Length; i++)
                    {
                        if (memory[currentHash][i].cost == nodeStart.cost)
                        {
                            hasUnexploredNodes = true;
                            break;
                        }
                    }

                    if (hasUnexploredNodes)
                    {
                        break;
                    }

                    var minNodeIndex = -1;
                    var minNodeCost = int.MinValue;

                    for (var i = 0; i < gs.projectiles.Length; i++)
                    {
                        var sqrDistance = (gs.projectiles[i].position - gs.playerPosition).sqrMagnitude;
                        for (var j = 0; j < memory[currentHash].Length; j++)
                        {
                            var list = memory[currentHash];
                            var node = list[j];
                            var euristique = (Int32) (sqrDistance / (SpaceInvadersGameState.projectileSpeed) *
                                                      (3 - gsCopy.iaScore));
                            costNew += memory[currentHash][j].cost + euristique;
                            if (costNew >= minNodeCost)
                            {
                                minNodeIndex = i;
                                minNodeCost = costNew;
                            }
                        }
                    }

                    Rules.Step(ref gsCopy, memory[currentHash][minNodeIndex].action, 0);
                    currentHash = Rules.GetHashCode(ref gsCopy, playerId);
                }
            }

            for (var i = 0; i < memory[rootHash].Length; i++)
            {
                summedScores[i] = memory[rootHash][i].cost;
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