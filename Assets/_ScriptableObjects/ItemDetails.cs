using UnityEngine;

[CreateAssetMenu(fileName = "ItemDetails", menuName = "Scriptable Objects/ItemDetails")]
public class ItemDetails : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private int quantity = 1;
    [SerializeField] private Sprite itemIcon;
    [SerializeField, TextArea] private string description;

    public string ItemName => itemName;
    public int Quantity => quantity;
    public Sprite ItemIcon => itemIcon;
    public string Description => description;
}
