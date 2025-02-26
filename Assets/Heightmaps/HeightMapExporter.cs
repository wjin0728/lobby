using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportHeightmapToRaw
{
    [MenuItem("Terrain/Export Heightmap to RAW")]
    static void ExportToRaw()
    {
        GameObject selectedObject = Selection.activeGameObject;
        TerrainData terrain = null;

        // ������ Terrain ������Ʈ�� ������ ���
        if (selectedObject != null)
        {
            Terrain terrainComponent = selectedObject.GetComponent<Terrain>();
            if (terrainComponent != null)
            {
                terrain = terrainComponent.terrainData; // Terrain���� TerrainData ��������
            }
        }

        // Assets���� TerrainData�� ���� ������ ���
        if (terrain == null)
        {
            terrain = Selection.activeObject as TerrainData;
        }

        // TerrainData�� ������ ���� �޽���
        if (terrain == null)
        {
            Debug.LogError("Terrain�̳� TerrainData�� �����ϼ���!");
            return;
        }

        int res = terrain.heightmapResolution;
        if (res != 513)
        {
            Debug.LogWarning($"Heightmap �ػ󵵰� {res}x{res}�Դϴ�. 513x513�� �ƴ� ��� �����ϼ���.");
        }

        float[,] heights = terrain.GetHeights(0, 0, res, res);

        string filePath = EditorUtility.SaveFilePanel("Save RAW Heightmap", "Assets", "heightmap.raw", "raw");
        if (string.IsNullOrEmpty(filePath)) return;

        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    ushort value = (ushort)(heights[y, x] * 65535f);
                    writer.Write(value);
                }
            }
        }

        Debug.Log($"RAW ���� ���� �Ϸ�: {filePath} ({res}x{res})");
        AssetDatabase.Refresh();
    }
}