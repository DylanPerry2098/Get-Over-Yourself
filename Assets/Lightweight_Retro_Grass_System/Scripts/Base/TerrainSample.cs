using UnityEngine;

namespace grassGeneration
{
    public struct TerrainSample
    {
        public Vector3 position;
        public Vector3 normal;

        public TerrainSample(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
        }
    }
}
