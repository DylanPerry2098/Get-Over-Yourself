using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

namespace grassGeneration
{
    public class GrassSpawner : MonoBehaviour
    {
        [SerializeField] private TerrainSampler terrainSampler;

        public Material grassMaterial;

        [Tooltip("The maximum amount of grass types to be spawned. Any grass modifiers applied to parameters beyound this limit will be ignored")]
        [Range(1, 4)][SerializeField] int maxGrassTypes = 3;
        [Tooltip("The density of each grass type in %. If the total is more than 100, a proportional amount will be subtracted from each to total 100.")]
        [Range(0, 100)][SerializeField] private float[] grassTypesDensity = new float[4];
        private List<float> orderedGrassTypeDensity;
        private List<int> grassOrder;

        [Tooltip("The size of each patch of grass. Larger patches will result in sparser grass, but may improve frame rate on larger terrains")]
        public float patchSize = 20f;
        [Tooltip("How many individual blades of grass are spawned per patch. Directly correlate to how dense grass is.")]
        public int bladesPerPatch = 500;

        [Tooltip("Minimum local terrain height where grass will spawn")]
        public float grassMinHeight = 0f;
        [Tooltip("Maximum local terrain height where grass will spawn")]
        public float grassMaxHeight = 50f;

        [Tooltip("Maximum angle at which grass can spawn")]
        public float maxSlope = 45f;

        [Tooltip("Base width of each blade of grass")]
        public float[] bladeWidths = new float[4];
        [Tooltip("Base height of each blade of grass")]
        public float[] bladeHeights = new float[4];

        [Tooltip("distance from min height at which grass density will start to decrease")]
        public float grassMinFade = 10f;
        [Tooltip("distance from max height at which grass density will start to decrease")]
        public float grassMaxFade = 10f;

        private List<Mesh> meshes;

        [SerializeField] private Transform parent;

        public void GenerateGrass()
        {
            if (terrainSampler == null)
            {
                Debug.LogError(
                    "GrassSpawner has no TerrainSampler assigned.",
                    this
                );

                return;
            }

            SortGrassDensity();
            grassMaterial.SetFloat("_GrassTypeTotal", maxGrassTypes);

            ClearGrass();
            meshes =
                GrassMeshGenerator.GenerateGrassMeshes(
                    terrainSampler,
                    patchSize,
                    bladesPerPatch,
                    grassMinHeight,
                    grassMaxHeight,
                    grassMinFade,
                    grassMaxFade,
                    bladeWidths,
                    bladeHeights,
                    grassMaterial,
                    orderedGrassTypeDensity,
                    grassOrder,
                    maxSlope
                );

            foreach (Mesh mesh in meshes)
            {
                GameObject grassObject =
                    new GameObject(mesh.name);

                grassObject.transform.SetParent(parent, false);

                MeshFilter meshFilter =
                    grassObject.AddComponent<MeshFilter>();

                MeshRenderer meshRenderer =
                    grassObject.AddComponent<MeshRenderer>();

                meshFilter.sharedMesh = mesh;

                meshRenderer.sharedMaterial = grassMaterial;

                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.receiveShadows = false;
            }
        }

        public void ClearGrass()
        {
            List<Transform> Meshes = new List<Transform>();
            Meshes.AddRange(parent.GetComponentsInChildren<Transform>());

            foreach (Transform grass in Meshes)
            {
                if (grass != parent)
                {
                    DestroyImmediate(grass.gameObject);
                }
            }
        }

        private void SortGrassDensity()
        {
            grassOrder = new List<int>(0);
            orderedGrassTypeDensity = new List<float>();
            orderedGrassTypeDensity.AddRange(grassTypesDensity);

            while (orderedGrassTypeDensity.Count > maxGrassTypes)
            {
                orderedGrassTypeDensity.RemoveAt(orderedGrassTypeDensity.Count - 1);
            }
            while (orderedGrassTypeDensity.Count < 4)
            {
                orderedGrassTypeDensity.Add(0);
            }

            float totalGrassDensity = 0;
            foreach (float i in orderedGrassTypeDensity)
            {
                totalGrassDensity += i;
            }

            if (totalGrassDensity > 100)
            {
                float amountToRemove = (totalGrassDensity - 100);
                float percentAmount = (amountToRemove * 100) / totalGrassDensity;

                for (int i = 0; i < maxGrassTypes; i++)
                {
                    orderedGrassTypeDensity[i] -= (orderedGrassTypeDensity[i] * percentAmount) / 100;
                    orderedGrassTypeDensity[i] = (int)orderedGrassTypeDensity[i];
                }
            }

            List<float> originalGrassOrder = new List<float>(orderedGrassTypeDensity.Count);
            originalGrassOrder.AddRange(orderedGrassTypeDensity);

            orderedGrassTypeDensity.Sort((a, b) => b.CompareTo(a));
            CheckIfDuplicateDensities(orderedGrassTypeDensity);

            foreach (float density in orderedGrassTypeDensity)
            {
                CheckIfDuplicateDensities(originalGrassOrder);
                for (int i = 0; i < originalGrassOrder.Count; i++)
                {
                    if (density == originalGrassOrder[i])
                    {
                        grassOrder.Add(i);
                        break;
                    }
                }
            }
        }

        private void CheckIfDuplicateDensities(List<float> numbers)
        {
            float indexToAdd = .1f;

            for (int i = 0; i < numbers.Count; i++)
            {
                for (int y = 0; y < numbers.Count; y++)
                {
                    if (i == y) continue;

                    if ((int)numbers[i] == (int)numbers[y])
                    {
                        if (numbers[i] % 1 == 0)
                        {
                            numbers[i] += indexToAdd;
                            indexToAdd += .1f;
                        }
                        if (numbers[y] % 1 == 0)
                        {
                            numbers[y] += indexToAdd;
                            indexToAdd += .1f;
                        }
                    }
                }
            }
        }

    }

}
