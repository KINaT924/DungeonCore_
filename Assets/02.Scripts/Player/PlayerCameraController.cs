using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 마우스 시점 입력을 받아
    /// CinemaMachine의 카메라 회전시키는 기준을 계산하는 컴포넌트입니다
    /// </summary>


    [SerializeField] PlayerInputReader inputReader;     // 플레이어 입력값을 전달해주는 스크립트 참조 변수
    [SerializeField] Transform cameraTarget;            // CinemaMachine이 따라갈 타겟
    [SerializeField] Transform playerPos;               // 플레이어의 몸 방향을 돌릴 좌표값

    [SerializeField] float mouseSenitivity = 0.12f;     // 마우스 감도
    [SerializeField] bool invertY = false;              // 마우스 Y축 반전 여부

    [SerializeField] float minPitch = -35f;             // 카메라가 바라볼 수 있는 최소 각도
    [SerializeField] float maxPitch = 70f;              // 카메라가 바라볼 수 있는 최대 각도

    [SerializeField] bool lookCursorOnstart = true;     // 마우스 커서를 화면 중앙에 고정할지 여부

    float yaw;                                          // 카메라의 좌우 회전값
    float pitch;                                        // 카메라의 상하 회전값

    private void Awake()
    {
        InitializeReferences();
    }

    private void Start()
    {
        InitializeRotation();

        if (lookCursorOnstart)
            LookCursor();
    }

    private void LateUpdate()
    {
        HandleLookInput();
        RotatePlayerBody();
        RotateCameraTarget();
    }

    // Inspertoc에서 연결이 되지않았을 때 기본값을 지정하는 백업용 메소드
    void InitializeReferences()
    {
        if (playerPos == null)
            playerPos = transform;
    }

    // 게임 시작 시 현재 회전값을 반영시키는 메소드
    void InitializeRotation()
    {
        if(cameraTarget == null)
            return;

        Vector3 currentEuler = cameraTarget.rotation.eulerAngles;

        yaw = currentEuler.y;
        pitch = NormalizeAngle(currentEuler.x);
    }

    // LookInput값을 받아와 yaw와 pitch값을 계산시키는 메소드
    void HandleLookInput()
    {
        if(inputReader == null)
            return;

        Vector2 lookInput = inputReader.LookInput;
        yaw += lookInput.x * mouseSenitivity;

        float yInput = invertY ? lookInput.y : -lookInput.y;
        pitch += yInput * mouseSenitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    // 캐릭터 몸을 마우스 좌우 시점으로 회전시키는 메소드 ( 시점 방향을 기준으로 캐릭터 회전 )
    void RotatePlayerBody()
    {
        if (playerPos == null)
            return;
        playerPos.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    // 시네머신이 따라갈 카메라를 회전시키는 메소드
    void RotateCameraTarget()
    {
        if (cameraTarget == null)
            return;
        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }


    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    void LookCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
