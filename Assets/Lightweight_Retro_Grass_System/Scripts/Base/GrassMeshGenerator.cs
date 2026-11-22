using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace grassGeneration
{
    public static class GrassMeshGenerator
    {
        public static List<Mesh> GenerateGrassMeshes(
            TerrainSampler terrainSampler,
            float patchSize,
            int bladesPerPatch,
            float grassMinHeight,
            float grassMaxHeight,
            float grassMinFade,
            float grassMaxFade,
            float[] bladeWidth,
            float[] bladeHeight,
            Material grassMat,
            List<float> orderedGrassTypeDensity,
            List<int> grassOrder,
            float maxSlope,
            int seed = 0)
        {
            Debug.Log(terrainSampler.transform.position.y);
            List<Mesh> grassMeshes = new List<Mesh>();

            if (terrainSampler == null)
            {
                Debug.LogError("GrassMeshGenerator: TerrainSampler is null.");
                return grassMeshes;
            }

            if (patchSize <= 0f)
            {
                Debug.LogError("GrassMeshGenerator: patchSize must be greater than 0.");
                return grassMeshes;
            }

            if (bladesPerPatch <= 0)
            {
                Debug.LogError("GrassMeshGenerator: bladesPerPatch must be greater than 0.");
                return grassMeshes;
            }

            Bounds terrainBounds =
                terrainSampler.GetBounds();

            float terrainWidth =
                terrainBounds.size.x;

            float terrainDepth =
                terrainBounds.size.z;

            int patchesX =
                Mathf.CeilToInt(terrainWidth / patchSize);

            int patchesZ =
                Mathf.CeilToInt(terrainDepth / patchSize);

            System.Random random = new System.Random(seed);

            Vector3[] candidatePositions =
                new Vector3[bladesPerPatch];

            TerrainSample[] samples =
                new TerrainSample[bladesPerPatch];

            bool[] valid =
                new bool[bladesPerPatch];

            for (int patchZ = 0; patchZ < patchesZ; patchZ++)
            {
                for (int patchX = 0; patchX < patchesX; patchX++)
                {
                    int verticesPerBlade = 8;
                    int indicesPerBlade = 12;

                    List<Vector3> vertices =
                        new List<Vector3>(
                            bladesPerPatch * verticesPerBlade
                        );

                    List<int> triangles =
                        new List<int>(
                            bladesPerPatch * indicesPerBlade
                        );

                    List<Vector2> uvs =
                        new List<Vector2>(
                            bladesPerPatch * 8
                        );

                    List<Vector3> normals =
                        new List<Vector3>(
                            bladesPerPatch * 8
                        );

                    List<Color32> colors =
                        new List<Color32>(
                            bladesPerPatch * 8
                        );

                    float patchMinX = terrainBounds.min.x + patchX * patchSize;

                    float patchMinZ = terrainBounds.min.z + patchZ * patchSize;

                    float currentPatchWidth = Mathf.Min(patchSize, terrainBounds.max.x - patchMinX);

                    float currentPatchDepth =
                        Mathf.Min
                        (
                            patchSize,
                            terrainBounds.max.z - patchMinZ
                        );

                    if (currentPatchWidth <= 0f ||
                        currentPatchDepth <= 0f)
                    {
                        continue;
                    }


                    for (int i = 0; i < bladesPerPatch; i++)
                    {
                        float localX =
                            patchMinX +
                            (float)random.NextDouble() *
                            currentPatchWidth;

                        float localZ =
                            patchMinZ +
                            (float)random.NextDouble() *
                            currentPatchDepth;

                        candidatePositions[i] =
                            new Vector3(
                                localX,
                                0f,
                                localZ
                            );
                    }

                    terrainSampler.SampleBatch(candidatePositions, samples, valid);

                    for (int i = 0; i < bladesPerPatch; i++)
                    {
                        if (!valid[i])
                            continue;

                        TerrainSample sample = samples[i];

                        float worldY =
                            sample.position.y;

                        // Height check.
                        if (worldY + 0.1f < grassMinHeight ||
                            worldY + 0.1f > grassMaxHeight)
                        {
                            continue;
                        }

                        float slope = Vector3.Angle(sample.normal, Vector3.up);

                        if (slope > maxSlope)
                        {
                            continue;
                        }

                        float density = 1f;

                        // Fade in at the lower boundary.
                        if (grassMinFade > 0f &&
                            worldY <
                            grassMinHeight + grassMinFade)
                        {
                            float t = Mathf.InverseLerp(
                                grassMinHeight,
                                grassMinHeight + grassMinFade,
                                worldY
                            );

                            density =
                                Mathf.SmoothStep(0f, 1f, t);
                        }

                        // Fade out at the upper boundary.
                        if (grassMaxFade > 0f &&
                            worldY >
                            grassMaxHeight - grassMaxFade)
                        {
                            float t = Mathf.InverseLerp(
                                grassMaxHeight - grassMaxFade,
                                grassMaxHeight,
                                worldY
                            );

                            float upperDensity =
                                Mathf.SmoothStep(1f, 0f, t);

                            density =
                                Mathf.Min(
                                    density,
                                    upperDensity
                                );
                        }

                        if ((float)random.NextDouble() > density)
                        {
                            continue;
                        }

                        // Slight random size variation.
                        float randomScale =
                            Mathf.Lerp(0.8f, 1.2f, (float)random.NextDouble());

                        int grassType = DefineGrassType(orderedGrassTypeDensity, grassOrder);

                        float finalWidth = bladeWidth[grassType] * randomScale;
                        float finalHeight = bladeHeight[grassType] * randomScale;

                        float rotation =
                            (float)random.NextDouble() * 360f;

                        AddGrassBlade(
                            vertices,
                            triangles,
                            uvs,
                            normals,
                            colors,
                            sample.position,
                            finalWidth,
                            finalHeight,
                            rotation,
                            grassType
                        );
                    }

                    if (vertices.Count == 0)
                        continue;

                    Mesh mesh = new Mesh();

                    mesh.name = $"GrassPatch_{patchX}_{patchZ}";

                    // This is unnecessary for normal-sized patches,
                    // but safe to leave enabled.
                    mesh.indexFormat = IndexFormat.UInt32;

                    mesh.SetVertices(vertices);
                    mesh.SetTriangles(triangles, 0);
                    mesh.SetUVs(0, uvs);
                    mesh.SetNormals(normals);
                    mesh.SetColors(colors);
                    //mesh.set

                    mesh.RecalculateBounds();

                    grassMeshes.Add(mesh);
                }
            }

            return grassMeshes;
        }

        private static int DefineGrassType(List<float> orderedGrassTypeDensity, List<int> grassOrder)
        {
            int grassType = 0;
            int randomIndex = Random.Range(0, 1001);
            float chance0 = orderedGrassTypeDensity[0] * 10;
            float chance1 = (orderedGrassTypeDensity[0] + orderedGrassTypeDensity[1]) * 10;
            float chance2 = (orderedGrassTypeDensity[0] + orderedGrassTypeDensity[1] + orderedGrassTypeDensity[2]) * 10;
            float chance3 = (orderedGrassTypeDensity[0] + orderedGrassTypeDensity[1] + orderedGrassTypeDensity[2] + orderedGrassTypeDensity[3]) * 10;
            if (randomIndex < chance0)
            {
                grassType = grassOrder[0];
            }
            else if (randomIndex > chance0 && randomIndex <= chance1)
            {
                grassType = grassOrder[1];
            }
            else if (randomIndex > chance1 && randomIndex <= chance2)
            {
                grassType = grassOrder[2];
            }
            else if (randomIndex > chance2 && randomIndex <= chance3)
            {
                grassType = grassOrder[3];
            }

            return grassType;
        }

        private static void AddGrassBlade(
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector2> uvs,
            List<Vector3> normals,
            List<Color32> colors,
            Vector3 position,
            float width,
            float height,
            float rotation,
            int grassType)
        {
            float grassTypeValue = grassType / 3f;
            Color grassColor = new Color(
                grassTypeValue,
                0f,
                0f,
                0f
            );

            Quaternion rotationQuaternion =
                Quaternion.Euler(0f, rotation, 0f);

            // Two crossed quads.
            AddGrassQuad(
                vertices,
                triangles,
                uvs,
                normals,
                colors,
                position,
                width,
                height,
                rotationQuaternion,
                grassColor
            );

            AddGrassQuad(
                vertices,
                triangles,
                uvs,
                normals,
                colors,
                position,
                width,
                height,
                rotationQuaternion *
                Quaternion.Euler(0f, 90f, 0f),
                grassColor
            );
        }

        private static void AddGrassQuad(
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector2> uvs,
            List<Vector3> normals,
            List<Color32> colors,
            Vector3 position,
            float width,
            float height,
            Quaternion rotation,
            Color grassColor)
        {
            Vector3 right =
                rotation * Vector3.right * (width * 0.5f);

            Vector3 up =
                Vector3.up * height;

            int vertexStart = vertices.Count;

            vertices.Add(position - right);
            vertices.Add(position + right);
            vertices.Add(position + right + up);
            vertices.Add(position - right + up);

            colors.Add(grassColor);
            colors.Add(grassColor);
            colors.Add(grassColor);
            colors.Add(grassColor);

            uvs.Add(new Vector2(0f, 0f));
            uvs.Add(new Vector2(1f, 0f));
            uvs.Add(new Vector2(1f, 1f));
            uvs.Add(new Vector2(0f, 1f));

            // Approximate normal for the plane.
            Vector3 normal =
                rotation * Vector3.forward;

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            triangles.Add(vertexStart + 0);
            triangles.Add(vertexStart + 2);
            triangles.Add(vertexStart + 1);

            triangles.Add(vertexStart + 0);
            triangles.Add(vertexStart + 3);
            triangles.Add(vertexStart + 2);
        }
    }
}

