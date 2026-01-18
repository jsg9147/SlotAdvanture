# 🎰 Slot Adventure

<p align="center">
  <img src="https://img.shields.io/badge/Unity-2021.3-black?logo=unity" alt="Unity">
  <img src="https://img.shields.io/badge/C%23-10.0-239120?logo=csharp" alt="C#">
  <img src="https://img.shields.io/badge/Platform-Steam-1b2838?logo=steam" alt="Steam">
  <img src="https://img.shields.io/badge/Status-Released-success" alt="Status">
</p>

<p align="center">
  <a href="https://store.steampowered.com/app/2811070/Slot_Adventure/">
    <img src="https://img.shields.io/badge/🎮 Steam에서 플레이-blue?style=for-the-badge" alt="Steam">
  </a>
  <a href="https://youtu.be/Bo9siV3U6p0">
    <img src="https://img.shields.io/badge/🎬 트레일러 보기-red?style=for-the-badge" alt="Trailer">
  </a>
</p>

---

## 📌 프로젝트 개요

| 항목 | 내용 |
|:---:|:---|
| **개발 기간** | 2024.01 ~ 2024.03 (3개월) |
| **개발 인원** | 1인 개발 (기획, 프로그래밍, 레벨 디자인) |
| **장르** | 확률 기반 로그라이크 RPG |
| **플랫폼** | PC (Steam) |
| **엔진** | Unity 2021.3 LTS |
| **언어** | C# |
| **스크립트 규모** | 102개 C# 스크립트 |

---

## 🎯 기획 의도

> **"컨트롤이 아닌 전략으로 승부하는 로그라이크"**

- 모든 전투를 **확률 기반으로 진행**하여, 컨트롤 요소 없이 **전략 설계에 집중하는 경험** 제공
- **컨트롤에 자신 없는 유저들도 즐길 수 있는 게임**을 목표로 설계
- 캐릭터, 스킬, 아이템, 이벤트 등 모든 요소에 **랜덤성과 도박 요소**를 추가해 매 플레이마다 다른 재미 유도
- 3명의 캐릭터 파티 구성, 각 캐릭터는 랜덤 스탯과 랜덤 스킬 보유

---

## 🎮 게임 플로우

```
┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐
│  MAIN   │ → │  LOBBY  │ → │   MAP   │ → │ BATTLE  │
│ (메인)  │    │(유닛선택)│    │(던전탐색)│    │ (전투)  │
└─────────┘    └─────────┘    └─────────┘    └────┬────┘
                                                  ↓
┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐
│  STORE  │ ← │SANCTUARY│ ← │ GAMBLE  │ ← │TREASURE │
│ (상점)  │    │ (성소)  │    │(미니게임)│    │ (보물)  │
└─────────┘    └─────────┘    └─────────┘    └─────────┘
```

---

## 💻 핵심 시스템 구현

### 1. 확률 기반 전투 시스템 (1000분율)

모든 행동의 성공 여부를 1000분율 확률 시스템으로 결정합니다.

```csharp
// 슬롯 결과값 (0~1000) 기반 명중 판정
// 예: SlotResult = 700 → 70% 성공률
// 명중 조건: random(0~1000) - (ACC * 10) <= SlotResult

public bool CalculateHit(int slotResult, int accuracy)
{
    int randomValue = Random.Range(0, 1001);
    int adjustedValue = randomValue - (accuracy * 10);
    return adjustedValue <= slotResult;
}
```

**구현 포인트:**
- 슬롯 머신 UI와 연동하여 확률을 시각적으로 표현
- 명중률(ACC) 스탯으로 확률 보정
- 크리티컬, 회피 등 추가 확률 레이어 적용

---

### 2. 턴제 전투 시스템

```csharp
// UnitAction: 전투 행동 데이터 구조
public class UnitAction
{
    public Unit unit;              // 행동 주체
    public List<Unit> targets;     // 대상 유닛들
    public BattleAction action;    // ATK, SKILL, ITEM, DEFEND, ESCAPE
    public SkillObject skill;      // 사용 스킬
    public ItemData item;          // 사용 아이템
}
```

**턴 진행 순서:**
1. **행동 선택** → 각 유닛에게 명령 할당
2. **순서 정렬** → SPD 스탯 기준 행동 순서 결정
3. **행동 실행** → DOTween 애니메이션과 함께 순차 실행
4. **확률 판정** → 슬롯 시스템으로 성공 여부 결정
5. **효과 적용** → 버프/디버프 턴 카운트 처리

---

### 3. 유닛 데이터 구조

```csharp
// 계층적 유닛 데이터 설계
Unit (런타임 인스턴스)
├── UnitData (영구 저장 데이터)
│   ├── StatData (HP, AD, AP, DEF, MR, SPD, ACC)
│   ├── Equipment (장착 장비)
│   ├── List<SkillObject> (습득 스킬)
│   └── List<StatusEffect> (활성 버프/디버프)
└── Animation State (Idle, Walk, Attack, Skill, Die)
```

---

### 4. 매니저 아키텍처 (싱글톤 패턴)

모든 매니저는 `DontDestroyOnLoad`를 활용한 싱글톤 패턴으로 구현했습니다.

