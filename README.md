# Paraller_Divergence

## 1. 소개

- 이 프로젝트는 Unity로 개발한 3D RPG 게임입니다.
  
- 개발 기간: 2025.03.27 ~ 2025.06.06(약 3개월)
  
- Repository에는 소스코드만 등록되어 있습니다.
  
- 시연 기준으로 작성된 코드입니다.
  
- 현재 디벨롭 작업 중에 있으며 추후 해당 README 파일은 수정 될 예정입니다.
- 
<br><br><br>
## 2. 개발 환경

- Unity 2022.3.60f1 LTS
  
- C#
  
- Window 10

<br><br><br>
## 3. 기능

- **플레이어 이동**: 방향키를 이용해 플레이어가 플랫포머 형식으로 이동하도록 구현.
  
- **해킹 카메라**: Cinemachine Virtual Camera를 활용하여 좌, 우 방향키 입력 시 선택 된 객체에 따른 카메라 움직임 구현현.
  
- **페이크 로딩**: 로딩 화면을 페이크로 구현하여 실제 로딩 시간을 감추고 시각적으로 표현.
  
- **콤보 공격**: 일정 시간 내에 공격키를 누르면 연속 공격(콤보)이 발동되며 공격 애니메이션의 특정 프레임에서 콤보 체크가 시작되고 콤보가 이어지도록 구현.
  
- **스킬**: 스킬 사용 시 스킬에 알맞는 애니메이션과 이동을 재생하며 사용 중에는 피격과 사망을 제외한 모든 조작이 불가능하도록 설정.
  
- **플레이어 피격 깜빡임**: 플레이어 피격 시 상태에 따라 슈퍼아머가 적용되며 Material을 복사해 Emission을 On/Off하여 깜빡이도록 구현.
  
- **Enemy AI**: FSM을 이용하여 적 캐릭터들이 플레이어를 추적하고 공격하는 AI 구현.
  
- **거리 기반 렌더링**: MeshRenderer 비활성화를 통한 거리 기반 렌더링 최적화

<br><br><br>
## 4. 기술 스택

- **Unity**: 게임 개발 엔진
  
- **C#**: 게임 로직 및 스크립트 작성
  
- **Visual Studio**: 코드 편집기
  
- **싱글톤**: Manager.cs 등 전역에서 하나만 필요한 객체 관리
  
- **오브젝트 풀링**: 성능 최적화를 위한 객체 재사용 기법
  
- **FSM (Finite State Machine)**: 적 AI 구현 및 상태 기반 행동 처리
  
- **UI 자동화 시스템**: 동적 UI 요소 관리 및 효율적 바인딩
  
- **Scriptable Object**: Player의 Move Data 관리
  
- **IK Manager**: Bone 형식 객체의 팔, 다리 등 자연스러운 물리적인 움직임 구현
  
- **Radiant Volume**: Material의 설정으로 발광 효과를 부여해 빛 효과 및 처리
  
- **Baked Lightmap**: Radiant Volume으로 인한 성능 저하를 위해 배경 Object에 베이킹 처리
  
- **Cinemachine Virtual Camera**: 플레이어 및 해킹 시 카메라 이동 구현, 카메라 흔들림 효과 구현

<br><br><br>
## 5. 설치 및 실행 방법
1. [게임 실행 파일 다운로드](https://drive.google.com/drive/folders/1QygPo52y0TPllSDVahST_C7E8gCYdTm4)
2. `Parallel_Divergence_시연.zip` 파일을 압축 해제합니다.
3. `TeamProject_VS.exe` 파일을 실행하여 게임을 시작합니다.

<br><br><br>
## 6. 영상 링크
1. [유튜브 시연 영상](https://youtu.be/ieOsy1KRgUU)
2. [유튜브 베타 테스트 영상](https://youtu.be/EvxuqcQSgqU): 시연 영상 전 버전입니다.

<br><br><br>
## 7. License

This work is licensed under the Creative Commons Attribution-NonCommercial 4.0 International License. 

You may not use the material for commercial purposes. 

For more details, see: [https://creativecommons.org/licenses/by-nc/4.0/](https://creativecommons.org/licenses/by-nc/4.0/)
