using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 스킬 입력과 쿨타임 등 스킬의 상세 내용을 담당하는 컴포넌트입니다
    /// </summary>

    [Header("레퍼런스")]
    [SerializeField] PlayerInputReader inputReader;                 // 입력 이벤트를 위한 참조
    [SerializeField] Health health;                                 // 체력 컴포넌트 ( 이 스크립트에선 사망 여부를 확인하기 위해 사용 )
    [SerializeField] PlayerDodgeController dodgeCtr;                // 회피 컴포넌트 ( 이 스크립트에선 회피 상태를 확인하기 위해 사용 )
    [SerializeField] PlayerCombat playerCombat;                     // 공격 컴포넌트 ( 스킬 사용 시 공격 상태를 확인하기 위해 사용 )
    [SerializeField] PlayerAnimationController animCtr;             // 애니메이션 컨트롤러 ( 스킬 사용 시 애니메이션을 재생하기 위해 사용 )

    [Header("스킬 판정 오브젝트")]
    [SerializeField] AttackArea skill1Area;
    [SerializeField] AttackArea skill2Area;

    [Header("스킬 설정")]
    [SerializeField] float skill1Damage = 30f;                      // 스킬1의 피해량
    [SerializeField] float skill2Damage = 50f;                      // 스킬2의 피해량
    [SerializeField] float skill1CoolTime = 5f;                     // 스킬1의 쿨타임
    [SerializeField] float skill2CoolTime = 8f;                     // 스킬2의 쿨타임
    float skill1Timer;                                              // 스킬1의 남아있는 대기시간
    float skill2Timer;                                              // 스킬2의 남아있는 대기시간

    int curSkill;       // 현재 시전중인 스킬 ( 0 : 없음, 1 : 스킬1, 2 : 스킬2 )
    bool hasOpenedArea;    // 스킬 판정 오브젝트가 활성화 되었는지 확인하는 변수

    // 외부에 사용 가능한 프로퍼티
    public bool IsUsingSkill { get { return curSkill != 0; } }
    public float Skill1RemainTime { get { return skill1Timer; } }
    public float Skill2RemainTime { get { return skill2Timer; } }

    // 컴포넌트가 제대로 연결되지 않았다면 오브젝트를 직접 찾아 연결하고 실패한다면 오류를 출력
    private void Awake()
    {
        if(inputReader ==null)
            inputReader = GetComponent<PlayerInputReader>();
        if (health == null)
            health = GetComponent<Health>();
        if(dodgeCtr == null)
            dodgeCtr = GetComponent<PlayerDodgeController>();
        if(playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();    
        if (animCtr == null)
            animCtr = GetComponent<PlayerAnimationController>();

        if (inputReader == null || health == null || dodgeCtr == null || playerCombat == null || animCtr == null
                                                    || skill1Area == null || skill2Area == null || skill1Area == skill2Area)
        {
            Debug.LogError("PlayerSkillController 스크립트에 필요한 컴포넌트가 할당되지 않았습니다.");

            enabled = false;    // 스크립트 비활성화
        }
    }

    private void OnEnable()
    {
        BindInputEvents();

        if( health != null)
            health.OnDied += CancelSkill;
    }

    private void OnDisable()
    {
        UnBindInputEvents();

        if (health != null)
            health.OnDied -= CancelSkill;

        CancelSkill();
    }

    private void Update()
    {
        UpdateCoolTimers();

        if(IsUsingSkill && (health.IsDead || dodgeCtr.IsDodging))
        {
            CancelSkill();
        }
    }

    // 각 입력에 대한 이벤트에 대응하는 메소드를 등록
    void BindInputEvents()
    {
        if (inputReader == null)
            return;

        inputReader.Skill1Pressed += HandleSkill1Pressed;
        inputReader.Skill2Pressed += HandleSkill2Pressed;
    }

    // 등록했었던 이벤트를 메소드와의 연결에서 해제
    void UnBindInputEvents()
    {
        if (inputReader == null)
            return;

        inputReader.Skill1Pressed -= HandleSkill1Pressed;
        inputReader.Skill2Pressed -= HandleSkill2Pressed;
    }

    // 두 스킬에 공통으로 적용되는 사용 조건을 검사하는 메소드
    bool CanUseSkill()
    {
        if (!isActiveAndEnabled)
            return false;
        if (health.IsDead || dodgeCtr.IsDodging || IsUsingSkill)
            return false;

        // 적용되는게 없다면 true를 반환하여 스킬 사용 가능
        return true;
    }

    // 스킬 1의 이벤트가 발생했을 때 호출되는 메소드
    void HandleSkill1Pressed()
    {
        if (!CanUseSkill())
            return;
        if (skill1Timer > 0f)
        {
            Debug.Log("스킬1은 아직 쿨타임 중입니다. 남은 시간 : " + skill1Timer);
            return;
        }
        // 스킬1 사용 로직
        UseSkill1();
    }

    // 스킬 2의 이벤트가 발생했을 때 호출되는 메소드
    void HandleSkill2Pressed()
    {
        if (!CanUseSkill())
            return;
        if (skill2Timer > 0f)
        {
            Debug.Log("스킬2은 아직 쿨타임 중입니다. 남은 시간 : " + skill2Timer);
            return;
        }
        // 스킬2 사용 로직
        UseSkill2();
    }

    // 사용조건을 통과해 스킬이 시작됬을 때 호출되는 메소드
    void UseSkill1()
    {
        CloseSkillAreas();
        hasOpenedArea = false;

        curSkill = 1;
        playerCombat.CancelAttack();        // 공격중이라면 공격을 취소
        skill1Timer = skill1CoolTime;       // 쿨타임 초기화
        animCtr.PlayerSkill1Animation();    // 스킬1 애니메이션 재생

        Debug.Log("스킬1 사용");
    }
    void UseSkill2()
    {
        CloseSkillAreas();
        hasOpenedArea = false;

        curSkill = 2;
        playerCombat.CancelAttack();        // 공격중이라면 공격을 취소
        skill2Timer = skill2CoolTime;       // 쿨타임 초기화d
        animCtr.PlayerSkill2Animation();    // 스킬2 애니메이션 재생
        Debug.Log("스킬2 사용");
    }


    // 스킬 타격 시작 이벤트에서 호출
    public void EnabelSkillArea(int skillNum)
    {
        if (!isActiveAndEnabled || curSkill == 0)
            return;
        if (curSkill != skillNum || hasOpenedArea)
            return;
        if (health.IsDead || dodgeCtr.IsDodging)
            return;

        hasOpenedArea = true;

        if(skillNum == 1)
            skill1Area.BeginAttack(skill1Damage);
        else if (skillNum == 2)
            skill2Area.BeginAttack(skill2Damage);
    }

    // 스킬 타격 종료 이벤트에서 호출
    public void DisableSkillArea(int skillNum)
    {
        if (curSkill == 0 || curSkill != skillNum)
            return;

        CloseSkillAreas();
    }

    // 스킬 모션 끝에서 이벤트로 호출
    void EndSkill(int skillNum)
    {
        if (curSkill == 0 || curSkill != skillNum)
            return;

        CancelSkill();
    }

    // 시전중단할 때 호출되는 메소드
    public void CancelSkill()
    {
        curSkill = 0;
        CloseSkillAreas();
    }

    // 스킬 판정을 닫아 Area를 비활성화시키는 메소드
    void CloseSkillAreas()
    {
        if (skill1Area != null)
            skill1Area.EndAttack();

        if (skill2Area != null)
            skill2Area.EndAttack();
    }

    // 각 타이머의 남은 시간을 매 프레임마다 감소시키는 메소드
    void UpdateCoolTimers()
    {
        skill1Timer = Mathf.Max(0f, skill1Timer - Time.deltaTime);
        skill2Timer = Mathf.Max(0f, skill2Timer - Time.deltaTime);
    }
}
