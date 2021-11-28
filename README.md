# ToyParty-HexaBlast

## 1. 과제 내용

-   Toy Party - Hexa Blast 게임에서 21 레벨을 구현.

- 필수 구현 항목
    -   ~~드랍랍로직 : 새로 생성된 블럭이 좌우로 흘러내리는 로직~~ (구현 완료)
    -   ~~매칭조건 : 직선 3개 이상, 4개 이상의 블럭이 모일 경우~~  (구현 완료)

- 가산점 항목
    -   장애물 : 팽이
    -   기타 : UI, 특수블럭 생성, 특수블럭 기능 등 필수 구현 항목 이외의 기능은 자유롭게 추가

----

## 기타 구현한 내용

1. UniRx를 사용하여 MVP 패턴으로 유지보수에 용이하게끔 로직과 View 분리
2. StateMachine으로 상태 관리
3. 블록 Shuffle 기능 구현
4. 엑셀 데이터 연동
5. 오브젝트 풀링 구현

# Environments

-   Unity@2020.3.15.f2
-   Unirx@7.1.0
-   Lean Touch@2.3.5
-   DoTween@1.2.632
-   Excel2Json@v1.0
-   UniTask@2.2.5
-   NetonSoftJson