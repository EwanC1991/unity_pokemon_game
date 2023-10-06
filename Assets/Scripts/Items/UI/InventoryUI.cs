using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using GDE.GenericSelectionUI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget, Busy }

public class InventoryUI : SelectionUI<TextSlot>
{
    [SerializeField] GameObject itemsList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    Action<ItemBase> onItemUsed;

    int selectedCategory = 0;

    MoveBase moveToLearn;
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

        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemsList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        SetItems(slotUIList.Select(s => s.GetComponent<TextSlot>()).ToList());
        UpdateSelectionUI();

    }

    public override void HandleUpdate()
    {
        int prevCategory = selectedCategory;

        
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedCategory;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedCategory;

        if (selectedCategory > Inventory.ItemCategories.Count - 1)
            selectedCategory = 0;
        else if (selectedCategory < 0)
            selectedCategory = Inventory.ItemCategories.Count - 1;

        if (prevCategory != selectedCategory)
        {
            ResetSelection();
            categoryText.text = Inventory.ItemCategories[selectedCategory];
            UpdateItemList();
        }
        base.HandleUpdate();
    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if (GameController.Instance.State == GameState.Shop)
        {
            onItemUsed?.Invoke(item);
            state = InventoryUIState.ItemSelection;
            yield break;
        }
        if (GameController.Instance.State == GameState.Battle)
        {
            // In Battle
            if (!item.CanUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"You can't use this during a battle!");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            // Outside Battle
            if (!item.CanUseOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"You can't use this outside a battle!");
                state = InventoryUIState.ItemSelection;
                yield break;
            }

        }


        if (selectedCategory == (int)ItemCategory.Pokeballs)
        {
            // StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();

            if (item is TmItem)
                partyScreen.ShowIfTmIsUsable(item as TmItem);
        }
    }

    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"Choose the move you want {pokemon.Base.Name} to forget.", true, false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
    }

    public override void UpdateSelectionUI()
    {
        base.UpdateSelectionUI();

        var slots = inventory.GetSlotsByCategory(selectedCategory);

        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

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

    void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;

        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;

        DialogManager.Instance.CloseDialog();


        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == PokemonBase.MaxNumOfMoves)
        {
            // Don't learn the new move
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {moveToLearn.Name}");
        }
        else
        {
            // Forget the selected move and learn new move

            var selectedMove = pokemon.Moves[moveIndex].Base;
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}!");
            pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }

    public ItemBase SelectedItem => inventory.GetItem(selectedItem, selectedCategory);
    public int SelectedCategory => selectedCategory;
}



