using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    protected Collider _collider;
    [SerializeField]
    protected GridCell _occupiedCell;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Vector3 vec = transform.rotation * Vector3.down;
        if (Physics.Raycast(_collider.bounds.center, vec, out RaycastHit hit, Mathf.Infinity, 1 << 3))
        {
            Debug.Log("Hits!");
            _occupiedCell = hit.transform.GetComponent<GridCell>();
        }
        Debug.Log("End of Entity Start!");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void FindPath(GridCell target) {

    }

    public void Move(GridCell target) {

    }
}
