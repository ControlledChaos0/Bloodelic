using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PseudoDataStructures;
using UnityEngine;
public class LevelGrid : MonoBehaviour
{
    // public static LevelGrid S_LevelGrid {
    //     get => s_levelGrid;
    //     set => s_levelGrid = value;
    // }
    // private static LevelGrid s_levelGrid;

    [SerializeField]
    private GameObject corner1;
    [SerializeField]
    private GameObject corner2;
    [SerializeField]
    private float gridSpaceSize = 1f;
    [SerializeField]
    private GameObject gridCellPrefab;
    [SerializeField]
    private PseudoDictionary<GridCellPosition, GridCell> pGridCellExistence;
    [SerializeField]
    private PseudoDictionaryArray<GridCell, GridCell> pGrid;

    public Dictionary<GridCell, GridCell[]> Grid {
        get => grid;
        private set => grid = value;
    }

    private Dictionary<GridCellPosition, GridCell> gridCellExistence;
    private Dictionary<GridCell, GridCell[]> grid;
    private int layerMask = 1 << 6;

    //TESTING VARIABLES
    private string testName = "GridCell; Position: 0.5, 0.5, 0; Enum: FRONT";
    private GridCell testGridCell;
    

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"If this is 0 im gonna fucking lose it: {pGridCellExistence.Keys.Length}");
        // S_LevelGrid = this;

        grid = pGrid.ToDictionary();
        gridCellExistence = pGridCellExistence.ToDictionary();
        Debug.Log($"What is going on: {gridCellExistence.Keys.Count}");
        GridManager.SetLevelGrid(gameObject);
        DebugGrid();
        //TurnAllWhite(testGridCell);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void CallCreateGrid() {
        if (gridCellPrefab == null) {
            Debug.LogError("ERROR: Null Grid Cell Prefab");
            return;
        }
        gridCellExistence = new();
        grid = new();
        CreateGrid();
        ConnectGridCells();
        pGrid = new(grid);
        pGridCellExistence = new(gridCellExistence);
    }
    public void ClearGrid() {
        gridCellExistence = new();
        grid = new();
        pGridCellExistence = new();
        pGrid = new();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    private void CreateGrid() {
        if (corner1 == null || corner2 == null) {
            return;
        }

        Debug.Log("sdaflkj;ljksdfaadsf");

        Vector3 cornerTemp1 = corner1.transform.position;
        Vector3 cornerTemp2 = corner2.transform.position;

        Vector3 cornerPos1 = new Vector3(Mathf.Min(cornerTemp1.x, cornerTemp2.x), Mathf.Min(cornerTemp1.y, cornerTemp2.y), Mathf.Min(cornerTemp1.z, cornerTemp2.z));
        Vector3 cornerPos2 = new Vector3(Mathf.Max(cornerTemp1.x, cornerTemp2.x), Mathf.Max(cornerTemp1.y, cornerTemp2.y), Mathf.Max(cornerTemp1.z, cornerTemp2.z));

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);

        float offset = gridSpaceSize / 2.0f;
        //DOWN (facing UP) xz
        for (float x = cornerPos1.x; x < cornerPos2.x; x += gridSpaceSize) {
            for (float z = cornerPos1.z; z < cornerPos2.z; z += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(x + offset, cornerPos2.y, z + offset), Vector3.down);
                float y = (Mathf.Round((cornerPos2.y - cornerPos1.y) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, y, layerMask);
                if (hits.Length == 0) {
                    Debug.Log(y);
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    Vector3 pos = hit.point;
                    float yf = RoundToNearest(pos.y);
                    bool check = Physics.Raycast(new Ray(new Vector3(x + offset, yf - offset, z + offset), Vector3.up), gridSpaceSize, layerMask);
                    if (!check) {
                        //Debug.Log("Trying to test");
                        CreateGridCell(new Vector3(x + offset, yf, z + offset), Quaternion.identity, GridCellPositionEnum.BOTTOM);
                    }
                }
            }
        }
        //FRONT (facing BACK) yz
        for (float x = cornerPos1.x; x < cornerPos2.x; x += gridSpaceSize) {
            for (float y = cornerPos1.y; y < cornerPos2.y; y += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(x + offset, y + offset, cornerPos1.z), Vector3.forward);
                float z = (Mathf.Round((cornerPos2.z - cornerPos1.z) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, z, layerMask);
                if (hits.Length == 0) {
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    Vector3 pos = hit.point;
                    float zf = RoundToNearest(pos.z);
                    bool check = Physics.Raycast(new Ray(new Vector3(x + offset, y + offset, zf + offset), Vector3.back), gridSpaceSize, layerMask);
                    if (!check) {
                        CreateGridCell(new Vector3(x + offset, y + offset, zf), Quaternion.Euler(-90, 0, 0), GridCellPositionEnum.FRONT);
                    }
                }
            }
        }
        //BACK (facing FRONT) yz
        for (float x = cornerPos1.x; x < cornerPos2.x; x += gridSpaceSize) {
            for (float y = cornerPos1.y; y < cornerPos2.y; y += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(x + offset, y + offset, cornerPos2.z), Vector3.back);
                float z = (Mathf.Round((cornerPos2.z - cornerPos1.z) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, z, layerMask);
                if (hits.Length == 0) {
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    Vector3 pos = hit.point;
                    float zf = RoundToNearest(pos.z);
                    bool check = Physics.Raycast(new Ray(new Vector3(x + offset, y + offset, zf - offset), Vector3.forward), gridSpaceSize, layerMask);
                    if (!check) {
                        CreateGridCell(new Vector3(x + offset, y + offset, zf), Quaternion.Euler(-90, 0, 180), GridCellPositionEnum.BACK);
                    }
                }
            }
        }
        //RIGHT (facing LEFT) xy
        for (float y = cornerPos1.y; y < cornerPos2.y; y += gridSpaceSize) {
            for (float z = cornerPos1.z; z < cornerPos2.z; z += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(cornerPos1.x, y + offset, z + offset), Vector3.right);
                float x = (Mathf.Round((cornerPos2.x - cornerPos1.x) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, x, layerMask);
                if (hits.Length == 0) {
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    Vector3 pos = hit.point;
                    float xf = RoundToNearest(pos.x);
                    bool check = Physics.Raycast(new Ray(new Vector3(xf + offset, y + offset, z + offset), Vector3.left), gridSpaceSize, layerMask);
                    if (!check) {
                        CreateGridCell(new Vector3(xf, y + offset, z + offset), Quaternion.Euler(-90, 0, 90), GridCellPositionEnum.RIGHT);
                        if ((new Vector3(-3f, 1.5f, 2.5f)).Equals(new Vector3(xf, y + offset, z + offset))) {
                            Debug.Log($"Hit Position: {pos}");
                        }
                    }
                }
            }
        }
        //LEFT (facing RIGHT) xy
        for (float y = cornerPos1.y; y < cornerPos2.y; y += gridSpaceSize) {
            for (float z = cornerPos1.z; z < cornerPos2.z; z += gridSpaceSize) {
                Ray ray = new Ray(new Vector3(cornerPos2.x, y + offset, z + offset), Vector3.left);
                float x = (Mathf.Round((cornerPos2.x - cornerPos1.x) * 100f) / 100f) + 0.001f;
                RaycastHit[] hits = Physics.RaycastAll(ray, x, layerMask);
                if (hits.Length == 0) {
                    continue;
                }
                foreach (RaycastHit hit in hits)
                {
                    if (hits.Length == 2) {
                        //Debug.Log(hit.collider.gameObject.name);
                    }
                    Vector3 pos = hit.point;
                    float xf = RoundToNearest(pos.x);
                    bool check = Physics.Raycast(new Ray(new Vector3(xf - offset, y + offset, z + offset), Vector3.right), gridSpaceSize, layerMask);
                    if (!check) {
                        //Debug.Log("WWWWWWWWWWWWWWWW");
                        CreateGridCell(new Vector3(xf, y + offset, z + offset), Quaternion.Euler(-90, 0, -90), GridCellPositionEnum.LEFT);
                    }
                }
            }
        }
    }

    private void CreateGridCell(Vector3 pos, Quaternion rot, GridCellPositionEnum posE) {
        GameObject temp = Instantiate(gridCellPrefab, pos, rot);
        temp.transform.parent = gameObject.transform;
        temp.name = $"GridCell; Position: {pos.x}, {pos.y}, {pos.z}; Enum: {posE}";
        GridCell gridCell = temp.GetComponent<GridCell>();
        gridCell.Position = new GridCellPosition(pos, posE);
        gridCellExistence.Add(gridCell.Position, gridCell);
        grid.Add(gridCell, new GridCell[4]);
    }

    private void ConnectGridCells() {
        GridCell[] allGridCells = grid.Keys.ToArray();
        Debug.Log(allGridCells.Length);
        foreach (GridCell gridCell in allGridCells) {
            if (gridCell.gameObject.name == testName) {
                testGridCell = gridCell;
            }
            ConnectGraph(gridCell);
        }
    }

    private void ConnectGraph(GridCell gridCell) {
        bool testing = false;
        if (gridCell.Equals(testGridCell)) {
            testing = true;
        }

        Vector3 position = gridCell.Position.Position;
        if (testing) {
             Debug.Log($"TEST GRID CELL: {gridCell.gameObject.name}");
        }
        Quaternion rotation = gridCell.gameObject.transform.rotation;
        Quaternion invertRot = Quaternion.Inverse(rotation);
        Vector3 rotVec = rotation * Vector3.forward;

        //2D array: Second num is edge, First num is whether they line up with the enum or not
        GridCell[,] gridCellConnections = new GridCell[2,4];
        Dictionary<GridCell, float> distanceChart = new();

        Collider[] colliders = Physics.OverlapSphere(position, gridSpaceSize, 1 << 3);
        // if (testing) {
        //     Debug.Log($"Collider Length: {colliders.Length}");
        // }
        foreach (Collider collider in colliders) {
            if (testing) {
                Debug.Log("-------------------------------------------------");
                Debug.Log(collider.gameObject.name);
            }
            if (collider.Equals(gridCell.Collider)) {
                continue;
            }
            GridCell otherGridCell = collider.gameObject.GetComponent<GridCell>();
            Vector3 dirToCol = collider.gameObject.transform.position - position;
            if (testing) {
                Debug.Log($"Position: {collider.gameObject.transform.position}");
            }
            Vector3 correctDir = invertRot * dirToCol;
            float roundX = Mathf.Round(correctDir.x * 100f) / 100f;
            float roundY = Mathf.Round(correctDir.y * 100f) / 100f;
            float roundZ = Mathf.Round(correctDir.z * 100f) / 100f;
            if (testing) {
                //Debug.Log($"Corrected Position: {correctDir}");
                Debug.Log($"Double Check: x,{roundX}; y,{roundY}; z,{roundZ}");
            }
            int edgeNum = -1;
            if ((roundX != 0 && roundZ != 0) || (roundX == 0 && roundZ == 0)) {
                if (testing) {
                    Debug.Log("Skipping!");
                }
                continue;
            }

            /*
            Edge Num goes 0-3 corresponding to edge direction
            0 - TOP
            1 - BOTTOM
            2 - LEFT
            3 - RIGHT
            */
            if (roundZ > 0) {
                edgeNum = 0;
            } else if (roundZ < 0) {
                edgeNum = 1;
            } else if (roundX < 0) {
                edgeNum = 2;
            } else if (roundX > 0) {
                edgeNum = 3;
            } 

            if (testing) {
                if (edgeNum == -1) {
                    Debug.Log("BRUH");
                }
                //Debug.Log($"Enum: {(int)gridCell.PositionE}; EdgeNum: {edgeNum}; Y: {correctDir.y}");
            }

            GridCellPositionEnum corPosition;
            if (roundY == 0) {
                corPosition = ConstantValues.GetPositionalArray((int)gridCell.Position.PositionE, edgeNum, 0);
            } else if (roundY > 0) {
                corPosition = ConstantValues.GetPositionalArray((int)gridCell.Position.PositionE, edgeNum, 1);
            } else {
                corPosition = ConstantValues.GetPositionalArray((int)gridCell.Position.PositionE, edgeNum, 2);
            }
            
            bool connectEdge = false;
            GridCell gridCellConnection = gridCellConnections[1,edgeNum];
            if (gridCellConnection == null) {
                connectEdge = true;
            } else if (distanceChart.Count != 0) {
                if (distanceChart[gridCellConnection] > dirToCol.magnitude) {
                    connectEdge = true;
                } else if (distanceChart[gridCellConnection] == dirToCol.magnitude) {
                    if (!gridCellConnection.Equals(gridCellConnections[0,edgeNum])) {
                        connectEdge = true;
                    } else if (otherGridCell.Position.PositionE == ConstantValues.GetPositionalArray((int)gridCell.Position.PositionE, edgeNum, 1)) {
                        connectEdge = true;
                    } else {
                        if (testing) {
                            Debug.Log("Lol lmao");
                        }
                    }
                }
            } else {
                if (testing) {
                    Debug.Log("All checks failed.");
                }
                continue;
            }

            if (connectEdge) {
                if (testing) {
                    Debug.Log($"EDGE {edgeNum} CONNECTED");
                }
                gridCellConnections[0,edgeNum] = otherGridCell;
                gridCellConnections[1,edgeNum] = otherGridCell;
                if (gridCellConnections[0,edgeNum].Position.PositionE != corPosition) {
                    if (testing) {
                        Debug.Log("SIKE");
                    }
                    gridCellConnections[0,edgeNum] = null;
                }
            }
            
            distanceChart.Add(otherGridCell, dirToCol.magnitude);
            if (testing) {
                Debug.Log($"Magnitude: {dirToCol.magnitude}");
            }
        }

        if (testing) {
            Debug.Log("-------------------------------------------------");
            Debug.Log($"{gridCellConnections[0,0]}, {gridCellConnections[0,1]}, {gridCellConnections[0,2]}, {gridCellConnections[0,3]}");
            Debug.Log($"{gridCellConnections[1,0]}, {gridCellConnections[1,1]}, {gridCellConnections[1,2]}, {gridCellConnections[1,3]}");
        }

        bool isEmpty = true;
        bool destroyed = false;
        for(int x = 0; x < 4; x++) {
            if (gridCellConnections[0,x] == null) {
                if (gridCellConnections[1,x] != null) {
                    if (testing) {
                        Debug.Log("DESTROYED 1");
                    }
                    Destroy(gridCell.gameObject);
                    destroyed = true;
                }
                continue;
            }
            if (gridCellConnections[1,x] != null && !gridCellConnections[1,x].Equals(gridCellConnections[0,x])) {
                if (testing) {
                    Debug.Log("DESTROYED 2");
                }
                Destroy(gridCell.gameObject);
                destroyed = true;
                continue;
            }
            isEmpty = false;
            grid[gridCell][x] = gridCellConnections[0,x];
        }
        gridCell.Neighbors = grid[gridCell];
        if (isEmpty) {
            if (testing) {
                Debug.Log("DESTROYED 3");
            }
            Destroy(gridCell.gameObject);
            destroyed = true;
        }
        if (destroyed) {
            grid.Remove(gridCell);
            gridCellExistence.Remove(gridCell.Position);
            Debug.Log("wait wtf this actually runs?");
        }
    }

    private void DebugGrid() {
        foreach(KeyValuePair<GridCellPosition, GridCell> pair in gridCellExistence) {
            GameObject temp = pair.Value.gameObject;
            GridCellPositionEnum posE = pair.Key.PositionE;
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
        }
    }
    // private void AddToGraph(GridCell gridCell, GridCellPosition gridCellPosition) {
    //     GridCell temp = gridCellExistence[gridCellPosition];
    //     grid[temp].Add(gridCell);
    //     grid[gridCell].Add(temp);
    // }

    private float RoundToNearest(float num) {
        float numRound = Mathf.Round(num * 100f) / 100f;
        float r = Mathf.Abs(numRound) % gridSpaceSize;
        return r >= gridSpaceSize / 2.0f ? Mathf.Sign(num) * (Mathf.Abs(numRound) - r + gridSpaceSize) : Mathf.Sign(num) * (Mathf.Abs(numRound) - r);
    }

    //TEST METHODS
    private void TurnAllWhite(GridCell gridCell) {
        gridCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
        GridCell[] list = grid[gridCell];
        foreach (GridCell connectedGridCell in list) {
            if (!connectedGridCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color.Equals(Color.blue)) {
                TurnAllWhite(connectedGridCell);
            }
        }
    }

    public void TurnSurroundingBlue(GridCell gridCell) {
        gridCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
        GridCell[] list = grid[gridCell];
        Debug.Log($"Length of List: {list.Length}");
        foreach (GridCell connectedGridCell in list) {
            connectedGridCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
        }
    }
}