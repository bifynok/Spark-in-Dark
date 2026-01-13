using UnityEngine;

public class PathLineBuilder : MonoBehaviour
{
    [SerializeField] private LineRenderer linePrefab;

    public void DrawLine(Vector3 from, Vector3 to)
    {
        LineRenderer line = Instantiate(linePrefab, transform);
        line.positionCount = 2;
        line.SetPosition(0, from);
        line.SetPosition(1, to);
    }
}
