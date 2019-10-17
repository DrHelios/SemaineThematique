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

        gs.gameShipSpeed = 0;
        gs.gameProjectileSpeed = 0;

        DefineGameSpeed(ApplicationData.IndexOfTypeOfChosenGameSpeed, ref gs);
    }

    public static void Step(ref SpaceInvadersGameState gs, int chosenPlayerAction, int chosenSecondPlayerInput)
    {
        if (gs.isGameOver)
        {
            throw new Exception("YOU SHOULD NOT TRY TO UPDATE GAME STATE WHEN GAME IS OVER !!!");
        }

        if (gs.currentGameStep == 40000)
            gs.isGameOver = true;

        UpdateEnemyPositions(ref gs);
        UpdateProjectiles(ref gs);
        HandleAgentInputs(ref gs, chosenPlayerAction);
        HandleSecondAgentInputs(ref gs, chosenSecondPlayerInput);
        HandleCollisions(ref gs);
        //HandleEnemyAtBottom(ref gs);
        gs.currentGameStep += 1;
    }

    static void DefineGameSpeed(int dropDownIndex, ref SpaceInvadersGameState gs)
    {
        switch (dropDownIndex)
        {
            case 0:
                gs.gameShipSpeed = SpaceInvadersGameState.playerSpeed * 1f;
                gs.gameProjectileSpeed = SpaceInvadersGameState.projectileSpeed * 1f;
                break;
            case 1:
                gs.gameShipSpeed = SpaceInvadersGameState.playerSpeed * 3f;
                gs.gameProjectileSpeed = SpaceInvadersGameState.projectileSpeed * 3f;
                break;
            case 2:
                gs.gameShipSpeed = SpaceInvadersGameState.playerSpeed * 6f;
                gs.gameProjectileSpeed = SpaceInvadersGameState.projectileSpeed * 6f;
                break;
            case 3:
                gs.gameShipSpeed = SpaceInvadersGameState.playerSpeed * 9f;
                gs.gameProjectileSpeed = SpaceInvadersGameState.projectileSpeed * 9f;
                break;
            case 4:
                gs.gameShipSpeed = SpaceInvadersGameState.playerSpeed * 12f;
                gs.gameProjectileSpeed = SpaceInvadersGameState.projectileSpeed * 12f;
                break;
            case 5:
                gs.gameShipSpeed = SpaceInvadersGameState.playerSpeed * 15f;
                gs.gameProjectileSpeed = SpaceInvadersGameState.projectileSpeed * 15f;
                break;
        }
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
                    gs.playerPosition += Vector2.up * gs.gameShipSpeed;
                }

                break;
            }
            case 2: // DOWN
            {
                if (gs.playerPosition.y > 0.5f)
                {
                    gs.playerPosition += Vector2.down * gs.gameShipSpeed;
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
                    speed = Vector2.right * gs.gameProjectileSpeed
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
                    gs.playerPosition2 += Vector2.up * gs.gameShipSpeed;
                }

                break;
            }
            case 6: // DOWN
            {
                if (gs.playerPosition2.y > 0.5f)
                {
                    gs.playerPosition2 += Vector2.down * gs.gameShipSpeed;
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
                    speed = Vector2.left * gs.gameProjectileSpeed
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
    
    public static long GetHashCode(ref SpaceInvadersGameState gs, int plyId)
    {
        var hash = 0L;
        if (plyId==1)
            hash = (long) math.round(math.clamp(gs.playerPosition.y, 0.50000001f, 8.4999999f) - 0.5);
        else if (plyId==2)
            hash = (long) math.round(math.clamp(gs.playerPosition2.y, 0.50000001f, 8.4999999f) - 0.5);


        var closestEnemyIndex = -1;
        var closestEnemyXPosition = -1f;
        var closestEnemyDistance = float.MaxValue;
        var closestEnemyYPosition = float.MaxValue;


        var distance= 0f;
        var enemyXPosition = 0f;
        if (plyId == 1)
        {
              enemyXPosition = gs.playerPosition.x;
              distance = math.abs(enemyXPosition - gs.playerPosition2.x);
        }

        else if (plyId == 2)
        {
             enemyXPosition = gs.playerPosition2.x;
             distance = math.abs(enemyXPosition - gs.playerPosition.x);
        }

        closestEnemyIndex = 0;


        if (closestEnemyIndex == -1)
        {
            return hash;
        }
        
        if (plyId == 1)
        {
            hash += 20+gs.currentGameStep * (long) math.round(math.clamp(gs.playerPosition2.y, 0.50000001f, 8.4999999f) - 0.5);

        }

        else if (plyId == 2)
        {
            hash += 20+gs.currentGameStep * (long) math.round(math.clamp(gs.playerPosition.y, 0.50000001f, 8.4999999f) - 0.5);

        }

//20+currentstep
        return hash;
    }
}