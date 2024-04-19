using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject _uiSlotPrefab;
    [SerializeField]
    private GameObject _smallSlots;
    [SerializeField]
    private int _maxSmallSlots = 3;
    [SerializeField]
    private List<InventorySlot> _inventorySlots;
    public List<InventorySlot> InventorySlots {
        get => _inventorySlots;
    }
    public bool IsFull {
        get => _inventorySlots.Count < _maxSmallSlots;
    }


    public void Start()
    {
        _inventorySlots = new();
        if (_smallSlots == null) {
            throw new Exception("Small Slots gameObject not defined. Pull from InventoryCanvas.");
        }
    }

    public void addItemToInventory(SmallHoldableObject newObject)
    {
        if (_inventorySlots.Count >= _maxSmallSlots) {
            Debug.Log("DON'T ADD, INVENTORY FULL");
            return;
        }

        GameObject slot = Instantiate(_uiSlotPrefab, _smallSlots.transform);
        InventorySlot iSlot = slot.GetComponent<InventorySlot>();
        
        iSlot.SetObject(newObject);
    }

    private void OnEnable()
    {
        SmallHoldableObject.pickedUp += addItemToInventory;
    }
}
