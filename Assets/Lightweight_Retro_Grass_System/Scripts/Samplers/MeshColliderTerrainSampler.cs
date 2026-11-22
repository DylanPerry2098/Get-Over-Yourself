using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace grassGeneration
{
    public class MeshColliderTerrainSampler : TerrainSampler
    {
        [SerializeField] private Collider terrainCollider;

        [SerializeField] private float raycastHeight = 1000f;
        [SerializeField] private float raycastDistance = 2000f;

        [SerializeField] private LayerMask terrainLayer = ~0;

        public override Bounds GetBounds()
        {
            if (terrainCollider == null)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            Bounds worldBounds = terrainCollider.bounds;

            Vector3 localMin =
                transform.InverseTransformPoint(worldBounds.min);

            Vector3 localMax =
                transform.InverseTransformPoint(worldBounds.max);

            Vector3 center = (localMin + localMax) * 0.5f;

            Vector3 size = new Vector3(
                Mathf.Abs(localMax.x - localMin.x),
                0f,
                Mathf.Abs(localMax.z - localMin.z)
            );

            return new Bounds(center, size);
        }

        public override bool TrySample(
            Vector3 localPosition,
            out TerrainSample sample)
        {
            sample = default;

            if (terrainCollider == null)
            {
                return false;
            }

            Vector3 worldPosition =
                transform.TransformPoint(localPosition);

            Vector3 rayOrigin =
                worldPosition +
                Vector3.up * raycastHeight;

            QueryParameters queryParameters =
                new QueryParameters(
                    terrainLayer,
                    false,
                    QueryTriggerInteraction.Ignore,
                    false
                );

            RaycastCommand command =
                new RaycastCommand(
                    rayOrigin,
                    Vector3.down,
                    queryParameters,
                    raycastDistance
                );

            NativeArray<RaycastCommand> commands =
                new NativeArray<RaycastCommand>(
                    1,
                    Allocator.TempJob
                );

            NativeArray<RaycastHit> results =
                new NativeArray<RaycastHit>(
                    1,
                    Allocator.TempJob
                );

            commands[0] = command;

            JobHandle handle =
                RaycastCommand.ScheduleBatch(
                    commands,
                    results,
                    1,
                    1,
                    default
                );

            handle.Complete();

            RaycastHit hit = results[0];

            commands.Dispose();
            results.Dispose();

            if (hit.collider == null)
            {
                return false;
            }

            Vector3 localHitPoint =
                transform.InverseTransformPoint(hit.point);

            Vector3 localNormal =
                transform.InverseTransformDirection(hit.normal)
                    .normalized;

            sample = new TerrainSample(
                localHitPoint,
                localNormal
            );

            return true;
        }

        public override void SampleBatch(
            Vector3[] localPositions,
            TerrainSample[] samples,
            bool[] valid)
        {
            int count = localPositions.Length;

            NativeArray<RaycastCommand> commands =
                new NativeArray<RaycastCommand>(
                    count,
                    Allocator.TempJob
                );

            NativeArray<RaycastHit> results =
                new NativeArray<RaycastHit>(
                    count,
                    Allocator.TempJob
                );

            QueryParameters queryParameters =
                new QueryParameters(
                    terrainLayer,
                    false,
                    QueryTriggerInteraction.Ignore,
                    false
                );

            for (int i = 0; i < count; i++)
            {
                Vector3 worldPosition =
                    transform.TransformPoint(
                        localPositions[i]
                    );

                Vector3 rayOrigin =
                    worldPosition +
                    Vector3.up * raycastHeight;

                commands[i] = new RaycastCommand(
                    rayOrigin,
                    Vector3.down,
                    queryParameters,
                    raycastDistance
                );
            }

            JobHandle handle =
                RaycastCommand.ScheduleBatch(
                    commands,
                    results,
                    64,
                    1,
                    default
                );

            handle.Complete();

            for (int i = 0; i < count; i++)
            {
                RaycastHit hit = results[i];

                if (hit.collider == null)
                {
                    valid[i] = false;
                    samples[i] = default;
                    continue;
                }

                Vector3 localHitPoint =
                    transform.InverseTransformPoint(
                        hit.point
                    );

                Vector3 localNormal =
                    transform.InverseTransformDirection(
                        hit.normal
                    ).normalized;

                samples[i] = new TerrainSample(
                    localHitPoint,
                    localNormal
                );

                valid[i] = true;
            }

            commands.Dispose();
            results.Dispose();
        }
    }
}

