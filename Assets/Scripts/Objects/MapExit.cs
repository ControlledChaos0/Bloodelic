using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class MapExit : MonoBehaviour
{
    public GridCell cell;

    public bool isInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        RegisterGridCell();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterGridCell()
    {
        float traceHeightBuffer = 0.5f;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, 1 << 3))
        {
            cell = hit.transform.GetComponent<GridCell>();
            // Move object to cell transform
            transform.position = hit.transform.position + Vector3.up * 0.001f;
        }

        isInitialized = true;
    }
}
