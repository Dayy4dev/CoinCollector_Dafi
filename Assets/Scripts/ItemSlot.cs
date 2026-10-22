using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // Item Data
    public string itemName;
    public int itemQuantity;
    public Sprite itemIcon;
    public bool isFull;

    // The full data of the item in this slot (the details panel needs it)
    public ItemDetails ItemDetails => itemDetails;

    // Maximum number of items that fit in one slot
    [SerializeField] private int maxStackSize = 99;

    // Item Slot
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject selectedPanel;

    // The data of the item currently stored in this slot
    private ItemDetails itemDetails;

    // The inventory that created this slot
    private InventoryManager inventory;

    // Called by InventoryManager right after this slot is created
    public void Setup(InventoryManager owner)
    {
        inventory = owner;
        SetSelected(false);
    }

    // Check if this slot can stack more of the same item
    public bool CanStackWith(string otherItemName)
    {
        return !isFull && itemName == otherItemName;
    }

    public void AddItem(ItemDetails item)
    {
        itemDetails = item;
        itemName = item.ItemName;
        itemQuantity = item.Quantity;
        itemIcon = item.ItemIcon;
        isFull = true;

        quantityText.text = itemQuantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemIcon;
    }

    // Unity calls this when the player clicks on the slot
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFull || inventory == null)
        {
            return; // empty slot: nothing to show
        }

        inventory.SelectSlot(this);
    }

    // Show or hide the selection highlight of this slot
    public void SetSelected(bool selected)
    {
        if (selectedPanel != null)
        {
            selectedPanel.SetActive(selected);
        }
    }
}