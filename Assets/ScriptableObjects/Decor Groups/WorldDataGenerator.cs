public class WorldDataGenerator
{
    private static System.Random rndg;

    public static MeshDataSO[] meshDataSOs;
    public static MeshDataSO GetRandomMeshData(int seed)
    {
        rndg = new System.Random(seed);
        return meshDataSOs[rndg.Next(meshDataSOs.Length)];
    }

}
