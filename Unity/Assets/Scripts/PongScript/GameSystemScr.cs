using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rules = GameStateRules;

public class GameSystemScr : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public GameObject ProjectilePrefab;
    public GameObject PlayerPrefab;

    private GameStateScr gs;

    private readonly List<Transform> enemiesView = new List<Transform>();
    private readonly List<Transform> projectilesView = new List<Transform>();
    private Transform playerView;
    private IBot bot;

    void Start()
    {
        Rules.Init(ref gs);

        playerView = Instantiate(PlayerPrefab).GetComponent<Transform>();
//        bot = new RandomAgent();
          //bot = new HumanBot();
          bot = new RandomBot();
        //bot = new RandomRolloutAgent();
    }

    void Update()
    {
        if (gs.isGameOver)
        {
            return;
        }

        SyncEnemyViews();
        SyncProjectileViews();
        playerView.position = gs.playerPos;

        Rules.Step(ref gs, bot.Act(ref gs, Rules.GetAvailableActions(ref gs)));
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
            enemiesView[i].position = gs.enemies[i].pos;
        }
    }

    private void SyncProjectileViews()
    {
        var projectilesToSpawn = gs.bullets.Length - projectilesView.Count;

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
            projectilesView[i].position = gs.bullets[i].pos;
        }
    }

    private void OnDestroy()
    {
        gs.enemies.Dispose();
        gs.bullets.Dispose();
    }
}

