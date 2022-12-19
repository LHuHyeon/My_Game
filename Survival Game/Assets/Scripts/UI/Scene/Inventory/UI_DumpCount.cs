using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DumpCount : MonoBehaviour
{
    public UI_Inven_Item invenSlot; // 버릴 아이템을 가진 슬롯
    public Slider sliderValue;
    public InputField countText;
    public int itemCount;

    void Update()
    {
        if (invenSlot != null)
        {
            itemCount = Mathf.Clamp(itemCount, 1, ((int)sliderValue.maxValue));
            countText.text = itemCount.ToString();
        }
    }

    // 버릴 슬롯 아이템 설정
    public void SetSlot(UI_Inven_Item _slot, int _itemSlotCount)
    {
        invenSlot = _slot;
        sliderValue.maxValue = _itemSlotCount;
        itemCount = 1;
    }

    // 슬라이더 값 설정
    public void SliderValueChange()
    {
        itemCount = ((int)sliderValue.value);
    }

    // 확인 버튼
    public void CheckButton()
    {
        // 버릴 아이템에 개수 적용
        ItemPickUp itemPrefab = invenSlot.item.itemPrefab.GetComponent<ItemPickUp>();
        itemPrefab.itemCount = itemCount;

        // 프리팹 생성 및 버릴 위치 설정
        GameObject _item = Managers.Resource.Instantiate($"Item/{invenSlot.item.itemType}/{invenSlot.item.itemName}");
        _item.transform.position = Managers.Game._player.transform.position;

        // 아이템 개수가 최대 개수보다 작을 때
        if (itemCount < invenSlot.itemCount)
            invenSlot.SetCount(-itemCount);
        else
            invenSlot.ClearSlot();  // 슬롯 초기화

        CancelButton();             // 종료
    }

    // 취소 버튼
    public void CancelButton()
    {
        invenSlot = null;
        sliderValue.value = 1;
        sliderValue.maxValue = 1;
        countText.text = "";
        gameObject.SetActive(false);
    }

    // 버튼 클릭 시 아이템 개수 +1
    public void PlusButton()
    {
        sliderValue.value = ++itemCount;
    }

    // 버튼 클릭 시 아이템 개수 -1
    public void MinusButton()
    {
        sliderValue.value = --itemCount;
    }
}
