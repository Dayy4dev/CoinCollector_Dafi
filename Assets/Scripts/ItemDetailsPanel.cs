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
    public void Show(ItemDetails item)
    {
        if (item == null)
        {
            return;
        }

        itemImage.sprite = item.ItemIcon;
        itemNameText.text = item.ItemName;
        itemQuantityText.text = "Quantity: " + item.Quantity;
        itemDescriptionText.text = item.Description;
    }
}
