using UnityEngine;


public class SurfaceSampler : MonoBehaviour
{
    // Get the terrain mix at a given position.
    private float[] GetTextureMix(Vector3 samplePosition, Terrain terrain)
    {
        Vector3 terrainPosition = terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;

        // Sample position in relation to terrain alphamap
        int mapPosX = Mathf.RoundToInt((samplePosition.x - terrainPosition.x) / terrainData.size.x * terrainData.alphamapWidth);
        int mapPosZ = Mathf.RoundToInt((samplePosition.z - terrainPosition.z) / terrainData.size.z * terrainData.alphamapHeight);
        
        float[,,] splatMapData = terrainData.GetAlphamaps(mapPosX, mapPosZ, 1, 1);
        
        // Extract values into that cellmix
        float[] cellMix = new float[splatMapData.GetUpperBound(2) + 1];

        // Convert 3D array to 1D array
        for (int i = 0; i < cellMix.Length; i++)
        {
            cellMix[i] = splatMapData[0, 0, i];
        }

        return cellMix;
    }

    public SurfaceAudioLibrary.SurfaceType GetSurfaceTypeFromTerrain(Vector3 samplePosition, Terrain terrain)
    {
        float[] cellMix = GetTextureMix(samplePosition, terrain);
        float strongestTexture = 0;
        int maxIndex = 0;
        for (int i = 0; i < cellMix.Length; i++)
        {
            if (cellMix[i] > strongestTexture)
            {
                strongestTexture = cellMix[i];
                maxIndex = i;
            }
        
        }

        string terrainName = terrain.terrainData.terrainLayers[maxIndex].name;
        return SurfaceAudioLibrary.TerrainNameToSurfaceType(terrainName);
    }
}