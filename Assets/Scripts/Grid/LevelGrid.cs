using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class LevelGrid : MonoBehaviour
{
    [Header("Debug Grid")]
    [SerializeField]
    private GameObject corner1;
    [SerializeField]
    private GameObject corner2;

    [Header("Creating Grid")]

    [SerializeField]
    private float gridSpaceSize = 1f;
    [SerializeField]
    private GameObject levelObject;
    [SerializeField]
    private GameObject gridCellPrefab;

    private Dictionary<GridCellPosition, GridCell> gridCellExistence;
    private Dictionary<GridCell, List<GridCell>> grid;
    

    // Start is called before the first frame update
    void Start()
    {
        //DrawGridOutline();
        gridCellExistence = new Dictionary<GridCellPosition, GridCell>();
        grid = new Dictionary<GridCell, List<GridCell>>();

        CallCreateGrid(levelObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cornerTemp1 = corner1.transform.position;
        Vector3 cornerTemp2 = corner2.transform.position;

        Vector3 cornerPos1 = new Vector3(Mathf.Min(cornerTemp1.x, cornerTemp2.x), Mathf.Min(cornerTemp1.y, cornerTemp2.y), Mathf.Min(cornerTemp1.z, cornerTemp2.z));
        Vector3 cornerPos2 = new Vector3(Mathf.Max(cornerTemp1.x, cornerTemp2.x), Mathf.Max(cornerTemp1.y, cornerTemp2.y), Mathf.Max(cornerTemp1.z, cornerTemp2.z));
        //DrawGridOutline();
        Debug.DrawRay(new Vector3(cornerPos2.x, cornerPos1.y + (gridSpaceSize / 2.0f), cornerPos1.z + (gridSpaceSize / 2.0f)), Vector3.right);
    }

    private void OnDrawGizmosSelected() {
        if (corner1 == null || corner2 == null || gridSpaceSize < 0.5) {
            return;
        }

        Vector3 cornerTemp1 = corner1.transform.position;
        Vector3 cornerTemp2 = corner2.transform.position;

        Vector3 cornerPos1 = new Vector3(Mathf.Max(cornerTemp1.x, cornerTemp2.x), Mathf.Max(cornerTemp1.y, cornerTemp2.y), Mathf.Max(cornerTemp1.z, cornerTemp2.z));
        Vector3 cornerPos2 = new Vector3(Mathf.Min(cornerTemp1.x, cornerTemp2.x), Mathf.Min(cornerTemp1.y, cornerTemp2.y), Mathf.Min(cornerTemp1.z, cornerTemp2.z));

        for (float x = cornerPos2.x; x <= cornerPos1.x; x += gridSpaceSize) {
            for (float y = cornerPos2.y; y <= cornerPos1.y; y += gridSpaceSize) {
                Vector3 start = new Vector3(x, y, cornerPos2.z);
                Vector3 end = new Vector3(x, y, cornerPos1.z);
                Gizmos.color = new Color(0, 0, 1, 0.5f);
                Gizmos.DrawLine(start, end);
            }
        }
        for (float x = cornerPos2.x; x <= cornerPos1.x; x += gridSpaceSize) {
            for (float z = cornerPos2.z; z <= cornerPos1.z; z += gridSpaceSize) {
                Vector3 start = new Vector3(x, cornerPos2.y, z);
                Vector3 end = new Vector3(x, cornerPos1.y, z);
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawLine(start, end);
            }
        }
        for (float y = cornerPos2.y; y <= cornerPos1.y; y += gridSpaceSize) {
            for (float z = cornerPos2.z; z <= cornerPos1.z; z += gridSpaceSize) {
                Vector3 start = new Vector3(cornerPos2.x, y, z);
                Vector3 end = new Vector3(cornerPos1.x, y, z);
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawLine(start, end);
            }
        }
    }
    
    private void CallCreateGrid(Transform transform) {
        int numChild = transform.childCount;
        if (numChild <= 0) {
            return;
        }

        if (gridCellPrefab == null) {
            Debug.LogError("ERROR: Null Grid Cell Prefab");
            return;
        }


        // for (int i = 0; i < numChild; i++) {
        //     Transform childTransform = transform.GetChild(i);
        //     MeshRenderer meshRend = childTransform.gameObject.GetComponent<MeshRenderer>();
        //     CreateGrid(childTransform, meshRend);
        // }

        CreateGrid();
    }

    private void CreateGrid() {
        if (corner1 == null || corner2 == null) {
            return;
        }

        Vector3 cornerTemp1 = corner1.transform.position;
        Vector3 cornerTemp2 = corner2.transform.position;

        Vector3 cornerPos1 = new Vector3(Mathf.Min(cornerTemp1.x, cornerTemp2.x), Mathf.Min(cornerTemp1.y, cornerTemp2.y), Mathf.Min(cornerTemp1.z, cornerTemp2.z));
        Vector3 cornerPos2 = new Vector3(Mathf.Max(cornerTemp1.x, cornerTemp2.x), Mathf.Max(cornerTemp1.y, cornerTemp2.y), Mathf.Max(cornerTemp1.z, cornerTemp2.z));

        float offset = gridSpaceSize / 2.0f;
        //DOWN (facing UP) xz
        for (float x = cornerPos1.x; x < cornerPos2.x; x += gridSpaceSize) {
            for (float z = cornerPos1.z; z < cornerPos2.z; z += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(x + offset, cornerPos2.y, z + offset), Vector3.down);
                float y = (Mathf.Round((cornerPos2.y - cornerPos1.y) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, y, 1);
                if (hits.Length == 0) {
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    Vector3 pos = hit.point;
                    float yf = RoundToNearest(pos.y);
                    CreateGridCell(new Vector3(x + offset, yf, z + offset), Quaternion.identity, GridCellPositionEnum.BOTTOM);
                }
            }
        }
        //FRONT (facing BACK) yz
        for (float x = cornerPos1.x; x < cornerPos2.x; x += gridSpaceSize) {
            for (float y = cornerPos1.y; y < cornerPos2.y; y += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(x + offset, y + offset, cornerPos1.z), Vector3.forward);
                float z = (Mathf.Round((cornerPos2.z - cornerPos1.z) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, z, 1);
                if (hits.Length == 0) {
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    Vector3 pos = hit.point;
                    float zf = RoundToNearest(pos.z);
                    CreateGridCell(new Vector3(x + offset, y + offset, zf), Quaternion.Euler(-90, 0, 0), GridCellPositionEnum.FRONT);
                }
            }
        }
        //BACK (facing FRONT) yz
        for (float x = cornerPos1.x; x < cornerPos2.x; x += gridSpaceSize) {
            for (float y = cornerPos1.y; y < cornerPos2.y; y += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(x + offset, y + offset, cornerPos2.z), Vector3.back);
                float z = (Mathf.Round((cornerPos2.z - cornerPos1.z) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, z, 1);
                if (hits.Length == 0) {
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    Vector3 pos = hit.point;
                    float zf = RoundToNearest(pos.z);
                    CreateGridCell(new Vector3(x + offset, y + offset, zf), Quaternion.Euler(-90, 0, -180), GridCellPositionEnum.BACK);
                }
            }
        }
        //RIGHT (facing LEFT) xy
        for (float y = cornerPos1.y; y < cornerPos2.y; y += gridSpaceSize) {
            for (float z = cornerPos1.z; z < cornerPos2.z; z += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(cornerPos1.x, y + offset, z + offset), Vector3.right);
                float x = (Mathf.Round((cornerPos2.x - cornerPos1.x) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, x, 1);
                if (hits.Length == 0) {
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    Vector3 pos = hit.point;
                    float xf = RoundToNearest(pos.x);
                    CreateGridCell(new Vector3(xf, y + offset, z + offset), Quaternion.Euler(-90, 0, 90), GridCellPositionEnum.RIGHT);
                }
            }
        }
        //LEFT (facing RIGHT) xy
        for (float y = cornerPos1.y; y < cornerPos2.y; y += gridSpaceSize) {
            for (float z = cornerPos1.z; z < cornerPos2.z; z += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(cornerPos2.x, y + offset, z + offset), Vector3.left);
                float x = (Mathf.Round((cornerPos2.x - cornerPos1.x) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, x, 1);
                if (hits.Length == 0) {
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    if (hits.Length == 2) {
                        Debug.Log(hit.collider.gameObject.name);
                    }
                    Vector3 pos = hit.point;
                    float xf = RoundToNearest(pos.x);
                    CreateGridCell(new Vector3(xf, y + offset, z + offset), Quaternion.Euler(-90, 0, -90), GridCellPositionEnum.LEFT);
                }
            }
        }
    }

    private void CreateGridCell(Vector3 pos, Quaternion rot, GridCellPositionEnum posE) {
        GameObject temp = Instantiate(gridCellPrefab, pos, rot);
        temp.transform.parent = gameObject.transform;
        GridCell gridCell = temp.GetComponent<GridCell>();
        gridCell.Position = new GridCellPosition(pos, posE);
        gridCell.PositionE = posE;
        grid.Add(gridCell, new List<GridCell>());
        switch (posE)
        {
            case GridCellPositionEnum.BOTTOM:
                temp.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                break;
            case GridCellPositionEnum.FRONT:
                temp.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.magenta;
                break;
            case GridCellPositionEnum.BACK:
                temp.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.cyan;
                break;
            case GridCellPositionEnum.RIGHT:
                temp.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
                break;
            case GridCellPositionEnum.LEFT:
                temp.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;
                break;
            default:
                temp.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.black;
                break;
        }
        // = Color.blue;
    }

    private void ConnectGraph(GridCell gridCell) {
        Vector3 position = gridCell.Position.Position;
        Quaternion rotation = gridCell.gameObject.transform.rotation;
        Vector3 rotVec = rotation * Vector3.forward;

        Collider[] colliders = Physics.OverlapSphere(position, gridSpaceSize, 1 << 3);
        foreach (Collider collider in colliders) {
            if (collider.Equals(gridCell.Collider)) {
                continue;
            }
        }

    }

    private void CheckBottomGraph(GridCell gridCell) {
        Vector3 position = gridCell.Position.Position;
        Quaternion rotation = gridCell.gameObject.transform.rotation;
        // Vector3 topEdge = new Vector3(position.x, position.y, position.z + gridSpaceSize);
        // Vector3 bottomEdge = position;
        // Vector3 leftEdge = position;
        // Vector3 rightEdge = new Vector3(position.x + gridSpaceSize, position.y, position.z);

        // GridCellPosition bottomPos = new GridCellPosition(new Vector3(topEdge.x, topEdge.y, topEdge.z + gridSpaceSize), GridCellPositionEnum.BOTTOM);
        // GridCellPosition frontPos = new GridCellPosition(topEdge, GridCellPositionEnum.FRONT);
        // GridCellPosition backPos = new GridCellPosition(new Vector3(topEdge.x, topEdge.y - gridSpaceSize, topEdge.z), GridCellPositionEnum.BACK);
        
        // //TopEdge
        // if (gridCellExistence.ContainsKey(bottomPos)) {
        //     AddToGraph(gridCell, bottomPos);
        // } else if (gridCellExistence.ContainsKey(frontPos)) {
        //     AddToGraph(gridCell, frontPos);
        // } else if (gridCellExistence.ContainsKey(backPos)) {
        //     AddToGraph(gridCell, backPos);
        // }
        //BottomEdge
    }

    private void AddToGraph(GridCell gridCell, GridCellPosition gridCellPosition) {
        GridCell temp = gridCellExistence[gridCellPosition];
        grid[temp].Add(gridCell);
        grid[gridCell].Add(temp);
    }

    private float RoundToNearest(float num) {
        float numRound = Mathf.Round(num * 100f) / 100f;
        float r = numRound % gridSpaceSize;
        return r >= gridSpaceSize / 2.0f ? (numRound - r) + gridSpaceSize : numRound - r;
    }
}
