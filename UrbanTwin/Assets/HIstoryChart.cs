using System.Collections.Generic;
using UnityEngine;

public class HistoryChart : MonoBehaviour
{
    public LineRenderer lineRenderer;
    float xSpacing =1.2f;   // X축 간격
    float yScale = 0.05f;    // Y축 스케일
    int maxPoints = 50;     // 최대 데이터 개수

    private Queue<float> dataPoints = new Queue<float>();
    private float lastValue = float.NaN; // 마지막 값 저장 (처음에는 비어있음)

    public void AddData(float value)
    {
        // 값이 같으면 추가하지 않음
        if (!float.IsNaN(lastValue) && Mathf.Approximately(value, lastValue))
        {
            return;
        }

        lastValue = value; // 마지막 값 갱신

        // 새로운 데이터 추가
        dataPoints.Enqueue(value);

        // 오래된 데이터 삭제
        if (dataPoints.Count > maxPoints)
        {
            dataPoints.Dequeue();
        }

        UpdateChart();
    }

    void UpdateChart()
    {
        lineRenderer.positionCount = dataPoints.Count;

        int i = 0;
        foreach (var val in dataPoints)
        {
            float x = i * xSpacing;     // 계속 오른쪽으로 증가
            float y = val * yScale;
            lineRenderer.SetPosition(i, new Vector3(x, y, -1));
            i++;
        }
    }
}