using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailsPanel : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemQuantityText;
    [SerializeField] private TMP_Text itemDescriptionText;

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
        itemQuantityText.text = "Jumlah: " + quantity;
        itemDescriptionText.text = item.Description;
    }

    public void Clear()
    {
        itemImage.sprite = null;
        itemImage.enabled = false;
        itemNameText.text = "";
        itemQuantityText.text = "";
        itemDescriptionText.text = "";
    }
}
