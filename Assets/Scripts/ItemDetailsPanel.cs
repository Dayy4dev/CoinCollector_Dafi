using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Sits on the InventoryDetails object (right side of the inventory)
// and draws the item that the player selected
public class ItemDetailsPanel : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemQuantityText;
    [SerializeField] private TMP_Text itemDescriptionText;

    // Called by InventoryManager whenever the player selects a slot
    public void Show(ItemDetails item, int quantity)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        itemImage.sprite = item.ItemIcon;
        itemImage.enabled = true;
        itemNameText.text = item.ItemName;
        itemQuantityText.text = "Quantity: " + quantity;
        itemDescriptionText.text = item.Description;
    }
    
    // Empty the panel, used when the selected slot has no item
    private void Clear()
    {
        itemImage.sprite = null;
        itemImage.enabled = false;
        itemNameText.text = "";
        itemQuantityText.text = "";
        itemDescriptionText.text = "";
    }
}
