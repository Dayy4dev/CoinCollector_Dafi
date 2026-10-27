using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, GrabableItem
{
    InputAction inventoryAction;
    [SerializeField] private GameObject InventoryCanvas;
    [SerializeField] private int slotCount = 16;
    [SerializeField] private ItemDetailsPanel detailsPanel;
    private bool InventoryActive;
    public ItemSlot[] itemSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateSlots();
        InventoryCanvas.SetActive(false);
        inventoryAction = InputSystem.actions.FindAction("Inventory");
        InventoryActive = false;

        if (detailsPanel == null)
        {
            Debug.LogWarning("No ItemDetailsPanel is assigned, item details will not be shown.", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inventoryAction.triggered && InventoryCanvas != null && InventoryActive == false)
        {
            InventoryActive = true;
            Time.timeScale = 0f;
            Debug.Log("Inventory is active");
            InventoryCanvas.SetActive(true);
            ShowFirstItem();
        } else if (inventoryAction.triggered && InventoryCanvas != null && InventoryActive == true)
        {
            InventoryActive = false;
            Time.timeScale = 1f;
            Debug.Log("Inventory is deactivated");
            InventoryCanvas.SetActive(false);
        }
    }

    private void CreateSlots()
    {
        ItemSlot slotPrefab = Resources.Load<ItemSlot>("ItemSlot");
        if (slotPrefab == null)
        {
            Debug.LogError("ItemSlot prefab was not found in a Resources folder.", this);
            itemSlot = new ItemSlot[0];
            return;
        }

        Transform parent = InventoryCanvas.transform;
        GridLayoutGroup grid = InventoryCanvas.GetComponentInChildren<GridLayoutGroup>(true);
        if (grid != null)
        {
            parent = grid.transform;
        }

        itemSlot = new ItemSlot[slotCount];
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i] = Instantiate(slotPrefab, parent);
        }
    }

    public void AddItem(ItemDetails item)
    {
        Debug.Log("Item added to inventory: " + item.ItemName + " Quantity: " + item.Quantity);

        if (itemSlot == null || itemSlot.Length == 0)
        {
            Debug.LogWarning("Inventory has no slots, '" + item.ItemName + "' was lost.");
            return;
        }

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].CanStackWith(item.ItemName))
            {
                itemSlot[i].AddQuantity(item.Quantity);
                return;
            }
        }

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].IsEmpty)
            {
                itemSlot[i].AddItem(item);
                return;
            }
        }

        Debug.LogWarning("Inventory is full, '" + item.ItemName + "' was lost.");
    }

    public void SelectSlot(ItemSlot slot)
    {
        DesselectAllSlot();

        slot.selectedPanel.SetActive(true);
        slot.isItemSelected = true;

        if (detailsPanel != null)
        {
            detailsPanel.Show(slot.ItemDetails, slot.itemQuantity);
        }
    }

    private void ShowFirstItem()
    {
        if (itemSlot != null)
        {
            foreach (ItemSlot slot in itemSlot)
            {
                if (!slot.IsEmpty)
                {
                    SelectSlot(slot);
                    return;
                }
            }
        }

        if (detailsPanel != null)
        {
            detailsPanel.Clear();
        }
    }

    public void DesselectAllSlot()
    {
        foreach (ItemSlot slot in itemSlot)
        {
            slot.isItemSelected = false;
            slot.selectedPanel.SetActive(false);
        }
    }
}
