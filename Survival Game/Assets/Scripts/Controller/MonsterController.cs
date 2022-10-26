using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    [SerializeField]
    float _scanRange = 10f;     // 주변 탐색 거리

    [SerializeField]
    float _attackRange = 2f;    // 공격 거리

    [SerializeField]
    GameObject _lockTarget;

    float distance;     // 타겟과 나의 거리
    bool stopMoving = false;    // 움직임 멈추기

    Vector3 _destPos;         // 도착 좌표
    
    Stat _stat;
    Animator anim;
    NavMeshAgent nav;

    public override Define.State State
    {
        get { return _state; }
        set {
            _state = value;

            switch (_state){
                case Define.State.Moving:
                    // anim.CrossFade("Walk", 0.1f);    // CrossFade는 애니메이션의 부드러움, 반복도 등 설정이 가능하다.
                    anim.SetTrigger("OnWalk");
                    break;
                case Define.State.Idle:
                    // anim.CrossFade("Idle", 0.1f);
                    anim.SetTrigger("OnIdle");
                    break;
                case Define.State.Skill:
                    // anim.CrossFade("Attack", 0.1f, -1, 0);
                    anim.SetTrigger("OnAttack");
                    break;
                case Define.State.Die:
                    break;
            }
        }
    }

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;

        _stat = GetComponent<Stat>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // 주변 플레이어 탐색
    protected override void UpdateIdle()
    {
        GameObject player = Managers.Game._player;

        if (player.isValid()){
            distance = TargetDistance(player);
            if (distance <= _scanRange){
                _lockTarget = player;
                State = Define.State.Moving;
                return;
            }
        }
    }

    protected override void UpdateMoving()
    {
        if (stopMoving)
        {
            nav.SetDestination(transform.position);
            return;
        }

        // 타겟(플레이어)이 존재하면 두 사이 거리가 _attackRange보다 작거나같을때 멈추고 스킬 시전(공격)
        if (_lockTarget != null){
            distance = TargetDistance(_lockTarget);
            if (distance <= _attackRange){
                nav.SetDestination(transform.position); // 타겟을 나로 지정하면 멈춘다.
                State = Define.State.Skill;
                return;
            }
        }

        // 도착 위치 벡터에서 플레이어 위치 벡터를 뺀다.
        Vector3 dir = _destPos - transform.position;

        // Vector3.magnitude = 벡터값의 길이
        if (dir.magnitude < 0.1f)
            State = Define.State.Idle;
        else{
            // nav.speed = _stat.MoveSpeed;
            nav.speed = 2f;
            nav.SetDestination(_destPos);   // 타겟에 접근
            
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        }
    }

    // 플레이어 공격
    protected override void UpdateSkill()
    {
        // 스킬 사용 중에 타겟 바라보기
        if (_lockTarget != null){
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    // 애니메이션 event
    void OnHitEvent()
    {
        if (_lockTarget != null){
            distance = TargetDistance(_lockTarget);

            if (distance <= _attackRange)
                State = Define.State.Skill;
            else
                State = Define.State.Moving;

            Stat targetStat = _lockTarget.GetComponent<Stat>();

            targetStat.OnAttacked(_stat);

            if (targetStat.Hp > 0){
                distance = TargetDistance(_lockTarget);
                if (distance <= _attackRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
                State = Define.State.Idle;
        }
        else
            State = Define.State.Idle;
    }

    // 타겟과 나의 거리
    float TargetDistance(GameObject target)
    {
        _destPos = target.transform.position;
        _destPos.y = 0;
        return (_destPos - transform.position).magnitude;
    }

    public void TakeDamage(Stat attacker)
    {
        anim.SetTrigger("OnHit");
        _stat.OnAttacked(attacker.GetComponent<Stat>());
        StartCoroutine(DelayHit());
    }

    IEnumerator DelayHit()
    {
        stopMoving = true;
        yield return new WaitForSeconds(0.7f);
        stopMoving = false;
    }
}