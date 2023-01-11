using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : MonoBehaviour
{
   [SerializeField] List<ItemSlot> slots;
   [SerializeField] List<ItemSlot> pokeballSlots;
   [SerializeField] List<ItemSlot> tmSlots;

   List<List<ItemSlot>> allSlots;

   private void Awake() 
   {
        allSlots = new List<List<ItemSlot>>() { slots, pokeballSlots, tmSlots };
   }

   public event Action OnUpdated;

   public static List<string> ItemCategories { get; set; } = new List<string>()
   {
        "ITEMS", "BALLS", "TM's and HM's"
   };

   public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
   {
        return allSlots[categoryIndex];
   }

   public ItemBase UseUtem(int itemIndex, Pokemon selectedPokemon)
   {
        var item = slots[itemIndex].Item;
        bool itemUsed = item.Use(selectedPokemon);

        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }

        return null;

   }

   public void RemoveItem(ItemBase item)
   {
        var itemSlot = slots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count == 0)
            slots.Remove(itemSlot);

        OnUpdated?.Invoke();
   }

   public static Inventory GetInventory()
   {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
   }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item => item;
    public int Count 
    {
        get => count;
        set => count = value;
    }
}
