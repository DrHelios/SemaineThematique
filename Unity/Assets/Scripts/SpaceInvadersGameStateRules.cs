using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class SpaceInvadersGameStateRules
{
    public static void Init(ref SpaceInvadersGameState gs)
    {
        gs.enemies = new NativeList<Enemy>(10, Allocator.Persistent);
        gs.projectiles = new NativeList<Projectile>(100, Allocator.Persistent);
        gs.playerPosition = new Vector2(-8f, 5f);
        gs.playerPosition2 = new Vector2(8f, 5f);
        gs.isGameOver = false;
        gs.lastShootStep = -SpaceInvadersGameState.shootDelay;
        gs.lastShootStep2 = -SpaceInvadersGameState.shootDelay;

        gs.playerScore = 0;
        gs.iaScore = 0;
    }


    public static void Step(ref SpaceInvadersGameState gs, int chosenPlayerAction, int chosenSecondPlayerInput)
    {
        if (gs.isGameOver)
        {
            throw new Exception("YOU SHOULD NOT TRY TO UPDATE GAME STATE WHEN GAME IS OVER !!!");
        }

        UpdateEnemyPositions(ref gs);
        UpdateProjectiles(ref gs);
        HandleAgentInputs(ref gs, chosenPlayerAction);
        HandleSecondAgentInputs(ref gs, chosenSecondPlayerInput);
        HandleCollisions(ref gs);
        //HandleEnemyAtBottom(ref gs);
        gs.currentGameStep += 1;
    }

    static void UpdateEnemyPositions(ref SpaceInvadersGameState gs)
    {
        for (var i = 0; i < gs.enemies.Length; i++)
        {
            var enemy = gs.enemies[i];
            enemy.position += gs.enemies[i].speed;
            gs.enemies[i] = enemy;
        }
    }

    static void UpdateProjectiles(ref SpaceInvadersGameState gs)
    {
        for (var i = 0; i < gs.projectiles.Length; i++)
        {
            var projectile = gs.projectiles[i];
            projectile.position += gs.projectiles[i].speed;
            gs.projectiles[i] = projectile;
        }
    }

    static void HandleCollisions(ref SpaceInvadersGameState gs)
    {
        for (var i = 0; i < gs.projectiles.Length; i++)
        {
            if (gs.projectiles[i].position.x > 10 || gs.projectiles[i].position.x < -10)
            {
                gs.projectiles.RemoveAtSwapBack(i);
                i--;
                continue;
            }
            
            var sqrDistance = (gs.projectiles[i].position - gs.playerPosition).sqrMagnitude;
            var sqrDistancePly2 = (gs.projectiles[i].position - gs.playerPosition2).sqrMagnitude;

            if (sqrDistance
                  <= Mathf.Pow(SpaceInvadersGameState.projectileRadius + SpaceInvadersGameState.playerRadius,
                      2))
            {
                gs.iaScore += 1;
                gs.projectiles.RemoveAtSwapBack(i);
                i--;
                if(gs.iaScore >= 3)
                {
                    gs.isGameOver = true;
                }
                break;
            }
            
            if (sqrDistancePly2
                <= Mathf.Pow(SpaceInvadersGameState.projectileRadius + SpaceInvadersGameState.playerRadius,
                    2))
            {
                gs.playerScore += 1;
                gs.projectiles.RemoveAtSwapBack(i);
                i--;
                if(gs.playerScore >= 3)
                {
                    gs.isGameOver = true;
                }
                break;
            }
        }


    }

    static void HandleAgentInputs(ref SpaceInvadersGameState gs, int chosenPlayerAction)
    {
        switch (chosenPlayerAction)
        {
            case 0: // IDLE
                return;
            case 1: // UP
            {
                if (gs.playerPosition.y < 8.5f)
                {
                    gs.playerPosition += Vector2.up * SpaceInvadersGameState.enemySpeed * 4;
                }

                break;
            }
            case 2: // DOWN
            {
                if (gs.playerPosition.y > 0.5f)
                {
                    gs.playerPosition += Vector2.down * SpaceInvadersGameState.enemySpeed * 4;
                }

                break;
            }
            case 3: // SHOOT
            {
                if (gs.currentGameStep - gs.lastShootStep < SpaceInvadersGameState.shootDelay)
                {
                    break;
                }

                gs.lastShootStep = gs.currentGameStep;
                //gs.lastShootStep2 = gs.currentGameStep;

                gs.projectiles.Add(new Projectile
                {
                    position = gs.playerPosition + Vector2.right * 1.3f,
                    speed = Vector2.right * SpaceInvadersGameState.projectileSpeed
                });
                break;
            }
        }
    }

    static void HandleSecondAgentInputs(ref SpaceInvadersGameState gs, int chosenPlayerAction2)
    {
        switch (chosenPlayerAction2)
        {
            case 0: // DO NOTHING
                return;
            case 5: // UP
            {
                if (gs.playerPosition2.y < 8.5f)
                {
                    gs.playerPosition2 += Vector2.up * SpaceInvadersGameState.enemySpeed * 4;
                }

                break;
            }
            case 6: // DOWN
            {
                if (gs.playerPosition2.y > 0.5f)
                {
                    gs.playerPosition2 += Vector2.down * SpaceInvadersGameState.enemySpeed * 4;
                }

                break;
            }
            case 7: // SHOOT
            {
                if (gs.currentGameStep - gs.lastShootStep2 < SpaceInvadersGameState.shootDelay)
                {
                    break;
                } 
                // gs.lastShootStep = gs.currentGameStep;
                gs.lastShootStep2 = gs.currentGameStep;

                gs.projectiles.Add(new Projectile
                {
                    position = gs.playerPosition2 + Vector2.left * 1.3f,
                    speed = Vector2.left * SpaceInvadersGameState.projectileSpeed
                });
                break;
            }
        }
    }
    
    /*static void HandleEnemyAtBottom(ref SpaceInvadersGameState gs)
    {
        for (var i = 0; i < gs.enemies.Length; i++)
        {
            if (gs.enemies[i].position.y >= 0)
            {
                continue;
            }

            gs.isGameOver = true;
            return;
        }
    }*/

    private static readonly int[] AvailableActions = new[]
    {
        0, 1, 2, 3,
    };
    
    private static readonly int[] AvailableActions2 = new[]
    {
        0, 5, 6, 7,
    };

    public static int[] GetAvailableActions(ref SpaceInvadersGameState gs)
    {
        return AvailableActions;
    }
    
    public static int[] GetAvailableActions2(ref SpaceInvadersGameState gs)
    {
        return AvailableActions2;
    }

    public static SpaceInvadersGameState Clone(ref SpaceInvadersGameState gs)
    {
        var gsCopy = new SpaceInvadersGameState();
        gsCopy.enemies = new NativeList<Enemy>(gs.enemies.Length, Allocator.Temp);
        gsCopy.enemies.AddRange(gs.enemies);
        gsCopy.projectiles = new NativeList<Projectile>(gs.projectiles.Length, Allocator.Temp);
        gsCopy.projectiles.AddRange(gs.projectiles);
        gsCopy.playerPosition = gs.playerPosition;
        gsCopy.playerPosition2 = gs.playerPosition2;
        gsCopy.currentGameStep = gs.currentGameStep;
        gsCopy.isGameOver = gs.isGameOver;
        gsCopy.lastShootStep = gs.lastShootStep;
        gsCopy.lastShootStep2 = gs.lastShootStep2;
        gsCopy.playerScore = gs.playerScore;
        gsCopy.iaScore = gs.iaScore;

        return gsCopy;
    }
    

    public static void CopyTo(ref SpaceInvadersGameState gs, ref SpaceInvadersGameState gsCopy)
    {
        gsCopy.enemies.Clear();
        gsCopy.enemies.AddRange(gs.enemies);
        gsCopy.projectiles.Clear();
        gsCopy.projectiles.AddRange(gs.projectiles);
        gsCopy.playerPosition = gs.playerPosition;
        gsCopy.playerPosition2 = gs.playerPosition2;
        gsCopy.currentGameStep = gs.currentGameStep;
        gsCopy.lastShootStep = gs.lastShootStep;
        gsCopy.lastShootStep2 = gs.lastShootStep2;
        gsCopy.isGameOver = gs.isGameOver;
        gsCopy.playerScore = gs.playerScore;
        gsCopy.iaScore = gs.iaScore;
    }
    
    public static long GetHashCode(ref SpaceInvadersGameState gs)
    {
        var hash = 0L;
        hash = (long) math.round(math.clamp(gs.playerPosition.x, -4.49999f, 4.49999f) + 4.5);

        var closestEnemyIndex = -1;
        var closestEnemyXPosition = -1f;
        var closestEnemyDistance = float.MaxValue;
        var closestEnemyYPosition = float.MaxValue;

        for (var i = 0; i < gs.enemies.Length; i++)
        {
            var enemyXPosition = gs.enemies[i].position.x;
            var distance = math.abs(enemyXPosition - gs.playerPosition.x);

            if (gs.enemies[i].position.y < closestEnemyYPosition
                || Math.Abs(gs.enemies[i].position.y - closestEnemyYPosition) < 0.000001f
                && distance < closestEnemyDistance)
            {
                closestEnemyIndex = i;
                closestEnemyXPosition = enemyXPosition;
                closestEnemyDistance = distance;
                closestEnemyYPosition = gs.enemies[i].position.y;
            }
        }

        if (closestEnemyIndex == -1)
        {
            return hash;
        }
        
        hash += 10 + (long) math.round(math.clamp(closestEnemyXPosition, -4.49999f, 4.49999f) + 4.5);

        return hash;
    }
}