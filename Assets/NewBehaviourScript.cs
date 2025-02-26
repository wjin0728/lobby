using UnityEngine;

public class CheckTerrainSize : MonoBehaviour
{
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            Vector3 size = terrain.terrainData.size;
            Debug.Log($"지형 크기: Width={size.x}m, Length={size.z}m, Height={size.y}m");
            float[,] heights = terrain.terrainData.GetHeights(0, 0, 513, 513);
            float maxHeight = 0;
            Texture2D alpha = terrain.terrainData.alphamapTextures[0];
            for (int y = 0; y < 513; y++)
            {
                for (int x = 0; x < 513; x++)
                {
                    if (heights[y, x] > maxHeight) maxHeight = heights[y, x];
                }
            }
            Debug.Log($"{alpha.height}");
        }
    }
}