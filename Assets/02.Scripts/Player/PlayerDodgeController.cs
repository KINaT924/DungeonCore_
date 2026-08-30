using UnityEngine;

public class PlayerDodgeController : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 회피 동작만을 담당하는 스크립트입니다
    /// 회피는 일반 이동과 달리 성격이 달라 분리하였습니다
    /// </summary>

    [Header("레퍼런스")]
    [SerializeField] PlayerInputReader inputReader;              // 플레이어 입력값을 전달해주는 컴포넌트 참조 변수
    [SerializeField] Transform cameraTranform;                   // 회피 방향을 계산하기 위해 사용될 카메라 변수
    [SerializeField] PlayerCombat playerCombat;                  // 플레이어 입력 판정을 처리하기위한 컴포넌트 참조 변수
    [SerializeField] Health health;                              // 플레이어가 사망했는지 알기위한 체력 확인 컴포넌트
    CharacterController characterCtr;                            // 실제 플레이어의 회피를 처리하는 컴포넌트 참조 변수

    [Header("회피 설정")]
    [SerializeField] float dodgeSpeed = 10f;                     // 플레이어가 회피중의 이동 속도
    [SerializeField] float dodgeDur = 0.5f;                      // 플레이어의 회피가 지속되는 시간
    [SerializeField] float dodgeCoolTime = 1f;                   // 플레이어가 회피를 다시 할 수 있는 시간
    Vector3 dodgeDir;                                            // 회피 방향값  
    float dodgeTimer;                                            // 회피 지속시간을 체크하는 타이머
    float coolTimer;                                             // 회피 쿨타임을 체크하는 타이머
    bool isDodging;                                              // 플레이어가 회피중인지 체크하는 변수

    public bool IsDodging { get { return isDodging; } }          // 외부에서 회피중인지 확인할 수 있는 프로퍼티

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
        if (playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();

        characterCtr = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        BindInputEvents();
    }
    private void OnDisable()
    {
        UnBindInputEvents();
    }
    void Update()
    {
        // 죽었으면 행동 x
        if (health != null && health.IsDead)
        {
            EndDodge();
            return;
        }

        UpdateCoolTimer();
        UpdateDodge();
    }


    // 플레이어의 입력 이벤트를 구독하는 메소드
    void BindInputEvents()
    {
        if (inputReader == null)
            return;

        inputReader.DodgePressed += HandleDodgePressed;
    }

    // 입력 이벤트 구독을 해제하는 메소드
    // 같은 이벤트가 중복 구독되어 회피가 여러번 실행하는 문제를 해결하기 위함
    void UnBindInputEvents()
    {
        if (inputReader == null)
            return;

        inputReader.DodgePressed -= HandleDodgePressed;
    }

    // 회피 입력이 들어왔을 때 호출받는 메소드
    // 회피 가능한 상태인지부터 검사하기 위한 메소드입니다
    // 회피 불가능한 상태인경우 : 이미 회피 중, 쿨타임이 남아있는 경우, 필요한 참조가 연결되지 않은 경우
    void HandleDodgePressed()
    {
        // 죽었으면 행동 x
        if (health != null && health.IsDead)
            return;
        if (isDodging)
            return;

        if (coolTimer > 0f)
            return;

        if (characterCtr == null || cameraTranform == null)
            return;

        StartDodge();
    }


    // 회피를 실질적으로 시작하는 메소드
    // 회피 시작 시점에 필요한 변수들을 초기화하고 회피 상태로 전환합니다
    void StartDodge()
    {
        // 회피중이면 공격판정을 취소
        if (playerCombat != null)
            playerCombat.CancelAttack();

        dodgeDir = CalculateDodgeDirection();
        dodgeTimer = dodgeDur;
        coolTimer = dodgeCoolTime;
        isDodging = true;
    }


    // 현재 입력과 카메라 방향을 기반으로 회피 방향을 계산하는 메소드
    Vector3 CalculateDodgeDirection()
    {
        Vector2 input = inputReader.MoveInput;
        Vector3 cameraForword = cameraTranform.forward;
        cameraForword.y = 0f;
        cameraForword.Normalize();

        Vector3 cameraRight = cameraTranform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 inputDir = cameraForword * input.y + cameraRight * input.x;

        if(inputDir.sqrMagnitude > 0.01f)
        {
            inputDir.Normalize();
            return inputDir;
        }

        Vector3 forwardDir = transform.forward;
        forwardDir.y = 0f;
        forwardDir.Normalize();


        return forwardDir;
    }


    // 회피 쿨타임을 감소시키는 메소드
    void UpdateCoolTimer()
    {
        if (coolTimer <= 0f)
            return;
        
        // 0보다 작아지지 않도록 설정
        coolTimer -= Time.deltaTime;
        coolTimer = Mathf.Max(coolTimer, 0f);
    }

    // 회피가 진행 중 일때 이동을 처리하는 메소드
    void UpdateDodge()
    {
        if (health != null && health.IsDead)
            return;
        if (!isDodging)
            return;

        // 회피중일 때 회피스피드만큼 더 빠르게 이동
        characterCtr.Move(dodgeDir * dodgeSpeed * Time.deltaTime);
        dodgeTimer -= Time.deltaTime;

        if (dodgeTimer <= 0f)
            EndDodge();
    }


    // 회피 상태를 종료하는 메소드
    // 상태를 바꾸기만하는 메소드지만 다양한 이벤트 활용법이 가능할 수도 있어 따로 빼뒀습니다
    void EndDodge()
    {
        isDodging = false;
    }
}
