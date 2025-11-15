using UnityEngine;

[ExecuteAlways]
public class FloorScaler : MonoBehaviour
{
    // Don't serialize a cross-scene reference.
    // Use autoFind to locate a SpawnerAuthoring at edit/runtime.
    public bool autoFindSpawner = true;
    SpawnerAuthoring spawnerAuthoring; // non-serialized, avoids cross-scene saved references

    public enum PlaneMeshType { UnityPlane_10x10, Quad_1x1 }
    public PlaneMeshType planeMesh = PlaneMeshType.UnityPlane_10x10;

    // Optional: move plane to spawner position + offsetY
    public bool matchSpawnerPosition = true;

    void OnValidate() => UpdateScale();
    void Start() => UpdateScale();

    void Update()
    {
        // Only update continuously if playing to follow runtime changes
        if (Application.isPlaying)
            UpdateScale();
    }

    [ContextMenu("Update Scale")]
    public void UpdateScale()
    {
        // Try to find the spawner when needed (edit & play) but do not serialize it
        if (spawnerAuthoring == null && autoFindSpawner)
            spawnerAuthoring = FindAnyObjectByType<SpawnerAuthoring>();

        if (spawnerAuthoring == null)
            return;

        var area = spawnerAuthoring.areaSize; // Vector2: x = width, y = depth
        float sx, sz;

        if (planeMesh == PlaneMeshType.UnityPlane_10x10)
        {
            // Unity built-in Plane is 10 units wide/deep
            sx = area.x / 10f;
            sz = area.y / 10f;
        }
        else
        {
            // Quad or custom mesh assumed 1x1
            sx = area.x;
            sz = area.y;
        }

        transform.localScale = new Vector3(sx, transform.localScale.y, sz);

        if (matchSpawnerPosition)
        {
            var spawnerPos = spawnerAuthoring.transform.position;
            transform.position = new Vector3(spawnerPos.x, spawnerPos.y + spawnerAuthoring.areaOffsetY, spawnerPos.z);
        }
    }
}