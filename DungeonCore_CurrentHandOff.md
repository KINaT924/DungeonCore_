# DungeonCore 최신 개발 인수인계 — 스킬 입력·개별 쿨타임 스크립트 작성 전

최종 갱신: 2026-09-05

## 1. 정확한 중단 지점

**PlayerSkillController 전체 코드와 Inspector 연결법을 안내했지만, 사용자가 아직 작성·적용하지 않은 상태다. 다음 대화는 이 스크립트 작성 단계부터 재개한다.**

- 사용자 최종 요청: “아직 입력 및 개별 쿨타임을 적용하지 못했어 이 스크립트 작성단계 그대로 인수인계 md파일에 최신화 해줄래?”
- 갱신 시 실제 확인: `Assets/02.Scripts/Player/PlayerSkillController.cs` 파일 없음.
- 스킬 입력, 개별 쿨타임, 시전 상태, 피해 판정, 취소 통합을 완료로 취급하지 않는다.
- 이 문서의 코드는 **미적용 학습안**이며 실제 구현 파일이 아니다.
- 새로운 대화에서는 실제 파일과 이 문서, `8주간의 계획서.md`, `현재 주간목표서.md`를 대조한다. 이전 계획 문서에는 체력 UI 부분 완료 및 스킬 1개 범위가 남아 있을 수 있다. 최신 사용자 결정은 아래 기록을 따른다.

## 2. 협업 및 학습 방식

- 취업용 Unity 포트폴리오 프로젝트다.
- 사용자가 직접 이해하고 작성하는 단계별 학습을 원한다.
- 명확한 파일 생성·수정·적용 요청이 없으면 실제 코드·씬·프리팹을 자동 수정하지 않는다. 문서 갱신 요청을 구현 승인으로 확대하지 않는다.
- 코드를 제안할 때 전체 코드, 변경 지점, 클래스·필드·프로퍼티·메서드의 역할과 이유를 설명하는 주석을 제공한다.
- Hierarchy 선택 대상, Inspector 컴포넌트와 필드, 드래그할 대상, Animator·Animation Event·Collider·Layer·Prefab 설정을 구체적으로 안내한다.
- 한 단계의 테스트 결과를 확인한 뒤 다음 단계로 넘어간다. 안내와 구현 완료를 구분한다.
- 기능별 학습 효과, 포트폴리오에서 설명 가능한 장점과 선택 이유, 실제 구조의 한계를 함께 설명한다.
- 사용자 네이밍을 유지한다. `cameraTranform`, `moveDir`, `characterCtr`, `dodgeCtr`, `dodgeDur`, `dodgeCoolTime`, `coolTimer`, `BindInputEvents()`, `UnBindInputEvents()` 등을 임의 정리하지 않는다.
- 일부 스크립트의 한글은 UTF-8 읽기에서 깨져 보였다. 원본 인코딩을 확인하지 않고 주석·파일을 일괄 재저장하지 않는다.
- 실제 Unity 실행을 하지 않았다면 실행 검증했다고 말하지 않는다.

## 3. 프로젝트 방향과 완료 범위

- Unity 6.3 기반 3D TPS 액션, 8주 싱글 MVP 우선.
- 전사와 거너, 몬스터·에너지·코어·성장 카드·탈출·보스전으로 이어지는 게임.
- 1주차 전사 기반 완료: 입력, 카메라, 이동, 기본 공격, 회피와 무적, Health/IDamageable, 사망과 행동 차단, 애니메이션 및 통합 테스트.
- 2주차 진행 중: Enemy_Dummy 피해·사망·중복 피해 방지와 Health 이벤트 구조 완료.
- **플레이어 체력 UI는 사용자 정상 동작 확인 완료.** 시작 표시, 피해 갱신, 사망 흐름이 정상이라고 보고했다.
- 사용자가 체력 UI 크기·위치를 보기 좋게 조정했다. 기존 배치를 보존한다.
- 체력 UI 초기 0/100 문제는 `Health.Awake()`보다 `PlayerHealthUI.OnEnable()`이 먼저 읽을 수 있는 초기화 순서 문제로 분석했다. `PlayerHealthUI.Start()`에서 `InitializeHealthUI()`를 다시 호출하는 수정이 실제 파일에 있으며 사용자 정상 동작 확인.
- 현재 스킬은 원래 1개에서 **2개로 범위 확대**. 기본 공격 모션·범위 변경은 보류하고 두 스킬부터 작업한다. 기존 기본 공격 구현을 삭제하거나 비활성화하라는 뜻은 아니다.

## 4. 미술 방향 — 확정된 사용자 결정

