﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEntities : ComponentSystem
{
    public Mesh spr;
    public Material mat;
    private float currentSpawnStep;
    private float lastSpawnStep;
    private float spawnDelay;

    protected override void OnCreate()
    {
        spawnDelay = 60 * 1f; // 60 car 60 fps
        currentSpawnStep = spawnDelay;
    }

    protected override void OnUpdate()
    {
        var spawners = Entities.WithAll<Spawner>().ToEntityQuery().ToComponentDataArray<Spawner>(Allocator.TempJob);

        if (spawners.Length <= 0)
        {
            spawners.Dispose();
            return;
        }
        
        var meteor = Entities.WithAll<Meteor>().ToEntityQuery().ToEntityArray(Allocator.TempJob);

        if (currentSpawnStep - lastSpawnStep >= spawnDelay && meteor.Length < 10000) // on cap à 10 000 entités 
        {
            var entity = EntityManager.Instantiate(spawners[0].meteorPrefab);
            EntityManager.SetComponentData(entity, new Meteor
            {
                pos = new float2(Random.Range(-9f,9f),Random.Range(0f,9f)), 
                speed =  new float2(Random.Range(-0.05f,0.05f), Random.Range(-0.05f,0.05f))
            });    
            lastSpawnStep = currentSpawnStep;

        }

        currentSpawnStep++;

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            var entity = EntityManager.Instantiate(spawners[0].meteorPrefab);
            EntityManager.SetComponentData(entity, new Translation
            {
                Value = new float3(Random.Range(-5, 5), Random.Range(-5, 5), 0)
            });
        }*/

        Entities.ForEach((ref Meteor met) => met.pos += met.speed);

        if (meteor.Length != 0)
        {
            for (var i = 0; i < meteor.Length; i++)
            {
                var posMet = EntityManager.GetComponentData<Meteor>(meteor[i]).pos;
                EntityManager.SetComponentData(meteor[i], new Translation
                {
                    Value = new float3(posMet.x, posMet.y, 0)
                });
            }
        }

        
        meteor.Dispose();
        spawners.Dispose();
    }
}