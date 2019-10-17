using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rules = SpaceInvadersGameStateRules;

public class GameSystemScript : MonoBehaviour
{
    // Pas utilisé
    [Header("Enemy prefab")]
    public GameObject EnemyPrefab;

    [Header("Player 1 prefab")]
    public GameObject Player1Prefab;
    public GameObject Player1ProjectilePrefab;
    public Text Player1Score;

    [Header("Player 2 prefab")]
    public GameObject Player2Prefab;
    public GameObject Player2ProjectilePrefab;
    public Text Player2Score;

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

        playerView = Instantiate(Player1Prefab).GetComponent<Transform>();
        playerView2 = Instantiate(Player2Prefab).GetComponent<Transform>();

        //agent = new RandomAgent {rdm = new Unity.Mathematics.Random((uint) Time.frameCount)};
        //agent2 = new RandomAgent {rdm = new Unity.Mathematics.Random((uint) Time.frameCount)};
        
        //agent = new RandomRolloutAgent();
        //agent2 = new RandomRolloutAgent();
        
        //agent = new HumanAgent();
        //agent2 = new HumanAgent();
        
        DefineAgent1(ApplicationData.IndexOfTypeOfChosenAgent);
        DefineAgent2(ApplicationData.IndexOfTypeOfChosenAgent2);
        
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
        Player1Score.text = gs.playerScore.ToString();
        Player2Score.text = gs.iaScore.ToString();

        Rules.Step(ref gs, agent.Act(ref gs, availableActions, 1),agent2.Act(ref gs, availableActions2, 2));
    }

    private void DefineAgent1(int dropDownIndex1)
    {
        switch (dropDownIndex1)
        {
            case 0: // player
                agent = new HumanAgent();
                break;
            case 1: // random
                agent = new RandomAgent {rdm = new Unity.Mathematics.Random((uint) Time.frameCount)};
                break;
            case 2: // randomRollout
                agent = new RandomRolloutAgent();
                break;
            case 3: // Dijstra
                break;
            case 4: // MCTS
                break;
            case 5: //Q Learning
                break;
        }
    }

    private void DefineAgent2(int dropDownIndex2)
    {
        switch (dropDownIndex2)
        {
            case 0: // player
                agent2 = new HumanAgent();
                break;
            case 1: // random
                agent2 = new RandomAgent {rdm = new Unity.Mathematics.Random((uint) Time.frameCount)};
                break;
            case 2: // randomRollout
                agent2 = new RandomRolloutAgent();
                break;
            case 3: // Dijstra
                break;
            case 4: // MCTS
                break;
            case 5: //Q Learning
                break;
        }
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
            var projectileView = Instantiate(Player1ProjectilePrefab).GetComponent<Transform>();
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