using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Roulette : MonoBehaviour
{
	public Button spinBtn;
	//public Button betMoneyUpBtn;
	//public Button betMoneyDownBtn;
	//public TMP_Text bettingMoneyText;

	[SerializeField]
	private Transform piecePrefab;              // 룰렛에 표시되는 정보 프리팹
	[SerializeField]
	private Transform linePrefab;                   // 정보들을 구분하는 선 프리팹
	[SerializeField]
	private Transform pieceParent;              // 정보들이 배치되는 부모 Transform
	[SerializeField]
	private Transform lineParent;                   // 선들이 배치되는 부모 Transform
	[SerializeField]
	private RoulettePieceData[] roulettePieceData;          // 룰렛에 표시되는 정보 배열

	[SerializeField]
	private int spinDuration;               // 회전 시간
	[SerializeField]
	private Transform spinningRoulette;         // 실제 회전하는 회전판 Transfrom
	[SerializeField]
	private AnimationCurve spinningCurve;               // 회전 속도 제어를 위한 그래프

	private int accumulatedWeight;          // 가중치 계산을 위한 변수
	private bool isSpinning = false;            // 현재 회전중인지
	private int selectedIndex = 0;          // 룰렛에서 선택된 아이템

	#region Test

	private float minAngle;
	float firstAngle;

	GambleManager gambleManager;
	#endregion

	private void Awake()
	{
		CalculateWeightsAndIndices();
		CalAngle();
		SpawnPiecesAndLines();
		
		// Debug..
		//Debug.Log($"Index : {GetRandomIndex()}");
	}

    private void Start()
    {
		spinBtn.onClick.AddListener(() =>
		{
			spinBtn.interactable = false;
			Spin(EndOfSpin);

			gambleManager.GambleBetting();
			BetBtnCheck();
		});
	}
	public void SetGambleMgr(GambleManager gambleManager) => this.gambleManager = gambleManager; 
	public void BetBtnCheck()
    {
		spinBtn.interactable = ItemManager.Instance.currentMoney >= gambleManager.currentBetMoney;
	}

    void CalAngle()
    {
		float totalWeight = 0f;
		for (int i = 0; i < roulettePieceData.Length; i++)
        {
			totalWeight += roulettePieceData[i].chance;
		}

		minAngle = 360 / totalWeight;
		firstAngle = minAngle * roulettePieceData[0].weight * 0.5f;
	}

	private void SpawnPiecesAndLines()
	{
		for (int i = 0; i < roulettePieceData.Length; ++i)
		{
			Transform piece = Instantiate(piecePrefab, pieceParent.position, Quaternion.identity, pieceParent);
			float angle = minAngle * (roulettePieceData[i].weight - roulettePieceData[0].weight);
			//float firstAngle = minAngle * roulettePieceData[0].weight * 0.5f;
			float pieceAngle = i != 0 ? firstAngle - (minAngle * ((roulettePieceData[i].weight - roulettePieceData[i - 1].weight)*0.5f)) : 0;
			// 생성한 룰렛 조각의 정보 설정 (아이콘, 설명)
			piece.GetComponent<RoulettePiece>().Setup(roulettePieceData[i]);
			// 생성한 룰렛 조각 회전
			piece.RotateAround(pieceParent.position, Vector3.back, angle + pieceAngle);

			Transform line = Instantiate(linePrefab, lineParent.position, Quaternion.identity, lineParent);
			// 생성한 선 회전 (룰렛 조각 사이를 구분하는 용도)
			line.RotateAround(lineParent.position, Vector3.back, angle + firstAngle);
		}

    }

	private void CalculateWeightsAndIndices()
	{
		for (int i = 0; i < roulettePieceData.Length; ++i)
		{
			roulettePieceData[i].index = i;

			// 예외처리. 혹시라도 chance값이 0 이하이면 1로 설정
			if (roulettePieceData[i].chance <= 0)
			{
				roulettePieceData[i].chance = 1;
			}

			accumulatedWeight += roulettePieceData[i].chance;
			roulettePieceData[i].weight = accumulatedWeight;

			//Debug.Log($"({roulettePieceData[i].index}){roulettePieceData[i].description}:{roulettePieceData[i].weight}");
		}
	}

	private int GetRandomIndex()
	{
		int weight = Random.Range(0, accumulatedWeight);

		for (int i = 0; i < roulettePieceData.Length; ++i)
		{
			if (roulettePieceData[i].weight > weight)
			{
				return i;
			}
		}

		return 0;
	}

	public void Spin(UnityAction<RoulettePieceData> action = null)
	{
		if (isSpinning == true) return;

		// 룰렛의 결과 값 선택
		selectedIndex = GetRandomIndex();
		float selectedPieceAngle = selectedIndex > 0 ? firstAngle - (minAngle * ((roulettePieceData[selectedIndex].weight - roulettePieceData[selectedIndex - 1].weight) * 0.5f)) : 0;

		// 선택된 결과의 중심 각도
		float angle = minAngle * (roulettePieceData[selectedIndex].weight - roulettePieceData[0].weight);
		// 정확히 중심이 아닌 결과 값 범위 안의 임의의 각도 선택
		float leftOffset = (angle - (selectedPieceAngle * 0.4f)) % 360;
		float rightOffset = (angle + (selectedPieceAngle * 0.4f)) % 360;
		float randomAngle = Random.Range(leftOffset, rightOffset);

		// 목표 각도(targetAngle) = 결과 각도 + 360 * 회전 시간 * 회전 속도
		int rotateSpeed = 2;
		float targetAngle = (randomAngle + 360 * spinDuration * rotateSpeed);

		isSpinning = true;
		StartCoroutine(OnSpin(targetAngle, action));
	}

	private IEnumerator OnSpin(float end, UnityAction<RoulettePieceData> action)
	{
		float current = 0;
		float percent = 0;

		while (percent < 1)
		{
			current += Time.deltaTime;
			percent = current / spinDuration;

			float z = Mathf.Lerp(0, end, spinningCurve.Evaluate(percent));
			spinningRoulette.rotation = Quaternion.Euler(0, 0, z);

			yield return null;
		}

		isSpinning = false;

		if (action != null) action.Invoke(roulettePieceData[selectedIndex]);
	}

	private void EndOfSpin(RoulettePieceData selectedData)
	{
		spinBtn.interactable = true;

		int prize = (int)(gambleManager.currentBetMoney * selectedData.value);
		gambleManager.GetMoney(prize);
		BetBtnCheck();
		Debug.Log($"{selectedData.index}:{selectedData.description}:{selectedData.value}:{prize}");
	}
}
