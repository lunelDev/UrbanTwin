using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UrbanTwin.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Urban Twin Sensor Client Started");

            string serverIp = "127.0.0.1"; // 서버 IP
            int port = 12345;              // 서버 포트 (서버랑 맞춰야 함)
            try
            {
                // 서버에 연결
                TcpClient client = new TcpClient(serverIp, port);
                Console.WriteLine("[연결 성공] 서버에 접속했습니다.");

                NetworkStream stream = client.GetStream();
                Random rand = new Random();
                while (true)
                {
                    // 랜덤 센서 데이터 생성
                    int temperature = rand.Next(-30, 40);   // 온도 (-15~35도)
                    int traffic = rand.Next(0, 100);       // 교통량 (0~100대)
                    int dust = rand.Next(10, 200);         // 미세먼지 (10~200)

                    string sensorData = $"{temperature},{traffic},{dust}";
                    byte[] data = Encoding.UTF8.GetBytes(sensorData);

                    // 서버로 전송
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"[송신] {sensorData}");

                    // 서버 응답 수신
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[서버 응답] {response}");

                    // 1초 대기 후 반복
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[에러] {ex.Message}");
            }
        }
    }
}