using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct GameStateScr
{
    public const float enemyRadius = 0.5f;
    public const float enemySpeed = 0.1f / 60f * 10f;

    public const float playerRadius = 0.5f;
    public const float playerSpeed = 0.3f / 60f * 10f;
    public const long shootDelay = 30;

    public const float bulletRadius = 0.1f / 60f * 10f;
    public const float bulletSpeed = 0.5f / 60f * 10f;
        
    public NativeList<BotStruc> enemies;
    public NativeList<BulletStruc> bullets;
    public Vector2 playerPos;
    public long lastShootStep;
    public long currentGameStep;
    public bool isGameOver;
    public long playerScore;
    
    /*//Ia const parameter
    public const float IaSpeed = 0.1f / 60f * 10f;
    public const float IaRadius = 0.5f;
    public const float laserIaRadius = 0.1f / 60f * 10f;
    public const float laserIaSpeed = 0.5f / 60f * 10f;

    // player const parameter
    public const float playerSpeed = 0.3f / 60f * 10f;
    public const float playerRadius = 0.5f;
    public const float laserPlayerRadius = 0.1f / 60f * 10f;
    public const float laserPlayerSpeed = 0.5f / 60f * 10f;
    public const long shootDelay = 30;

    public NativeList<Laser> LaserIa;
    public long IaScore;

    public NativeList<Laser> laserPlayer;
    public Vector2 playerPosition;
    public long playerScore;

    public long lastShootStep;
    public long currentGS;
    public bool isGameOver;*/
}
