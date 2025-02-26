using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEditor.SearchService;

public class SceneObjectExporter
{
    private static List<string> sceneUsingMeshNamesList = new List<string>();
    private static List<string> sceneUsingMaterialNamesList = new List<string>();
    private static List<string> m_pTextureNamesListForCounting = new List<string>();
    private static List<string> m_pTextureNamesListForWriting = new List<string>();

    private static BinaryWriter binaryWriter = null;
    private static bool hasBounds = false;
    private static Bounds combinedBounds = new(Vector3.zero, Vector3.zero);

    [MenuItem("Scene/Export All Objects")]
    static void ExportAllObjects()
    {
        // 현재 씬의 모든 GameObject 추출
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        Dictionary<string, GameObject> prefabSources = new Dictionary<string, GameObject>();

        ExportResources();

        /*string filePath = EditorUtility.SaveFilePanel("Save Scene bin", "Assets", "Scene.bin", "bin");
        if (string.IsNullOrEmpty(filePath)) return;

        binaryWriter = new BinaryWriter(File.Open(filePath, FileMode.Create));

        WriteString("<Prefabs:>");
        foreach (GameObject obj in allObjects)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                GameObject prefabSource = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(obj);

                string prefabPath = AssetDatabase.GetAssetPath(prefabSource);
                if (!prefabSources.ContainsKey(prefabPath))
                {
                    WriteObject(prefabSource);
                    prefabSources[prefabPath] = prefabSource;
                }
            }
        }
        WriteString("</Prefabs>");

        foreach (GameObject obj in allObjects)
        {

        }


        binaryWriter.Flush();
        binaryWriter.Close();*/
    }

    static void ExportResources()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        Dictionary<string, Mesh> sceneMeshes = new Dictionary<string, Mesh>();
        Dictionary<string, Material> sceneMaterials = new Dictionary<string, Material>();

