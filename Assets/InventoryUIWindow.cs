using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIWindow : MonoBehaviour {
    private bool isWindowActive = false;

    public int startingPosX;
    public int startingPosY;

    public int slotCountPerPage;
    public int slotCountLength;
    public int slotPadding;


    public GameObject itemSlotPrefab = null;

    private ToggleGroup itemSlotToggleGroup = null;
    private int currentPosX;
    private int currentPosY;

    private void Awake() {

        if (!itemSlotPrefab) {
            itemSlotPrefab = Resources.Load("UI/Prefabs/InventorySlotPrefab") as GameObject;
        }

        itemSlotToggleGroup = GetComponent<ToggleGroup>();
    }

    void Start() {
        InputHandler.current.OnPress_i += Input_HandleOnPress_i;
        gameObject.SetActive(isWindowActive);

        CreateInventorySlotsInWindow();
    }

    private void Input_HandleOnPress_i() {
        isWindowActive = !isWindowActive;
        gameObject.SetActive(isWindowActive);
    }

    public void CloseButtonPress(bool b) {
        isWindowActive = b;
        gameObject.SetActive(isWindowActive);
    }

    private void CreateInventorySlotsInWindow() {
        currentPosX = startingPosX;
        currentPosY = startingPosY;
        for (int currentCount = 0, i = 0; i < slotCountPerPage; i++) {
            GameObject itemSlot = Instantiate<GameObject>(itemSlotPrefab, transform);
            RectTransform itemRect = itemSlot.GetComponent<RectTransform>();
            itemSlot.name = "Slot" + i;
            itemSlot.GetComponent<Toggle>().group = itemSlotToggleGroup;
            itemRect.localPosition = new Vector3(currentPosX, currentPosY);
            currentPosX += (int)itemRect.rect.width + slotPadding;
            currentCount++;

            if (currentCount % slotCountLength == 0) {
                currentPosY -= (int)itemRect.rect.width + slotPadding;
                currentPosX = startingPosX;
            }
        }
    }
}
