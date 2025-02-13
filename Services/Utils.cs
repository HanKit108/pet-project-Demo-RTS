using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private const float DEFAULT_RAY_DISTANCE = 100f;
    
    public static Vector3 GetMousePositionFromScreen()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, DEFAULT_RAY_DISTANCE, ServiceLocator.GetService<GameManager>().GroundLayer))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
    public static bool IsPointInside(Vector3 point, Vector3 startPosition, Vector3 endPosition)
    {
        float minX = Mathf.Min(startPosition.x, endPosition.x);
        float maxX = Mathf.Max(startPosition.x, endPosition.x);
        float minZ = Mathf.Min(startPosition.z, endPosition.z);
        float maxZ = Mathf.Max(startPosition.z, endPosition.z);

        if (point.x > minX && point.x < maxX && point.z > minZ && point.z < maxZ)
            return true;

        return false;
    }
    public static List<Vector3> GetPositionListAround(Vector3 startPosition, float distanceBetween, int count)
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);

        int koef = 6;

        for (int i = 0; i < count - 1; i++)
        {
            float angle = (i % koef) * (360f / (koef * (WholeFromDivision(i, koef) + 1)));
            Vector3 dir = Quaternion.Euler(0, angle, 0) * new Vector3(1, 0, 0);
            Vector3 position = startPosition + dir * distanceBetween * (WholeFromDivision(i, koef) + 1);
            positionList.Add(position);
        }

        Vector3 positionsCenter = Vector3.zero;

        foreach (Vector3 position in positionList)
        {
            positionsCenter += position;
        }
        positionsCenter /= positionList.Count;

        Vector3 positionOffset = positionsCenter - startPosition;

        for (int i = 0; i < positionList.Count; i++)
        {
            positionList[i] -= positionOffset;
        }

        return positionList;
    }

    public static float WholeFromDivision(float a, float b)
    {
        return Mathf.Floor(a / b);
    }

}