```
📁 StaticManager/ (전역 매니저)
├── GameManager        # 게임 상태, 유닛 데이터, 스테이지 관리
├── PrefabManager      # 유닛 프리팹 풀링 및 인스턴스화
├── EffectManager      # 이펙트 오브젝트 풀링
├── LoadingManager     # 비동기 씬 로딩
├── SlotMachineManager # 확률 계산 핵심 로직
└── LocalizationManager # 다국어 CSV 지원

📁 Gameplay Managers/ (씬별 매니저)
├── BattleManager      # 턴 순서, 액션 큐, 확률 판정
├── ItemManager        # 인벤토리, 아이템 관리
├── EquipmentManager   # 장비 스탯 계산
├── SkillManager       # 스킬북 학습 시스템
├── SanctuaryManager   # 부활 버프 관리
├── StoreManager       # 상점 시스템
├── TreasureManager    # 보상 분배
└── GambleManager      # 미니게임 (블랙잭, 사다리, 룰렛)
```

---

### 5. 데이터 기반 설계 (ScriptableObject)

모든 게임 콘텐츠를 ScriptableObject로 관리하여 확장성과 유지보수성을 확보했습니다.

```
📁 04_ScriptableObject/
├── Enemy/           # 스테이지별 몬스터 (Tutorial, Fire, Forest, Snow, Cave, Boss)
├── Item/            # 장비 및 소비 아이템 (무기, 방어구, 룬, 스킬북, 보물)
├── Skill/           # 스킬 타입별 분류 (Melee, Range, Buff, Debuff, MultiShot)
├── Monster Skill/   # 몬스터 전용 스킬 풀
└── SantuaryBuff/    # 성소 부활 버프 (5단계)
```

---

### 6. 상태 효과 시스템

```csharp
public class StatusEffect
{
    public int duration;           // 남은 턴 수
    public bool isDebuff;          // 버프/디버프 구분
    public bool isRevival;         // 성소 부활 버프 여부
    public StatModifier modifier;  // HP, AD, AP, DEF, MR 변화량
    public GameObject particle;    // 시각 효과
}
```

**성소 시스템 특징:**
- 부활 버프 보유 시 사망해도 HP 50% 회복 후 부활
- 부활 시 버프 소모 (중복 불가)
- 일반 버프와 별도 플래그로 관리

---

### 7. 다국어 지원 (CSV 기반)

```
📁 LocalizationCSV/
├── UI.csv           # 인터페이스 텍스트
├── Items.csv        # 아이템 이름/설명
├── Skills.csv       # 스킬 이름/설명
├── Units.csv        # 유닛 이름
└── Monsters.csv     # 몬스터 이름

CSV 형식: Key | English | Korean | Chinese
```

런타임에 `LocalizationManager`가 CSV를 파싱하여 현재 언어에 맞는 텍스트 제공

---

## 🛠 사용 기술 및 에셋

| 분류 | 기술/에셋 | 용도 |
|:---:|:---|:---|
| **애니메이션** | DOTween | 전투 연출, UI 트랜지션, 시퀀스 애니메이션 |
| **오디오** | Master Audio | BGM/SFX 분리, 플레이리스트 관리, 오디오 믹서 |
| **플랫폼** | Steamworks.NET | Steam 업적 (12종), 클라우드 세이브 |
| **저장** | Easy Save 3 | 게임 데이터 직렬화, 세이브/로드 |
| **UI** | TextMesh Pro | 고품질 텍스트 렌더링, CJK 폰트 지원 |
| **이펙트** | Epic Toon FX, Cartoon FX | 전투 파티클 이펙트 |

---

## 📊 주요 개발 기능 요약

| 기능 | 설명 |
|:---|:---|
| **확률 기반 전투** | 1000분율 시스템으로 모든 행동의 성공률 결정 |
| **슬롯 UI** | 확률 시스템을 슬롯 머신 UI로 시각화 |
| **로그라이크 던전** | 절차적 맵 생성, 보스 클리어 시 다음 층 진입 |
| **턴제 파티 전투** | SPD 기반 행동 순서, 3인 파티 운영 |
| **성소 시스템** | 사망 시 1회 부활 버프, 별도 클래스 관리 |
| **장비 시스템** | 실시간 스탯 반영, 장비 교체 시 자동 해제 |
| **인벤토리** | 장비/소모 아이템 분리, 슬롯 UI 기반 |
| **미니게임** | 블랙잭, 사다리타기, 룰렛 |
| **다국어 지원** | 영어, 한국어, 중국어 |

---

## 📈 개발 회고

### 잘된 점
- 1000분율 확률 시스템으로 직관적이면서도 깊이 있는 전략성 구현
- ScriptableObject 기반 데이터 설계로 콘텐츠 확장이 용이
- 싱글톤 매니저 패턴으로 시스템 간 명확한 역할 분리

### 개선할 점
- 슬롯 머신 UI에 더 다양한 인터랙션 추가 여지
- 인벤토리 UI의 사용자 편의성 보완 필요
- 던전 구조와 스킬 조합 다양성 확장 가능

---

## 🎬 플레이 영상

[![Slot Adventure Trailer](https://img.youtube.com/vi/Bo9siV3U6p0/maxresdefault.jpg)](https://youtu.be/Bo9siV3U6p0)

> 클릭하여 트레일러 영상을 확인하세요

---

## 🔗 링크

<p align="center">
  <a href="https://store.steampowered.com/app/2811070/Slot_Adventure/">
    <img src="https://img.shields.io/badge/Steam-구매하기-1b2838?style=for-the-badge&logo=steam" alt="Steam">
  </a>
</p>

---

<p align="center">
  <b>© 2024 MSGStudio. All Rights Reserved.</b>
</p>