        //씬 내 공유 리소스 탐색
        foreach (GameObject obj in allObjects)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                string meshName = meshFilter.sharedMesh.name;
                if (!sceneMeshes.ContainsKey(meshName))
                {
                    sceneMeshes[meshName] = meshFilter.sharedMesh;
                }
            }
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer)
            {
                Material[] materials = renderer.sharedMaterials;
                foreach (Material material in materials)
                {
                    string matName = material.name;
                    if (!sceneMaterials.ContainsKey(matName))
                    {
                        sceneMaterials[matName] = material;
                    }
                }
            }
        }
        //리소스 추출
        string filePath = EditorUtility.SaveFilePanel("Save Scene Resource bin", "Assets", "SceneResource.bin", "bin");
        if (string.IsNullOrEmpty(filePath)) return;

        binaryWriter = new BinaryWriter(File.Open(filePath, FileMode.Create));

        int i = 0;
        WriteString("<Meshes:>");
        foreach (Mesh mesh in sceneMeshes.Values)
        {
            i++;
            WriteMeshInfo(mesh);
            if (i == 30) break;
        }
        WriteString("</Meshes>");

        WriteString("<Materials:>");
        foreach (Material material in sceneMaterials.Values)
        {
            WriteMaterial(material);
        }
        WriteString("</Materials>");

        binaryWriter.Flush();
        binaryWriter.Close();

        Debug.Log($"리소스 추출 완료");
    }

    static bool FindResourcesByName(List<string> pTextureNamesList, Object resource)
    {
        if (resource)
        {
            string strTextureName = string.Copy(resource.name).Replace(" ", "_");
            for (int i = 0; i < pTextureNamesList.Count; i++)
            {
                if (pTextureNamesList.Contains(strTextureName)) return (true);
            }
            pTextureNamesList.Add(strTextureName);
            return (false);
        }
        else
        {
            return (true);
        }
    }

    static void WriteObjectName(Object obj)
    {
        binaryWriter.Write((obj) ? string.Copy(obj.name).Replace(" ", "_") : "null");
    }

    static void WriteObjectName(int i, Object obj)
    {
        binaryWriter.Write(i);
        binaryWriter.Write((obj) ? string.Copy(obj.name).Replace(" ", "_") : "null");
    }

    static void WriteObjectName(string strHeader, Object obj)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write((obj) ? string.Copy(obj.name).Replace(" ", "_") : "null");
    }

    static void WriteObjectName(string strHeader, int i, Object obj)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write((obj) ? string.Copy(obj.name).Replace(" ", "_") : "null");
        binaryWriter.Write(i);
    }

    static void WriteObjectName(string strHeader, int i, int j, Object obj)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(i);
        binaryWriter.Write(j);
        binaryWriter.Write((obj) ? string.Copy(obj.name).Replace(" ", "_") : "null");
    }

    static void WriteObjectName(string strHeader, int i, Object obj, float f, int j, int k)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(i);
        binaryWriter.Write((obj) ? string.Copy(obj.name).Replace(" ", "_") : "null");
        binaryWriter.Write(f);
        binaryWriter.Write(j);
        binaryWriter.Write(k);
    }

    static void WriteString(string strToWrite)
    {
        binaryWriter.Write(strToWrite);
    }

    static void WriteString(string strHeader, string strToWrite)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(strToWrite);
    }

    static void WriteString(string strToWrite, int i)
    {
        binaryWriter.Write(strToWrite);
        binaryWriter.Write(i);
    }

    static void WriteString(string strToWrite, int i, float f)
    {
        binaryWriter.Write(strToWrite);
        binaryWriter.Write(i);
        binaryWriter.Write(f);
    }

    static void WriteTextureName(string strHeader, Texture texture)
    {
        binaryWriter.Write(strHeader);
        if (texture)
        {
            binaryWriter.Write(string.Copy(texture.name).Replace(" ", "_"));
        }
        else
        {
            binaryWriter.Write("null");
        }
    }

    static void WriteInteger(int i)
    {
        binaryWriter.Write(i);
    }

    static void WriteInteger(string strHeader, int i)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(i);
    }

    static void WriteFloat(string strHeader, float f)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(f);
    }

    static void WriteVector(Vector2 v)
    {
        binaryWriter.Write(v.x);
        binaryWriter.Write(v.y);
    }

    static void WriteVector(string strHeader, Vector2 v)
    {
        binaryWriter.Write(strHeader);
        WriteVector(v);
    }

    static void WriteVector(Vector3 v)
    {
        binaryWriter.Write(v.x);
        binaryWriter.Write(v.y);
        binaryWriter.Write(v.z);
    }

    static void WriteVector(string strHeader, Vector3 v)
    {
        binaryWriter.Write(strHeader);
        WriteVector(v);
    }

    static void WriteVector(Vector4 v)
    {
        binaryWriter.Write(v.x);
        binaryWriter.Write(v.y);
        binaryWriter.Write(v.z);
        binaryWriter.Write(v.w);
    }

    static void WriteVector(string strHeader, Vector4 v)
    {
        binaryWriter.Write(strHeader);
        WriteVector(v);
    }

    static void WriteVector(Quaternion q)
    {
        binaryWriter.Write(q.x);
        binaryWriter.Write(q.y);
        binaryWriter.Write(q.z);
        binaryWriter.Write(q.w);
    }

    static void WriteVector(string strHeader, Quaternion q)
    {
        binaryWriter.Write(strHeader);
        WriteVector(q);
    }

    static void WriteColor(Color c)
    {
        binaryWriter.Write(c.r);
        binaryWriter.Write(c.g);
        binaryWriter.Write(c.b);
        binaryWriter.Write(c.a);
    }

    static void WriteColor(string strHeader, Color c)
    {
        binaryWriter.Write(strHeader);
        WriteColor(c);
    }

    static void WriteTextureCoord(Vector2 uv)
    {
        binaryWriter.Write(uv.x);
        binaryWriter.Write(1.0f - uv.y);
    }

    static void WriteVectors(string strHeader, Vector2[] vectors)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(vectors.Length);
        if (vectors.Length > 0) foreach (Vector2 v in vectors) WriteVector(v);
    }

    static void WriteVectors(string strHeader, Vector3[] vectors)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(vectors.Length);
        if (vectors.Length > 0) foreach (Vector3 v in vectors) WriteVector(v);
    }

    static void WriteVectors(string strHeader, Vector4[] vectors)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(vectors.Length);
        if (vectors.Length > 0) foreach (Vector4 v in vectors) WriteVector(v);
    }

    static void WriteColors(string strHeader, Color[] colors)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(colors.Length);
        if (colors.Length > 0) foreach (Color c in colors) WriteColor(c);
    }

    static void WriteTextureCoords(string strHeader, Vector2[] uvs)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(uvs.Length);
        if (uvs.Length > 0) foreach (Vector2 uv in uvs) WriteTextureCoord(uv);
    }

    static void WriteIntegers(int[] pIntegers)
    {
        binaryWriter.Write(pIntegers.Length);
        foreach (int i in pIntegers) binaryWriter.Write(i);
    }

    static void WriteIntegers(string strHeader, int[] pIntegers)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(pIntegers.Length);
        if (pIntegers.Length > 0) foreach (int i in pIntegers) binaryWriter.Write(i);
    }

    static void WriteIntegers(string strHeader, int n, int[] pIntegers)
    {
        binaryWriter.Write(strHeader);
        binaryWriter.Write(n);
        binaryWriter.Write(pIntegers.Length);
        if (pIntegers.Length > 0) foreach (int i in pIntegers) binaryWriter.Write(i);
    }

    static void WriteBoundingBox(string strHeader, Bounds bounds)
    {
        binaryWriter.Write(strHeader);
        WriteVector(bounds.center);
        WriteVector(bounds.extents);
    }

    static void WriteMatrix(Matrix4x4 matrix)
    {
        binaryWriter.Write(matrix.m00);
        binaryWriter.Write(matrix.m10);
        binaryWriter.Write(matrix.m20);
        binaryWriter.Write(matrix.m30);
        binaryWriter.Write(matrix.m01);
        binaryWriter.Write(matrix.m11);
        binaryWriter.Write(matrix.m21);
        binaryWriter.Write(matrix.m31);
        binaryWriter.Write(matrix.m02);
        binaryWriter.Write(matrix.m12);
        binaryWriter.Write(matrix.m22);
        binaryWriter.Write(matrix.m32);
        binaryWriter.Write(matrix.m03);
        binaryWriter.Write(matrix.m13);
        binaryWriter.Write(matrix.m23);
        binaryWriter.Write(matrix.m33);
    }

    static void WriteMatrix(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.SetTRS(position, rotation, scale);
        WriteMatrix(matrix);
    }

    static void WriteTransform(string strHeader, Transform current)
    {
        binaryWriter.Write(strHeader);
        WriteVector(current.localPosition);
        WriteVector(current.localEulerAngles);
        WriteVector(current.localScale);
        WriteVector(current.localRotation);
    }

    static void WriteLocalMatrix(string strHeader, Transform current)
    {
        binaryWriter.Write(strHeader);
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.SetTRS(current.localPosition, current.localRotation, current.localScale);
        WriteMatrix(matrix);
    }

    static void WriteWorldMatrix(string strHeader, Transform current)
    {
        binaryWriter.Write(strHeader);
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.SetTRS(current.position, current.rotation, current.lossyScale);
        WriteMatrix(matrix);
    }

    static void WriteMatrixes(string strHeader, Matrix4x4[] matrixes)
    {
        WriteString(strHeader, matrixes.Length);
        if (matrixes.Length > 0)
        {
            foreach (Matrix4x4 matrix in matrixes) WriteMatrix(matrix);
        }
    }

    static void WriteMeshInfo(Mesh mesh)
    {
        WriteObjectName("<Mesh>:", mesh);

        WriteInteger(mesh.vertexCount);


        WriteBoundingBox("<Bounds>:", mesh.bounds);

        if ((mesh.vertices != null) && (mesh.vertices.Length > 0)) WriteVectors("<Positions>:", mesh.vertices);
        if ((mesh.colors != null) && (mesh.colors.Length > 0)) WriteColors("<Colors>:", mesh.colors);
        if ((mesh.uv != null) && (mesh.uv.Length > 0)) WriteTextureCoords("<TextureCoords0>:", mesh.uv);
        if ((mesh.uv2 != null) && (mesh.uv2.Length > 0)) WriteTextureCoords("<TextureCoords1>:", mesh.uv2);
        if ((mesh.normals != null) && (mesh.normals.Length > 0)) WriteVectors("<Normals>:", mesh.normals);

        if ((mesh.normals.Length > 0) && (mesh.tangents.Length > 0))
        {
            Vector3[] tangents = new Vector3[mesh.tangents.Length];
            Vector3[] biTangents = new Vector3[mesh.tangents.Length];
            for (int i = 0; i < mesh.tangents.Length; i++)
            {
                tangents[i] = new Vector3(mesh.tangents[i].x, mesh.tangents[i].y, mesh.tangents[i].z);
                biTangents[i] = Vector3.Normalize(Vector3.Cross(mesh.normals[i], tangents[i])) * mesh.tangents[i].w;
            }

            WriteVectors("<Tangents>:", tangents);
            WriteVectors("<BiTangents>:", biTangents);
        }

        WriteInteger("<SubMeshes>:", mesh.subMeshCount);
        if (mesh.subMeshCount > 0)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] subindicies = mesh.GetTriangles(i);
                WriteIntegers("<SubMesh>:", i, subindicies);
            }
        }
        WriteString("</Mesh>");

    }

    static void WriteMaterial(Material material)
    {
        WriteObjectName("<Material>:", material);

        string shaderName = material.shader.name;

        WriteObjectName("<Shader>:", material.shader);

        switch (shaderName)
        {
            case "SyntyStudios/Basic_LOD_Shader":

                WriteTextureName("<AlbedoMap>:", material.GetTexture("_Albedo"));
                WriteColor("<AlbedoColor>:", material.GetColor("_AlbedoColour"));

                WriteFloat("<Smoothness>:", material.GetFloat("_Smoothness"));

                WriteFloat("<Metallic>:", material.GetFloat("_Metallic"));

                WriteTextureName("<NormalMap>:", material.GetTexture("_NormalMap"));

                break;
            case "SyntyStudios/Triplanar_01":
            case "SyntyStudios/Triplanar_Basic":

                WriteTextureName("<SidesMap>:", material.GetTexture("_Sides"));
                WriteTextureName("<SidesNormalMap>:", material.GetTexture("_SidesNormal"));

                WriteTextureName("<TopMap>:", material.GetTexture("_Top"));
                WriteTextureName("<TopNormalMap>:", material.GetTexture("_TopNormal"));

                WriteFloat("<FallOff>:", material.GetFloat("_FallOff"));
                WriteFloat("<Tiling>:", material.GetFloat("_Tiling"));

                break;
            case "SyntyStudios/VegitationShader":
            case "SyntyStudios/VegitationShader_Basic":

                WriteTextureName("<LeafAlbedoMap>:", material.GetTexture("_LeafTex"));
                WriteTextureName("<LeafNormalMap>:", material.GetTexture("_LeafNormalMap"));
                WriteColor("<LeafAlbedoColor>:", material.GetColor("_LeafBaseColour"));
                WriteFloat("<LeafSmoothness>:", material.GetFloat("_LeafSmoothness"));
                WriteFloat("<LeafMetallic>:", material.GetFloat("_LeafMetallic"));

                WriteTextureName("<TrunkAlbedoMap>:", material.GetTexture("_TunkTex"));
                WriteTextureName("<TrunkNormalMap>:", material.GetTexture("_TrunkNormalMap"));
                WriteColor("<TrunkAlbedoColor>:", material.GetColor("_TrunkBaseColour"));
                WriteFloat("<TrunkSmoothness>:", material.GetFloat("_TrunkSmoothness"));
                WriteFloat("<TrunkMetallic>:", material.GetFloat("_TrunkMetallic"));

                WriteTextureName("<EmissiveMap>:", material.GetTexture("_EmissiveMask") ?? material.GetTexture("_TrunkEmissiveMask"));
                WriteColor("<EmissiveColor>:", material.GetColor("_EmissionColor"));

                break;
            case "Universal Render Pipeline/Lit":

                WriteTextureName("<AlbedoMap>:", material.GetTexture("_BaseMap"));
                WriteColor("<AlbedoColor>:", material.GetColor("_BaseColor"));

                WriteFloat("<Smoothness>:", material.GetFloat("_Smoothness"));

                WriteTextureName("<MetallicMap>:", material.GetTexture("_MetallicGlossMap"));
                WriteFloat("<Metallic>:", material.GetFloat("_Metallic"));

                WriteTextureName("<SpecularMap>:", material.GetTexture("_SpecGlossMap"));
                WriteColor("<Specular>:", material.GetColor("_SpecColor"));

                WriteTextureName("<NormalMap>:", material.GetTexture("_BumpMap"));

                break;
            default:
                break;
        }

        WriteString("</Material>");
    }

    static void WriteFrameInfo(Transform current)
    {
        WriteTransform("<Transform>:", current);
        WriteLocalMatrix("<TransformMatrix>:", current);

        MeshFilter meshFilter = current.gameObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = current.gameObject.GetComponent<MeshRenderer>();

        if (meshFilter && meshRenderer)
        {
            WriteMeshInfo(meshFilter.sharedMesh);

            if (!hasBounds)
            {
                combinedBounds = meshRenderer.bounds;
                hasBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(meshRenderer.bounds);
            }

            Material[] materials = meshRenderer.materials;
            WriteInteger(materials.Length);
            if (materials.Length > 0)
            {
                foreach(Material mat in materials)
                {
                    WriteObjectName(mat);
                }
            }
        }
    }

    static void WriteFrameHierarchyInfo(Transform child)
    {
        WriteFrameInfo(child);

        WriteInteger("<Children>:", child.childCount);

        if (child.childCount > 0)
        {
            for (int k = 0; k < child.childCount; k++)
            {
                WriteFrameHierarchyInfo(child.GetChild(k));
            }
        }

        WriteString("</Frame>");
    }

    static void WriteObject(GameObject gameObject)
    {
        Transform transform = gameObject.transform;
        WriteFrameHierarchyInfo(transform);

        Vector3 sphereCenter = transform.InverseTransformPoint(combinedBounds.center);
        float radius = combinedBounds.extents.magnitude;
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        radius /= maxScale;

        WriteVector(sphereCenter);
        binaryWriter.Write(radius);

    }
}
