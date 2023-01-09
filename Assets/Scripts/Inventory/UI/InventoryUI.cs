using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection , Busy }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemsList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;

    int selectedItem = 0;
    InventoryUIState state;

    const int itemsInViewPort = 8;


    List<ItemSlotUI> slotUIList;
    Inventory inventory;
    RectTransform itemsListRect;

    private void Awake() 
    {
        inventory = Inventory.GetInventory();
        itemsListRect = itemsList.GetComponent<RectTransform>();
    }

    private void Start() 
    {
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        // Clear all existing items
        foreach (Transform child in itemsList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();

        foreach (var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemsList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();

    }
    public void HandleUpdate(Action onBack)
    {
        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

            if (prevSelection != selectedItem)
                UpdateItemSelection();

            if (Input.GetKeyDown(KeyCode.Z))
                OpenPartyScreen();
            else if (Input.GetKeyDown(KeyCode.X))
                onBack?.Invoke();
        }
        else if (state == InventoryUIState.PartySelection)
        {
            // Handle Party Selection
            Action onSelected = () => 
            {
                // Use the item on the selected Pokemon
                StartCoroutine(UseItem());

            };

            Action onBackPartyScreen = () =>
            {
                // Close the party screen
                ClosePartyScreen();
            };
            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        var usedItem = inventory.UseUtem(selectedItem, partyScreen.SelectedMember);

        if (usedItem != null)
        {
            yield return DialogManager.Instance.ShowDialogText($"The {usedItem.Name} healed {partyScreen.SelectedMember.Base.Name}");
        }
        else 
        {
            yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
        }

        ClosePartyScreen();
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        var item = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewPort) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewPort/2, 0, selectedItem) * slotUIList[0].Height;
        itemsListRect.localPosition = new Vector2(itemsListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemsInViewPort/2;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + itemsInViewPort/2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }   

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }
}



