﻿using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rules = SpaceInvadersGameStateRules;

public class GameSystemScript : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public GameObject ProjectilePrefab;
    public GameObject ProjectilePlayer2Prefab;
    public GameObject PlayerPrefab;
    public GameObject PlayerPrefab2;
    public Text PlayerScore;
    public Text PlayerScore2;

    private SpaceInvadersGameState gs;

    private readonly List<Transform> enemiesView = new List<Transform>();
    private readonly List<Transform> projectilesView = new List<Transform>();
    private NativeArray<int> availableActions;
    private NativeArray<int> availableActions2;
    private Transform playerView;
    private Transform playerView2;
    private IAgent agent;
    private IAgent agent2;


    void Start()
    {
        Rules.Init(ref gs);

        playerView = Instantiate(PlayerPrefab).GetComponent<Transform>();
        playerView2 = Instantiate(PlayerPrefab2).GetComponent<Transform>();

        //agent = new RandomAgent {rdm = new Unity.Mathematics.Random((uint) Time.frameCount)};
        //agent2 = new RandomAgent {rdm = new Unity.Mathematics.Random((uint) Time.frameCount)};
        
        //agent = new RandomRolloutAgent();
        //agent2 = new RandomRolloutAgent();
        
        agent = new HumanAgent();
        agent2 = new HumanAgent();

        gs.sndPlayer = true;
        availableActions =
            new NativeArray<int>(SpaceInvadersGameStateRules.GetAvailableActions(ref gs), Allocator.Persistent);
        availableActions2 =
            new NativeArray<int>(SpaceInvadersGameStateRules.GetAvailableActions2(ref gs), Allocator.Persistent);
    }

    void Update()
    {
        if (gs.isGameOver)
        {
            if (gs.iaScore >= 3)
            {
                ApplicationData.gameOverText= "N°2 win";
            }
            if (gs.playerScore >= 3)
            {
                ApplicationData.gameOverText = "N°1 win";
            }
            SceneManager.LoadScene("GameOver");
            return;
        }

        SyncEnemyViews();
        SyncProjectileViews();
        playerView.position = gs.playerPosition;
        playerView2.position = gs.playerPosition2;
        PlayerScore.text = gs.playerScore.ToString();
        PlayerScore2.text = gs.iaScore.ToString();

        Rules.Step(ref gs, agent.Act(ref gs, availableActions, 1),agent2.Act(ref gs, availableActions2, 2));
    }

    private void SyncEnemyViews()
    {
        var enemiesToSpawn = gs.enemies.Length - enemiesView.Count;

        for (var i = 0; i < enemiesToSpawn; i++)
        {
            var enemyView = Instantiate(EnemyPrefab).GetComponent<Transform>();
            enemiesView.Add(enemyView);
        }

        for (var i = 0; i < -enemiesToSpawn; i++)
        {
            Destroy(enemiesView[enemiesView.Count - 1].gameObject);
            enemiesView.RemoveAt(enemiesView.Count - 1);
        }

        for (var i = 0; i < enemiesView.Count; i++)
        {
            enemiesView[i].position = gs.enemies[i].position;
        }
    }

    private void SyncProjectileViews()
    {
        var projectilesToSpawn = gs.projectiles.Length - projectilesView.Count;

        for (var i = 0; i < projectilesToSpawn; i++)
        {
            var projectileView = Instantiate(ProjectilePrefab).GetComponent<Transform>();
            projectilesView.Add(projectileView);
        }

        for (var i = 0; i < -projectilesToSpawn; i++)
        {
            Destroy(projectilesView[projectilesView.Count - 1].gameObject);
            projectilesView.RemoveAt(projectilesView.Count - 1);
        }

        for (var i = 0; i < projectilesView.Count; i++)
        {
            projectilesView[i].position = gs.projectiles[i].position;
        }
    }

    private void OnDestroy()
    {
        gs.enemies.Dispose();
        gs.projectiles.Dispose();
        availableActions.Dispose();
    }
}