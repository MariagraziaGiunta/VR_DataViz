using UnityEngine;

[ExecuteAlways]
public class WallsPlacer : MonoBehaviour
{
    public GameObject wallPrefab; // assign a wall-shaped prefab (already sized)
    public float verticalOffset = 0f; // additional vertical offset for the wall base
    public float wallOutwardOffset = 0f; // move walls outward from the floor edge if needed
    public float wallHeight = 2f; // height of the walls (not used directly, assume prefab is sized accordingly)
    public float wallThickness = 0.05f; // thickness of the walls (not used directly, assume prefab is sized accordingly)

    const int WallCount = 4;
    const string WallNamePrefix = "Wall_";

    void OnValidate() => UpdateWalls();
    void Start() => UpdateWalls();

    [ContextMenu("Update Walls")]
    public void UpdateWalls()
    {
        if (wallPrefab == null) return;

        float pos = 5f; // default for Unity Plane 10x10
        float xScale = 10f; // default for Unity Plane 10x10

        // ensure four walls exist as children
        for (int i = 0; i < WallCount; i++)
        {
            Transform child = transform.Find(WallNamePrefix + i);
            GameObject wallObj;
            if (child == null)
            {
                wallObj = (GameObject)PrefabUtilitySafeInstantiate(wallPrefab, transform);
                wallObj.name = WallNamePrefix + i;
            }
            else
            {
                wallObj = child.gameObject;
            }

            Vector3 localPos = Vector3.zero;
            Vector3 localScale = Vector3.one;
            Quaternion localRot = Quaternion.identity;


            switch (i)
            {
                // Front (+Z)
                case 0:
                    localPos = new Vector3(0f, verticalOffset, pos + wallOutwardOffset);
                    localRot = Quaternion.identity;
                    break;
                // Back (-Z)
                case 1:
                    localPos = new Vector3(0f, verticalOffset, -pos - wallOutwardOffset);
                    localRot = Quaternion.identity;
                    break;
                // Right (+X)
                case 2:
                    localPos = new Vector3(pos + wallOutwardOffset, verticalOffset, 0f);
                    localRot = Quaternion.Euler(0f, 90f, 0f);
                    break;
                // Left (-X)
                default:
                    localPos = new Vector3(-pos - wallOutwardOffset, verticalOffset, 0f);
                    localRot = Quaternion.Euler(0f, 90f, 0f);
                    break;
            }

            // apply transform relative to floor
            wallObj.transform.SetParent(transform, false);
            wallObj.transform.localPosition = localPos;
            wallObj.transform.localRotation = localRot;
            wallObj.transform.localScale = new Vector3(xScale, wallHeight, wallThickness); ;
        }

        // remove any extra children created previously with same prefix but index >=4
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var c = transform.GetChild(i);
            if (c.name.StartsWith(WallNamePrefix))
            {
                if (!int.TryParse(c.name.Substring(WallNamePrefix.Length), out int idx) || idx >= WallCount)
                {
                    #if UNITY_EDITOR
                    if (Application.isPlaying) Destroy(c.gameObject); else UnityEditor.Undo.DestroyObjectImmediate(c.gameObject);
                    #else
                    Destroy(c.gameObject);
                    #endif
                }
            }
        }
    }

    // Helper: instantiate prefab safely in edit mode or play mode
    static GameObject PrefabUtilitySafeInstantiate(GameObject prefab, Transform parent)
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var obj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);
            return obj;
        }
        #endif
        return Object.Instantiate(prefab, parent);
    }
}