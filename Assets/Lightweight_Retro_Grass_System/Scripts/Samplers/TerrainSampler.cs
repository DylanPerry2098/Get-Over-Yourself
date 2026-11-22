using UnityEngine;

namespace grassGeneration
{
    public abstract class TerrainSampler : MonoBehaviour
    {
        public abstract Bounds GetBounds();

        public abstract bool TrySample(
            Vector3 localPosition,
            out TerrainSample sample
        );

        public virtual void SampleBatch(
            Vector3[] localPositions,
            TerrainSample[] samples,
            bool[] valid)
        {
            for (int i = 0; i < localPositions.Length; i++)
            {
                valid[i] = TrySample(
                    localPositions[i],
                    out samples[i]
                );
            }
        }
    }
}

