using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class NavGrid : MonoBehaviour
{
    [SerializeField] Node[] nodes;
    [SerializeField] int width;
    [SerializeField] int height;

    [SerializeField] Vector3 navGridPosition;

    public void GenerateNodes()
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        nodes = new Node[width * height];

    }

    private void OnDrawGizmos()
    {

        if (width <= 0 || height <= 0)
        {
            return;
        }
        if (nodes.Length != width * height)
        {
            return;
        }

        Gizmos.color = new Color(1, 0, 0.2f, 0.65F);

        Vector3 topLeft = this.transform.position + navGridPosition;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                DrawNodeGizmos(x, y, topLeft);
            }
        }
    }

    private void DrawNodeGizmos(int x, int y, Vector3 topLeft)
    {
        Vector3 offset = new Vector3(x, 0, y);

        Gizmos.DrawCube(topLeft + offset, Vector3.one * 0.5f);


        float handleSize = 0.15f;
        float handleOffset = 0.3f;

        Handles.color = new Color(0, 1, 0, 0.4f);

        Handles.SphereHandleCap(0, topLeft + offset + new Vector3(-handleOffset, 0, 0), Quaternion.identity, handleSize, EventType.Repaint);
        Handles.SphereHandleCap(0, topLeft + offset + new Vector3(handleOffset, 0, 0), Quaternion.identity, handleSize, EventType.Repaint);

        Handles.SphereHandleCap(0, topLeft + offset + new Vector3(0, 0, handleOffset), Quaternion.identity, handleSize, EventType.Repaint);
        Handles.SphereHandleCap(0, topLeft + offset + new Vector3(0, 0, -handleOffset), Quaternion.identity, handleSize, EventType.Repaint);
    }

}
