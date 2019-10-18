using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Spawner : IComponentData
{
    public Entity meteorPrefab;
}

public class MeteorSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject meteorPrefab;


    public void Convert(Entity meteorEntity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var meteorSpawner = new Spawner
        {
            meteorPrefab = conversionSystem.GetPrimaryEntity(meteorPrefab),
        };
        
        dstManager.AddComponent<Spawner>(meteorEntity);
        dstManager.SetComponentData(meteorEntity, meteorSpawner);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meteorPrefab);
    }
}
