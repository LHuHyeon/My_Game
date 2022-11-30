using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager
{
    // 현재 들고있는 무기를 여기서 확인
    public GameObject weaponActive;        // 현재 무기 오브젝트 (활성화/비활성화 용도)
    public Item currentWeapon;             // 현재 무기 아이템
    public GameObject attackCollistion;    // 공격 시 충돌처리 해줄 객체

    // 무기 장착
    public void EquipWeapon(Item _item)
    {
        int count = Managers.Game._player.GetComponent<PlayerController>().weaponList.Count;
        currentWeapon = _item;
        
        // 무기 개수가 많지 않으므로 루프문 사용
        for(int i=0; i<count; i++)
        {
            if (_item.itemName == Managers.Game._player.GetComponent<PlayerController>().weaponList[i].name)
            {
                weaponActive = Managers.Game._player.GetComponent<PlayerController>().weaponList[i];
                return;
            }
        }
    }
}
