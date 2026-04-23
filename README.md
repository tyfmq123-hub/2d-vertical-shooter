# 2D Vertical Shooter

Unity로 제작한 2D 세로형 슈팅 게임 프로젝트입니다.  
플레이어 이동/발사, 적 스폰 패턴, 피격 처리, 점수/라이프 UI를 포함합니다.

---

## Preview

- 장르: 2D Vertical Scrolling Shooter
- 엔진: Unity (2D / URP 기반)
- 주요 구현: 적 스폰 매니저, 탄환 시스템, 플레이어 피격 및 리스폰, 점수 UI

---

## Features

- **플레이어 조작**
  - 방향키(또는 WASD) 이동
  - `Space` 발사
- **적 스폰 시스템**
  - 상단 스폰 포인트에서 수직 하강
  - 좌/우 스포너에서 대각선 진입
  - `GameManager`의 스폰 모드로 패턴 전환 가능
- **적 탄환 패턴**
  - 일반 적 탄환 발사
  - `Enemy C`는 플레이어 방향 조준 발사
- **UI/게임 흐름**
  - 라이프 감소 및 게임오버 패널
  - 점수 증가 표시

---

## Controls

- `← / → / ↑ / ↓` 또는 `W / A / S / D`: 이동
- `Space`: 발사

---

## Project Structure

- `Assets/GameManager.cs`: 적 생성과 스폰 모드 관리
- `Assets/Enemy.cs`: 적 이동/피격/발사/점수 처리
- `Assets/EnemyBullet.cs`: 적 탄환 이동/삭제
- `Assets/Player.cs`: 플레이어 이동/발사/피격/리스폰
- `Assets/UiManager.cs`: 라이프, 점수, 게임오버 UI 처리

---

## How To Run

1. Unity Hub에서 이 프로젝트 폴더를 엽니다.
2. `Assets/Scenes/SampleScene.unity`(또는 사용 중인 메인 씬)를 엽니다.
3. Play 버튼으로 실행합니다.

---

## Notes

- 스폰 패턴은 `GameManager` 인스펙터 값(`spawnMode`, `spawnPoints`, `spawners`)에 따라 달라집니다.
- 씬 오브젝트 참조가 비어 있으면 의도한 스폰/발사 패턴이 동작하지 않을 수 있습니다.

---

## Credit

- 학습 기반: 골드메탈님의 2D 슈팅게임 제작 학습 콘텐츠
