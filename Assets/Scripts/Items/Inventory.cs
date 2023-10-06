using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum ItemCategory { Items, Pokeballs, TMs }

public class Inventory : MonoBehaviour, ISavable
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

   public ItemBase GetItem(int itemIndex, int categoryIndex)
   {
          var currentSlots = GetSlotsByCategory(categoryIndex);
          return currentSlots[itemIndex].Item;
   }

   public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon, int selectedCategory)
   {
        var item = GetItem(itemIndex, selectedCategory);
        return UseItem(item, selectedPokemon);

   }
    public ItemBase UseItem(ItemBase item, Pokemon selectedPokemon)
    {
        bool itemUsed = item.Use(selectedPokemon);

        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item);

            return item;
        }

        return null;
    }


    public void AddItem(ItemBase item, int count=1)
   {
          int category = (int) GetCategoryFromItem(item);
          var currentSlots = GetSlotsByCategory(category);

          var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
          if (itemSlot != null)
          {
               itemSlot.Count += count;
          }
          else 
          {
               currentSlots.Add(new ItemSlot()
               {
                    Item = item,
                    Count = count
               });
          }

          OnUpdated?.Invoke();
   }

   public int GetItemCount(ItemBase item)
   {
          int category = (int) GetCategoryFromItem(item);
          var currentSlots = GetSlotsByCategory(category);

          var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);

          if (itemSlot != null)
               return itemSlot.Count;
          else 
               return 0;
   }

   public void RemoveItem(ItemBase item, int countToRemove=1)
   {
          int category = (int)GetCategoryFromItem(item);
          var currentSlots = GetSlotsByCategory(category);

          var itemSlot = currentSlots.First(slot => slot.Item == item);
          itemSlot.Count -= countToRemove;
          if (itemSlot.Count == 0)
               currentSlots.Remove(itemSlot);

          OnUpdated?.Invoke();
   }

   public bool HasItem(ItemBase item)
   {
          int category = (int)GetCategoryFromItem(item);
          var currentSlots = GetSlotsByCategory(category);

          return currentSlots.Exists(slot => slot.Item == item);

   }

   ItemCategory GetCategoryFromItem(ItemBase item)
   {
          if (item is RecoveryItem || item is EvolutionItem)
               return ItemCategory.Items;
          else if (item is PokeballItem)
               return ItemCategory.Pokeballs;
          else
               return ItemCategory.TMs;
   }

   public static Inventory GetInventory()
   {
          return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
   }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
          items = slots.Select(i => i.GetSaveData()).ToList(),
          pokeballs = pokeballSlots.Select(i => i.GetSaveData()).ToList(),
          tms = tmSlots.Select(i => i.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
          var saveData = state as InventorySaveData;

          slots = saveData.items.Select(i => new ItemSlot(i)).ToList();
          pokeballSlots = saveData.pokeballs.Select(i => new ItemSlot(i)).ToList();
          tmSlots = saveData.tms.Select(i => new ItemSlot(i)).ToList();

          allSlots = new List<List<ItemSlot>>() { slots, pokeballSlots, tmSlots };

          OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSlot()
    {

    }

    public ItemSlot(ItemSaveData saveData)
    {
          item = ItemDB.GetObjectByName(saveData.name);
          count = saveData.count;
    }

    public ItemSaveData GetSaveData()
    {
          var saveData = new ItemSaveData()
          {
               name = item.name,
               count = count
          };

          return saveData;
    }

    public ItemBase Item 
    {
        get => item;
        set => item = value;
    }
    public int Count 
    {
        get => count;
        set => count = value;
    }
}

[Serializable]
public class ItemSaveData
{
     public string name;
     public int count;
}

[Serializable]
public class InventorySaveData
{
     public List<ItemSaveData> items;
     public List<ItemSaveData> pokeballs;
     public List<ItemSaveData> tms;
}