- 전사·거너는 현재 KayKit SD 캐릭터를 유지한다. 모델 교체를 다시 추진하지 않는다.
- 사용자가 만든 `Assets/03.Art/Prefabs/Map/Map Group.prefab`은 POLYGON city pack 기반 도시 맵이다. 도로·공원·건물·가로등·주차장·외곽 벽 등이 있으며 사용자가 전체와 인게임 화면을 제공했다.
- 조명을 어둡게 조정해 분위기를 살릴 계획이며 몬스터도 과도하게 크게 만들지 않을 예정이다.
- SD의 귀여움을 최우선으로 고른 것은 아니지만, 전사·거너 에셋 확보와 전체 제작 방향을 고려해 **현재 캐릭터 유지로 최종 결정**했다.
- 기본 공격과 스킬은 검 모델 길이에만 한정하지 않고 확장된 범위를 의도한다.
- **이펙트는 추후 추가**한다. 지금 검기 이펙트 제작이나 최종 시각 범위 확정을 선행 조건으로 삼지 않는다. 이후 이펙트와 실제 판정을 맞춘다.

## 5. 스킬 명칭과 모션

| 구분 | 사용자 의도 Animator 상태 이름 | 클립 | 입력 |
|---|---|---|---|
| Skill1 | Skill1_SlashAttack | Attack_Slash.anim | Skill1 / 키보드 1 |
| Skill2 | Skill2_SpinAttack | Attack_Spin.anim | Skill2 / 키보드 2 |

- 처음 제안한 `SpinSkill` 상태 이름은 사용자가 `Skill2_SpinAttack`으로 바꿨다. 최신 이름 사용.
- `Attack_Slash.anim`: 약 0.97초, 30 FPS, Loop 꺼짐.
- `Attack_Spin.anim`: 1.2초, 30 FPS, Loop 꺼짐.
- 이전 검사에서 두 클립 Animation Event는 비어 있었다. 연결 단계에서 재확인.
- **갱신 시 실제 Animator 불일치 발견:** Slash 클립을 참조하는 상태 이름이 `Skill2_SlashAttack`으로 저장되어 있다. 사용자 의도는 `Skill1_SlashAttack`이므로 다음 Animator 연결 단계에서 확인·안내한다. 자동 수정하지 않았다.
- 저장된 Animator에는 `Skill1`, `Skill2` 파라미터와 `Skill2_SpinAttack` 상태가 존재한다. 이것만으로 키 입력 시전 구현 완료라고 판단하지 않는다.

## 6. 이전 Mixamo 조사와 주의사항

- `X Bot@Sword And Shield Slash.fbx` 원본 위치: `Assets/99.Assets/kaykit_Anim/Animations/`.
- 원본은 Humanoid / Create From This Model. 사용자는 X Bot은 정상, 전사만 뒤틀린다고 확인했다.
- Knight의 실제 chest 뼈가 Avatar Chest에 매핑되지 않은 점을 발견해 보완을 안내했지만, 사용자는 모션 개선이 없다고 보고했다. **Chest가 확정 원인 또는 해결책이었다고 기록하지 않는다.**
- 이후 Inspector 오류 3개가 발생했으나 사용자 최종 보고로 사라졌다. Editor.log의 AnimatorInspector/GameObjectInspector/TransformInspector.OnEnable 내부 참조 오류였으며 스킬 코드 오류가 아니었다.
- FBX에서 생성되는 Avatar와 독립 `Assets/03.Art/Animations/KnightAvatar.asset`은 별개다. 이전 검사에서 Player는 독립 Avatar를 참조했다. 불필요한 Avatar 재변경은 하지 않는다.
- 사용자는 새 `Attack_Slash`, `Attack_Spin`으로 방향을 바꿨다. Mixamo 문제 재조사를 현재 작업의 선행 조건으로 만들지 않는다.

## 7. 현재 재사용할 코드와 범위 구성 상태

- `PlayerInputReader`에는 `Skill1Pressed`, `Skill2Pressed`, `OnSkill1()`, `OnSkill2()`가 이미 존재한다.
- Player Input의 Unity Events 연결과 숫자 키 바인딩은 다음 입력 테스트 시 실제 확인한다.
- `Health.IsDead`, `PlayerDodgeController.IsDodging`을 읽어 스킬 사용을 차단할 수 있다.
- `AttackArea`는 `GetComponent<Collider>()`, `BeginAttack(damage)`, `EndAttack()`, `HashSet<IDamageable>`로 범위와 공격당 중복 피해를 관리한다.
- 대상은 현재 `target.GetComponent<IDamageable>()`로 찾는다. 적 Collider와 Health가 서로 다른 오브젝트에 있으면 후속 검토 필요.
- 앞서 Player 아래 `Skill1Area`(Box), `Skill2Area`(Sphere), 각각 Rigidbody/AttackArea 구성법을 안내했지만 **사용자가 완료했다고 확인하지 않았다.** 생성 완료를 가정하지 않는다.
- 범위 초기 제안은 Skill1 Box Center (0,1,1.5), Size (3,2,3), Skill2 Sphere Center (0,1,0), Radius 2였다. 확정 수치가 아니다. 구형 판정의 높이 범위와 회전 검기의 순차 판정은 추후 검토.

