using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 Animation을 제어하는 컴포넌트
    /// 이동 , 회피, 공격 등 플레이어의 상태에 따라 Animator의 파라미터를 변경하여 애니메이션을 제어합니다
    /// </summary>

    [Header("레퍼런스")]
    [SerializeField] Animator animator;                             // 플레이어의 Animator 컴포넌트에 대한 참조
    [SerializeField] PlayerInputReader inputReader;                 // 입력 이벤트를 위한 참조
    [SerializeField] PlayerDodgeController dodgeCtr;                // 플레이어의 회피 상태를 확인하기 위한 참조
    [SerializeField] PlayerCombat playerCombat;                     // 플레이어의 공격 상태를 확인하기 위한 참조
    [SerializeField] Health health;                         // 플레이어가 사망했는지 알기위한 체력 확인 컴포넌트

    [Header("애니메이션 설정")]
    [SerializeField] float dampTime = 0.1f;                         // 애니메이션 파라미터의 변화 속도를 조절하는 댐핑 시간

    bool wasAttacking;                                              // 이전 프레임에서 공격 중이었는지 확인하는 변수

    int moveXHash;                                                  // Animator의 MoveX 파라미터 해시값
    int moveYHash;                                                  // Animator의 MoveY 파라미터 해시값
    int moveAmountHash;                                             // Animator의 MoveAmount 파라미터 해시값
    int isMovingHash;                                               // Animator의 IsMoving 파라미터 해시값
    int isDodgingHash;                                              // Animator의 IsDodging 파라미터 해시값
    int isAttackingHash;                                             // Animator의 isAttacking 파라미터 해시값
    int attackHash;                                                 // Animator의 Attack 트리거 해시값

    // 스크립터가 최초로 생성되릴 때 필요한 컴퍼넌트와 파라미터 해시값을 정의해서 준비
    // 미리 숫자ID로 변경하는 편이 안정적인 효과를 봄
    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if(inputReader == null)
            inputReader = GetComponent<PlayerInputReader>();
        if(dodgeCtr == null)
            dodgeCtr = GetComponent<PlayerDodgeController>();
        if(playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();

        moveXHash = Animator.StringToHash("MoveX");
        moveYHash = Animator.StringToHash("MoveY");
        moveAmountHash = Animator.StringToHash("MoveAmount");
        isMovingHash = Animator.StringToHash("IsMoving");
        isDodgingHash = Animator.StringToHash("IsDodging");
        isAttackingHash = Animator.StringToHash("IsAttacking");
        attackHash = Animator.StringToHash("Attack");

        if (animator != null)
            animator.applyRootMotion = false;
    }


    // 매 프레임마다 플레이어의 상태를 읽고 파라미터를 갱신
    void Update()
    {
        if (animator == null)
            return;

        if (health != null && health.IsDead)
        {
            animator.SetFloat(moveXHash, 0f);
            animator.SetFloat(moveYHash, 0f);
            animator.SetFloat(moveAmountHash, 0f);
            animator.SetBool(isMovingHash, false);
            animator.SetBool(isDodgingHash, false);
            animator.SetBool(isAttackingHash, false);
            return;
        }

        UpdateMoveAnimation();
        UpdateDodgeAnimation();
        UpdateAttackAnimation();
    }

    // 플레이어인풋리더에서 현재 입력값을 읽어 애니메이터에 전달하는 역할
    void UpdateMoveAnimation()
    {
        if (inputReader == null)
            return;

        // 좌우앞뒤 입력값을 받고 전체 크기값을 구한뒤 실제 이동 입력이 들어오는지 확인
        Vector2 moveInput = inputReader.MoveInput;
        float moveX = moveInput.x;
        float moveY = moveInput.y;
        float moveAmount = Mathf.Clamp01(moveInput.magnitude);
        bool isMoving = moveAmount > 0.01f;

        // 이동 애니메이션값 저장
        animator.SetFloat(moveXHash, moveX, dampTime, Time.deltaTime);
        animator.SetFloat(moveYHash, moveY, dampTime, Time.deltaTime);
        animator.SetFloat(moveAmountHash, moveAmount, dampTime, Time.deltaTime);
        animator.SetBool(isMovingHash, isMoving);
    }

    // 회피컨트롤러 컴포넌트의 현재 회피중인가를 읽고 애니메이터에 전달하는 역할
    void UpdateDodgeAnimation()
    {
        bool isDodging = false;

        if (dodgeCtr != null)
            isDodging = dodgeCtr.IsDodging;

        // 회피 애니메이션 전환
        animator.SetBool(isDodgingHash, isDodging);
    }

    // 플레이어공격 컴포넌트의 현재의 공격중인가를 읽고 애니메이터에 전달하는 역할
    void UpdateAttackAnimation()
    {
        bool isAttacking = false;

        if (playerCombat != null)
            isAttacking = playerCombat.IsAttacking;

        animator.SetBool(isAttackingHash, isAttacking);

        if (isAttacking && !wasAttacking)
            animator.SetTrigger(attackHash);

        // 공격중일 때프레임마다 반복실행되는것을 방지
        wasAttacking = isAttacking;
    }
}
