using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class GameStateRules
{
    public static void Init(ref GameStateScr gs)
    {
        gs.enemies = new NativeList<BotStruc>(10, Allocator.Persistent);
        for (var i = 0; i < 10; i++)
        {
            var enemy = new BotStruc
            {
                pos = new Vector2(i - 4.5f, 9f),
                speed = new Vector2(0f, -GameStateScr.enemySpeed)
            };
            gs.enemies.Add(enemy);
        }

        gs.bullets = new NativeList<BulletStruc>(100, Allocator.Persistent);
        gs.playerPos = new Vector2(0f, 0f);
        gs.isGameOver = false;
        gs.lastShootStep = -GameStateScr.shootDelay;
        gs.playerScore = 0;
    }


    public static void Step(ref GameStateScr gs, int chosenPlayerAction)
    {
        if (gs.isGameOver)
        {
            throw new Exception("Game Over");
        }

        UpdateEnemyPositions(ref gs);
        UpdateProjectiles(ref gs);
        HandleAgentInputs(ref gs, chosenPlayerAction);
        HandleCollisions(ref gs);
        //HandleEnemyAtBottom(ref gs);
        gs.currentGameStep += 1;
    }

    static void UpdateEnemyPositions(ref GameStateScr gs)
    {
        for (var i = 0; i < gs.enemies.Length; i++)
        {
            var enemy = gs.enemies[i];
            enemy.pos += gs.enemies[i].speed;
            gs.enemies[i] = enemy;
        }
    }

    static void UpdateProjectiles(ref GameStateScr gs)
    {
        for (var i = 0; i < gs.bullets.Length; i++)
        {
            var projectile = gs.bullets[i];
            projectile.pos += gs.bullets[i].speed;
            gs.bullets[i] = projectile;
        }
    }

    static void HandleCollisions(ref GameStateScr gs)
    {
        for (var i = 0; i < gs.bullets.Length; i++)
        {
            var sqrDistance = (gs.bullets[i].pos - gs.playerPos).sqrMagnitude;

            if (!(sqrDistance
                  <= Mathf.Pow(GameStateScr.bulletRadius + GameStateScr.playerRadius,
                      2)))
            {
                continue;
            }

            gs.isGameOver = true;
            return;
        }

        for (var i = 0; i < gs.bullets.Length; i++)
        {
            if (gs.bullets[i].pos.y > 10)
            {
                gs.bullets.RemoveAtSwapBack(i);
                i--;
                continue;
            }

            for (var j = 0; j < gs.enemies.Length; j++)
            {
                var sqrDistance = (gs.bullets[i].pos - gs.enemies[j].pos).sqrMagnitude;

                if (!(sqrDistance
                      <= Mathf.Pow(SpaceInvadersGameState.projectileRadius + SpaceInvadersGameState.enemyRadius,
                          2)))
                {
                    continue;
                }

                gs.bullets.RemoveAtSwapBack(i);
                i--;
                gs.enemies.RemoveAtSwapBack(j);
                j--;
                gs.playerScore += 100;
                break;
            }
        }

        if (gs.enemies.Length == 0)
        {
            gs.isGameOver = true;
        }
    }

    static void HandleAgentInputs(ref GameStateScr gs, int chosenPlayerAction)
    {
        switch (chosenPlayerAction)
        {
            case 0: // IDLE
                return;
            case 1: // UP
            {
                gs.playerPos += Vector2.up * GameStateScr.playerSpeed;
                break;
            }
            case 2: // DOWN
            {
                gs.playerPos += Vector2.down * GameStateScr.playerSpeed;
                break;
            }
            case 3: // SHOOT
            {
                if (gs.currentGameStep - gs.lastShootStep < GameStateScr.shootDelay)
                {
                    break;
                }

                gs.lastShootStep = gs.currentGameStep;
                gs.bullets.Add(new BulletStruc
                {
                    pos = gs.playerPos + Vector2.up * 1.3f,
                    speed = Vector2.up * GameStateScr.bulletSpeed
                });
                break;
            }
        }
    }

    /*static void HandleEnemyAtBottom(ref GameStateScr gs)
    {
        for (var i = 0; i < gs.enemies.Length; i++)
        {
            if (gs.enemies[i].pos.y >= 0)
            {
                continue;
            }

            gs.isGameOver = true;
            return;
        }
    }*/

    private static readonly int[] AvailableActions = new[]
    {
        0, 1, 2, 3
    };

    public static int[] GetAvailableActions(ref GameStateScr gs)
    {
        return AvailableActions;
    }

    public static GameStateScr Clone(ref GameStateScr gs)
    {
        var gsCopy = new GameStateScr();
        gsCopy.enemies = new NativeList<BotStruc>(gs.enemies.Length, Allocator.Temp);
        gsCopy.enemies.AddRange(gs.enemies);
        gsCopy.bullets = new NativeList<BulletStruc>(gs.bullets.Length, Allocator.Temp);
        gsCopy.bullets.AddRange(gs.bullets);
        gsCopy.playerPos = gs.playerPos;
        gsCopy.currentGameStep = gs.currentGameStep;
        gsCopy.isGameOver = gs.isGameOver;
        gsCopy.lastShootStep = gs.lastShootStep;

        return gsCopy;
    }
    public static void CopyTo(ref GameStateScr gs, ref SpaceInvadersGameState gsCopy)
    {
        gsCopy.enemies.Clear();
        gsCopy.enemies.AddRange(gs.enemies);
        gsCopy.projectiles.Clear();
        gsCopy.projectiles.AddRange(gs.projectiles);
        gsCopy.playerPosition = gs.playerPosition;
        gsCopy.currentGameStep = gs.currentGameStep;
        gsCopy.lastShootStep = gs.lastShootStep;
        gsCopy.isGameOver = gs.isGameOver;
        gsCopy.playerScore = gs.playerScore;
    }
}