## 8. 바로 재개할 학습안 — 입력과 개별 쿨타임

작성할 파일: `Assets/02.Scripts/Player/PlayerSkillController.cs`

이번 단계는 입력 승인 로그와 독립 타이머만 확인한다. 애니메이션·피해·시전 상태는 아직 연결하지 않는다. 따라서 1과 2를 연속으로 누르면 둘 다 승인되는 것이 이 단계에서는 정상이다.

아래는 직전 안내한 구현 내용이다. 작성 시작 전에 실제 파일 유무를 다시 확인하고, 사용자가 직접 작성할 수 있도록 설명한다.

```csharp
using UnityEngine;

/// <summary>
/// Skill1, Skill2 입력과 개별 쿨타임을 관리합니다.
/// 현재는 사용 조건을 검사하고 로그로 확인하는 학습 단계입니다.
/// 애니메이션, 판정, 스킬 취소는 이후 연결합니다.
/// </summary>
public class PlayerSkillController : MonoBehaviour
{
    [Header("레퍼런스")]
    // 스킬 입력 이벤트를 제공하는 컴포넌트입니다.
    [SerializeField] PlayerInputReader inputReader;
    // 사망 상태에서 사용을 차단합니다.
    [SerializeField] Health health;
    // 회피 중 사용을 차단합니다.
    [SerializeField] PlayerDodgeController dodgeCtr;

    [Header("스킬 쿨타임")]
    // 초기 테스트 설정값이며 최종 밸런스가 아닙니다.
    [SerializeField, Min(0f)] float skill1CoolTime = 5f;
    [SerializeField, Min(0f)] float skill2CoolTime = 8f;

    // 설정값과 별도로 현재 남은 시간을 저장합니다.
    float skill1CoolTimer;
    float skill2CoolTimer;

    // 이후 UI가 읽을 수 있도록 외부 변경은 허용하지 않습니다.
    public float Skill1RemainingCooldown
    {
        get { return skill1CoolTimer; }
    }
    public float Skill2RemainingCooldown
    {
        get { return skill2CoolTimer; }
    }

    // Inspector 참조가 없으면 같은 오브젝트에서 찾습니다.
    private void Awake()
    {
        if (inputReader == null)
            inputReader = GetComponent<PlayerInputReader>();
        if (health == null)
            health = GetComponent<Health>();
        if (dodgeCtr == null)
            dodgeCtr = GetComponent<PlayerDodgeController>();

        if (inputReader == null || health == null || dodgeCtr == null)
            Debug.LogError("PlayerSkillController의 레퍼런스를 확인하세요.", this);
    }

    // 활성화 시 입력 알림을 등록합니다.
    private void OnEnable()
    {
        BindInputEvents();
    }

    // 비활성화 시 등록한 입력 알림을 해제합니다.
    private void OnDisable()
    {
        UnBindInputEvents();
    }

    // 두 타이머를 프레임 소요 시간만큼 감소시킵니다.
    private void Update()
    {
        UpdateCoolTimers();
    }

    // 입력별 처리 메서드를 각각 구독합니다.
    void BindInputEvents()
    {
        if (inputReader == null)
            return;
        inputReader.Skill1Pressed += HandleSkill1Pressed;
        inputReader.Skill2Pressed += HandleSkill2Pressed;
    }

    // 동일한 이벤트와 메서드 조합으로 구독을 해제합니다.
    void UnBindInputEvents()
    {
        if (inputReader == null)
            return;
        inputReader.Skill1Pressed -= HandleSkill1Pressed;
        inputReader.Skill2Pressed -= HandleSkill2Pressed;
    }

    // 공통 사용 조건입니다. 개별 쿨타임은 각 핸들러에서 검사합니다.
    bool CanUseSkill()
    {
        if (health == null || dodgeCtr == null)
            return false;
        if (health.IsDead)
            return false;
        if (dodgeCtr.IsDodging)
            return false;
        return true;
    }

    // Skill1 조건을 통과했을 때만 사용을 승인합니다.
    void HandleSkill1Pressed()
    {
        if (!CanUseSkill())
            return;
        if (skill1CoolTimer > 0f)
        {
            Debug.Log($"Skill1 쿨타임: {skill1CoolTimer:F1}초 남음", this);
            return;
        }
        UseSkill1();
    }

    // Skill2는 Skill1과 별개의 타이머를 검사합니다.
    void HandleSkill2Pressed()
    {
        if (!CanUseSkill())
            return;
        if (skill2CoolTimer > 0f)
        {
            Debug.Log($"Skill2 쿨타임: {skill2CoolTimer:F1}초 남음", this);
            return;
        }
        UseSkill2();
    }

    // 승인 시 Skill1 타이머만 시작합니다.
    void UseSkill1()
    {
        skill1CoolTimer = skill1CoolTime;
        Debug.Log("Skill1_SlashAttack 사용 승인", this);
    }

    // 승인 시 Skill2 타이머만 시작합니다.
    void UseSkill2()
    {
        skill2CoolTimer = skill2CoolTime;
        Debug.Log("Skill2_SpinAttack 사용 승인", this);
    }

    // 음수가 되지 않도록 0으로 제한합니다.
    void UpdateCoolTimers()
    {
        skill1CoolTimer = Mathf.Max(0f, skill1CoolTimer - Time.deltaTime);
        skill2CoolTimer = Mathf.Max(0f, skill2CoolTimer - Time.deltaTime);
    }
}
```

