using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField]
    private GameObject levelObject;
    [SerializeField]
    private GameObject gridCellPrefab;

    private List<GameObject> levelGrid;

    //private int height = 10;
    //private int width = 10;
    private float gridSpaceSize = 1f;

    // Start is called before the first frame update
    void Start()
    {
        //CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CallCreateGrid(Transform transform) {
        int numChild = transform.childCount;
        if (numChild <= 0) {
            return;
        }

        for (int i = 0; i < numChild; i++) {
            Transform childTransform = transform.GetChild(i);
            MeshRenderer meshRend = childTransform.gameObject.GetComponent<MeshRenderer>();
            //_levelGrid.CreateGrid((int)meshRend.bounds.size.x, (int)meshRend.bounds.size.z);
        }
    }

    // public void CreateGrid(int height, int width)
    // {
    //     if (levelGrid == null) {
    //         levelGrid = new List<GameObject>();
    //     }

    //     if (gridCellPrefab == null) {
    //         Debug.LogError("ERROR: Null Grid Cell Prefab");
    //         return;
    //     }

    //     for (int x = 0; x < width; x++) {
    //         for (int y = 0; y < height; y++) {
    //             levelGrid[x,y] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, y * gridSpaceSize), Quaternion.identity);
                
    //             levelGrid[x,y].GetComponent<GridCell>().SetPosition(x,y);
    //             levelGrid[x,y].transform.parent = transform;
    //             levelGrid[x,y].gameObject.name = $"Grid Space (X: {x}, Y: {y})";
    //         }
    //     }
    // }
}
