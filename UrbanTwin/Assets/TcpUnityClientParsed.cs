using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using TMPro;

public class TcpUnityClientParsed : MonoBehaviour
{
    public string serverIp = "127.0.0.1"; // 서버 IP
    public int port = 12345;              // 서버 포트
    public TMP_Text uiText;               // UI(TextMeshPro) 연결

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool running = false;

    private string latestMessage = "";
    public string[] parts;
    void Start()
    {
        try
        {
            client = new TcpClient(serverIp, port);
            stream = client.GetStream();
            running = true;

            // 별도 스레드에서 데이터 수신
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("[연결 성공] 서버에 접속했습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[에러] {e.Message}");
        }
    }

    void ReceiveData()
    {
        byte[] buffer = new byte[1024];

        while (running)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log($"[서버 수신] {message}");

                    // 받은 메시지를 latestMessage에 저장
                    latestMessage = message;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[수신 에러] {e.Message}");
                running = false;
            }
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(latestMessage) && uiText != null)
        {
           parts = latestMessage.Split(',');
            if (parts.Length == 3)
            {
                // 데이터 파싱
                string parsed = $"온도: {parts[0]}℃ | 교통량: {parts[1]} | 미세먼지: {parts[2]}";
                uiText.text = parsed;
            }
            else
            {
                // 예외 상황 (데이터가 잘못 들어온 경우)
                uiText.text = $"잘못된 데이터: {latestMessage}";
            }
        }
    }

    void OnApplicationQuit()
    {
        running = false;
        stream?.Close();
        client?.Close();
    }
}