### Inspector 연결

1. Play 종료 후 PlayerInputReader, Health, PlayerDodgeController가 붙은 Player에 PlayerSkillController 추가.
2. Input Reader → Player의 PlayerInputReader.
3. Health → Player의 Health.
4. Dodge Ctr → Player의 PlayerDodgeController.
5. Skill1 Cool Time → 5, Skill2 Cool Time → 8 (테스트 값).
6. Player Input → Events → 해당 Action Map에서 Skill1 → PlayerInputReader.OnSkill1, Skill2 → PlayerInputReader.OnSkill2 확인. 기존 항목을 중복 추가하지 않는다.

### 테스트와 완료 기준 — 모두 아직 미확인

- [ ] 컴파일 오류 없이 컴포넌트 추가 및 참조 연결.
- [ ] Game 창에서 1 입력 → Skill1_SlashAttack 사용 승인 로그.
- [ ] 바로 1 재입력 → 남은 쿨타임 출력.
- [ ] Skill1 쿨타임 중 2 입력 → Skill2_SpinAttack 승인 로그.
- [ ] 바로 2 재입력 → Skill2 남은 쿨타임 출력.
- [ ] 각각 5초/8초 경과 후 해당 스킬 재승인.
- [ ] 회피 중·사망 후 사용 승인 없음.

학습 효과: 입력 이벤트와 스킬 책임 분리, 설정값과 실행 상태 구분, 개별 타이머, 읽기 전용 프로퍼티를 통한 추후 UI 연결, 활성화/비활성화 구독 관리.

## 9. 입력 테스트 이후 진행할 작업

아래는 후속 계획이며 구현 완료가 아니다.

1. 현재 사용 중인 스킬 상태와 외부 읽기 구조 추가.
2. 스킬 중 다른 스킬과 기본 공격 차단, 이동 규칙 연결.
3. PlayerAnimationController와 Skill1/Skill2 Trigger 연결.
4. Skill1_SlashAttack / Skill2_SpinAttack 상태 전환 확인. Slash 상태 명칭 불일치 점검.
5. Animation Event에서 공격 범위 활성·비활성·스킬 종료 연결.
6. 회피·사망 시 취소와 판정 즉시 종료. 취소해도 시작한 쿨타임 유지하는 방향을 제안했으며 연결 단계에서 확인.
7. Enemy_Dummy 피해와 공격당 중복 피해 방지 확인. Spin은 우선 적마다 사용 1회당 피해 1회인 설계안.
8. 실제 플레이 결과에 맞춰 범위·피해량·쿨타임 조정.
9. 이펙트는 추후 추가하고 최종 시각 범위와 판정 동기화.

기본 공격의 새 모션·범위 제작은 보류한다. 다만 스킬과의 상태 충돌 차단은 필요한 통합 범위다.

## 10. 다음 대화 시작 문장

> 인수인계 문서와 실제 파일을 확인해줘. PlayerSkillController 입력·개별 쿨타임 코드는 아직 작성하지 않았어. 그 작성 단계부터 전체 코드와 주석, Inspector 연결, 테스트와 학습 효과를 설명해줘. 현재 SD 전사·거너 방향을 유지하고, 기본 공격 변경은 보류한 채 Skill1_SlashAttack(Attack_Slash), Skill2_SpinAttack(Attack_Spin) 두 스킬부터 진행할 거야. 이펙트는 나중에 추가해. 명시적으로 요청하지 않으면 코드나 씬을 직접 수정하지 마.
