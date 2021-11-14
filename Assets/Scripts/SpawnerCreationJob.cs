
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;

public struct SpawnerCreationJob : IJobParallelFor
{
    public NativeArray<Vector3> SpawnPointsArray;
    public void Execute(int index)
    {
       
    }
}
