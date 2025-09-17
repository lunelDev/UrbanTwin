using System;
using UnityEngine;
using UnityEngine.UI;

public class UrbanTwin_UIManager : MonoBehaviour
{
    public TcpUnityClientParsed tcpUnityClientParsed;

    [Header("온도")]
    public Scrollbar temperatureBar;
    float minTemp = -30f;
    float maxTemp = 50f;
    [SerializeField] private Image tempHandleImage; // 핸들 색상 바꾸는 용도

    [Header("미세먼지")]
    [SerializeField] private Image fineDustImage;
    [SerializeField] private Sprite[] fineDustSprite; // 0=매우좋음, 1=좋음, 2=나쁨, 3=매우나쁨

    [Header("교통량")]
    public Scrollbar trafficBar;
    [SerializeField] private Image trafficHandleImage; // 교통량 핸들 색상
    int maxTraffic = 100; // 최대 교통량
    
    
    [Header("히스토그래프")]
    public HistoryChart temperatureChart;
    public HistoryChart trafficChart;
    public HistoryChart dustChart;

    private void Update()
    {
        if (tcpUnityClientParsed.parts != null && tcpUnityClientParsed.parts.Length >= 3)
        {
            // 온도
            int temperature = int.Parse(tcpUnityClientParsed.parts[0]);
            UpdateTemperatureUI(temperature);
            temperatureChart.AddData(temperature);
            // 교통량
            int traffic = int.Parse(tcpUnityClientParsed.parts[1]);
            UpdateTrafficUI(traffic);
            trafficChart.AddData(traffic);
            // 미세먼지
            int fine_dust = int.Parse(tcpUnityClientParsed.parts[2]);
            UpdateFineDustUI(fine_dust);
            dustChart.AddData(fine_dust);
        }
    }

    void UpdateTemperatureUI(int temp)
    {
        // 0 ~ 1 정규화
        float normalized = (temp - minTemp) / (maxTemp - minTemp);
        normalized = Mathf.Clamp01(normalized);
        temperatureBar.value = normalized;

        // 색상 보간
        Color cold = Color.blue;
        Color hot = Color.red;

        if (tempHandleImage != null)
        {
            tempHandleImage.color = Color.Lerp(cold, hot, normalized);
        }
    }

    void UpdateFineDustUI(int dust)
    {
        if (dust <= 30)
        {
            fineDustImage.sprite = fineDustSprite[0]; // 매우 좋음
        }
        else if (dust <= 80)
        {
            fineDustImage.sprite = fineDustSprite[1]; // 좋음
        }
        else if (dust <= 150)
        {
            fineDustImage.sprite = fineDustSprite[2]; // 나쁨
        }
        else
        {
            fineDustImage.sprite = fineDustSprite[3]; // 매우 나쁨
        }
    }

    void UpdateTrafficUI(int traffic)
    {        
        // 0~1 정규화 후 Scrollbar 값 갱신
        float normalized = (float)traffic / maxTraffic;
        trafficBar.size = Mathf.Clamp01(normalized);
        // 색상 변화 (녹 → 노랑 → 빨강)
        if (traffic < 30)
        {
            if (trafficHandleImage != null) trafficHandleImage.color = Color.green;
        }
        else if (traffic < 70)
        {
            if (trafficHandleImage != null) trafficHandleImage.color = Color.yellow;
        }
        else
        {
            if (trafficHandleImage != null) trafficHandleImage.color = Color.red;
        }
    }
}
