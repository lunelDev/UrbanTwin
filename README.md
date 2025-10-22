
# UrbanTwin - 디지털트윈 & IoT 데이터 시각화

## 프로젝트 개요
**UrbanTwin**은 가상의 IoT 센서 데이터를 생성하고, 이를 서버를 통해 Unity 클라이언트에서 실시간으로 시각화하는 **디지털 트윈 데모 프로젝트**입니다.  
센서 시뮬레이터(Client) → 서버(Server) → Unity 클라이언트(Unity)로 이어지는 데이터 파이프라인을 직접 설계하고 구현했습니다.  

이 프로젝트는 **IoT 데이터의 실시간 흐름**과 **Unity 기반 시각화 시스템 구조**를 학습하기 위한 개인 프로젝트로,  
디지털 트윈의 기본 개념을 이해하고자 하는 개발자에게 좋은 예제입니다.  


## 기술 스택

| 구분 | 사용 기술 |
|------|------------|
| **개발 환경** | Unity **6000.2.7f2**, .NET 6 (C#) |
| **개발 도구** | Rider, Visual Studio |
| **네트워크 통신** | TCP/IP 소켓 (Client–Server–Unity 구조) |
| **시각화** | Unity UI, Scrollbar, Image, TextMeshPro, LineRenderer |
| **멀티스레딩** | C# Thread + UnityMainThreadDispatcher |
| **운영 체제** | Windows 11 환경 테스트 완료 |

## 주요 기능

### 🔹 1. 센서 시뮬레이터 (UrbanTwin.Client)
- 온도, 교통량, 미세먼지 데이터를 **랜덤 생성**
- TCP 소켓을 통해 서버(`127.0.0.1:12345`)로 데이터 전송
- 서버로부터의 응답(Echo) 수신 후 콘솔에 출력
  
```csharp
  string sensorData = $"{temperature},{traffic},{dust}";
  stream.Write(Encoding.UTF8.GetBytes(sensorData));
  Console.WriteLine($"[송신] {sensorData}");
````

### 🔹 2. 서버 (UrbanTwin.Server)

* `TcpListener` 기반 서버로 다중 클라이언트 연결 관리
* 클라이언트에서 수신된 데이터를 **브로드캐스트(Echo 중계)**
* 연결 종료 및 예외 발생 시 자동 정리

  ```csharp
  static void BroadcastMessage(string message)
  {
      byte[] data = Encoding.UTF8.GetBytes(message);
      foreach (var c in clients)
          c.GetStream().Write(data, 0, data.Length);
  }
  ```

### 🔹 3. Unity 클라이언트 (UrbanTwin.Unity)

* 서버에 TCP 클라이언트로 연결하여 **실시간 데이터 수신**
* `"온도,교통량,미세먼지"` 데이터를 파싱해 UI에 표시
* 데이터별 시각화 방식

  * **온도:** Scrollbar 게이지 + 색상 보간 (파랑 ↔ 빨강)
  * **교통량:** Scrollbar 게이지 + 색상 변화 (녹 → 노랑 → 빨강)
  * **미세먼지:** 상태 아이콘 교체 (매우 좋음 → 매우 나쁨)
* **히스토리 그래프(LineRenderer)** 기반 실시간 로그 차트 구현

  * 일정 개수 이상 시 자동 데이터 삭제 → 최근 로그만 유지


## 실행 흐름

```
[ UrbanTwin.Client ]  →  [ UrbanTwin.Server ]  →  [ UrbanTwin.Unity ]
센서 데이터 생성            데이터 브로드캐스트         실시간 시각화 및 로그 관리
```

## 🖼 시연 화면

| UrbanTwin.Client (센서 시뮬레이터)                                                      | UrbanTwin.Server (TCP 서버)                                                     | UrbanTwin.Unity (시각화)                                                                 |
| -------------------------------------------------------------------------------- | ----------------------------------------------------------------------------- | ------------------------------------------------------------------------------------- |
| <img src="./docs/urbanTwin_Client.png" width="250"/> <br/>센서 데이터(온도·교통량·미세먼지) 송신 | <img src="./docs/urbanTwinServer.png" width="250"/> <br/>서버에서 데이터 수신 및 브로드캐스트 | <img src="./docs/urbanTwin_unity.png" width="250"/> <br/>Unity UI 게이지·아이콘·히스토리 차트 시각화 |


## 실행 방법

### 1️⃣ 서버 실행

```bash
cd UrbanTwin.Server
dotnet run
```

### 2️⃣ 센서 시뮬레이터 실행

```bash
cd UrbanTwin.Client
dotnet run
```

### 3️⃣ Unity 실행

1. `UrbanTwin.Unity/` 폴더를 Unity Editor로 열기
2. `SampleScene` 실행
3. 실시간 데이터 수신 및 시각화 확인

## 📂 프로젝트 구조

```
UrbanTwin/
├── UrbanTwin.Client/
│   └── Program.cs              # 센서 시뮬레이터 (TCP 송신)
├── UrbanTwin.Server/
│   └── Program.cs              # TCP 서버 (브로드캐스트 중계)
└── UrbanTwin.Unity/
    ├── TcpUnityClientParsed.cs # 데이터 수신 및 파싱
    ├── UrbanTwin_UIManager.cs  # UI 업데이트 및 그래프 제어
    ├── HistoryChart.cs         # 실시간 로그 차트 (LineRenderer)
    └── Scenes/
        └── SampleScene.unity
```


