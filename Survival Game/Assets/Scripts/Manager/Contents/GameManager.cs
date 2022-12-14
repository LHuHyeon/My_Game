using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컨텐츠에서 사용될 매니저 (플레이어, 몬스터 등..)
public class GameManager
{
    public GameObject _player;              // 플레이어 관리 오브젝트
    public PlayerStat playerStat;           // 플레이어 스탯
    public UI_PlayerInfo playerInfo;        // 플레이어 UI 관리 (HP, 슬롯 등..)
    public ItemEffectDatabase itemDatabase; // 아이템 스탯 저장소
    public UI_Inven baseInventory;          // 인벤토리 관리

    public bool isInventory = false;    // 인벤토리 비활성화/활성화 여부
    public bool isDiveRoll = false;     // 현재 구르기 중인가?
    public bool isShop = false;         // 상점 이용 중인가?
    public bool isStat = false;         // 스탯 비활성화/활성화 여부
    public bool isUIMode = false;       // UI 사용 중인지 확인
    
    HashSet<GameObject> _monsters = new HashSet<GameObject>();

    public Action<int> OnSpawnEvent;

    // 캐릭터 소환
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch(type){
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
            default:
                Debug.Log("GameManager : Null Type");
                break;
        }

        return go;
    }

    // 타입 확인
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;
    }

    // 캐릭터 삭제
    public void Despawn(GameObject go)
    {
        switch(GetWorldObjectType(go)){
            case Define.WorldObject.Monster:
                {
                    if (_monsters.Contains(go)){ // 존재 여부 확인
                        _monsters.Remove(go);
                        if (OnSpawnEvent != null)
                            OnSpawnEvent.Invoke(-1);
                    }
                }
                break;
            case Define.WorldObject.Player:
                {
                    if (_player == go)
                        _player = null;
                }
                break;
        }

        Managers.Resource.Destroy(go);
    }

    // 공통적인 활성화/비활성화
    public void IsActive(bool has, UI_Scene _scene)
    {
        if (_scene.baseObject != null)
            _scene.baseObject.SetActive(has);

        // SortingOrder 관리
        if (has)
            Managers.UI.OnUI(_scene);
        else
            Managers.UI.CloseUI(_scene);
    }
}
