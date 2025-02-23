using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float moveSpeed = 2.0f; // 움직임 속도
    public float changeInterval = 2.0f; // 움직임을 변경하는 간격

    private float timeSinceLastChange;
    private int direction; // -1 (왼쪽) 또는 1 (오른쪽)

    private Camera mainCamera;
    Animator animator;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        mainCamera = Camera.main; // 메인 카메라를 가져옵니다.

        direction = Random.Range(0, 2) == 0 ? -1 : 1;

        // 이동 시작 시간 설정
        timeSinceLastChange = Time.time;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.SetBool("isWalk", true);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    bool OutCamera()
    {
        // 오브젝트의 월드 좌표를 뷰포트 좌표로 변환
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // 뷰포트 좌표를 사용하여 카메라 밖으로 나갔는지 확인
        if (viewportPosition.x < 0 || viewportPosition.x > 1 ||
            viewportPosition.y < 0 || viewportPosition.y > 1)
        {
            return true;
        }

        return false;
    }

    void Move()
    {
        // 현재 위치
        Vector3 currentPosition = transform.position;

        // 현재 위치에서 새로운 위치 계산
        currentPosition.x += direction * moveSpeed * Time.deltaTime;
        spriteRenderer.flipX = (direction != 1);
        // 오브젝트가 화면 바깥으로 나가면 방향 반전
        if (OutCamera())
        {
            currentPosition.x = 8 * direction;
            direction *= -1;
        }

        // 새로운 위치로 이동
        transform.position = currentPosition;

        // 일정 간격마다 움직임 방향 변경
        if (Time.time - timeSinceLastChange > changeInterval)
        {
            direction *= -1; // 방향 반전
            timeSinceLastChange = Time.time; // 시간 재설정

            changeInterval = Random.Range(4,8);
        }
    }
}
