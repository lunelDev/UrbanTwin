using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;
using TMPro; // TextMeshPro를 UI로 쓸 경우 필요

public class TcpUnityClient : MonoBehaviour
{
    public string serverIp = "127.0.0.1"; // 서버 IP
    public int port = 12345;              // 서버 포트
    public TMP_Text uiText;               // Unity UI(TextMeshPro) 연결

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool running = false;

    void Start()
    {
        try
        {
            client = new TcpClient(serverIp, port);
            stream = client.GetStream();
            running = true;

            // 데이터 수신을 별도 스레드에서 실행
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

                    // UI 업데이트는 Unity 메인 스레드에서만 가능하므로 큐에 전달
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        if (uiText != null)
                            uiText.text = $"서버 데이터: {message}";
                    });
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[수신 에러] {e.Message}");
                running = false;
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
