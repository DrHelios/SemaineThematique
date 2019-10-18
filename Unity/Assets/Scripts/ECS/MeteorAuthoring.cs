using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct Meteor : IComponentData
{
    //public float radius;
    public float2 pos;
    public float2 speed;
}

[RequiresEntityConversion]
public class MeteorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Meteor>(entity);
    }
}