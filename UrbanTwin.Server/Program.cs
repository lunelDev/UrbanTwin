/*using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UrbanTwin.Server;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Urban Twin Server Started");
        //서버 ip & 포트
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        int port = 12345;

        TcpListener server = new TcpListener(ip, port);
        server.Start();
        Console.WriteLine($"Listening on {ip}:{port}");

        bool expression = true;
        while (expression)
        {
            // 클라이언트가 접속하면 데이터를 받고, 그대로 응답해주는 Echo Server 예제
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("[Connected] 클라이언트 접속됨");

// 클라이언트와 데이터를 주고받기 위한 네트워크 스트림 가져오기
            NetworkStream stream = client.GetStream();

// 수신 데이터 저장 버퍼 (최대 1024 바이트)
            byte[] buffer = new byte[1024];
            int bytesRead;

// 클라이언트가 데이터를 보내는 동안 반복
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                // 받은 데이터를 UTF-8 문자열로 변환
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[수신] {message}");

                // 응답 메시지 생성 (Echo 방식)
                string response = $"서버 응답: {message}";

                // 응답을 다시 바이트 배열로 변환
                byte[] responseData = Encoding.UTF8.GetBytes(response);

                // 클라이언트로 응답 전송
                stream.Write(responseData, 0, responseData.Length);
            }

// 클라이언트 연결 종료 후 리소스 해제
            client.Close();
        }
    }
}*/


// 브로드캐스트 형식의 서버
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UrbanTwin.Server
{
    class Program
    {
        static List<TcpClient> clients = new List<TcpClient>(); // 연결된 클라이언트 리스트
        static object lockObj = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("🚀 UrbanTwin Server Started");

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 12345;

            TcpListener server = new TcpListener(ip, port);
            server.Start();
            Console.WriteLine($"[Listening] {ip}:{port}");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                lock (lockObj) clients.Add(client);
                Console.WriteLine("[Connected] 클라이언트 접속됨!");

                // 각 클라이언트는 Task로 처리
                Task.Run(() => HandleClient(client));
            }
        }

        static void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[수신] {message}");

                    // 연결된 모든 클라이언트에 브로드캐스트
                    BroadcastMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[에러] {ex.Message}");
            }
            finally
            {
                lock (lockObj) clients.Remove(client);
                client.Close();
                Console.WriteLine("[Disconnected] 클라이언트 연결 종료");
            }
        }

        static void BroadcastMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            lock (lockObj)
            {
                foreach (var c in clients)
                {
                    try
                    {
                        NetworkStream s = c.GetStream();
                        s.Write(data, 0, data.Length);
                    }
                    catch
                    {
                        // 클라이언트 오류 무시
                    }
                }
            }
        }
    }
}
