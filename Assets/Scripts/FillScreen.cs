using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // scale it to fill the screen
        Renderer render = GetComponentInChildren<Renderer>();
        Bounds bound = render.bounds;
        Vector3 botRight = Camera.main.WorldToViewportPoint(bound.max);
        Vector3 topLeft = Camera.main.WorldToViewportPoint(bound.min);
        Vector3 diagonal = botRight - topLeft;
        float scaleX = 1.0f / diagonal.x;
        float scaleY = 1.0f / diagonal.y;
        float scale = Mathf.Max(scaleX, scaleY);
        transform.localScale = scale * transform.localScale;
        // and center it
        Vector3 pos = transform.position;
        pos.x = Camera.main.transform.position.x;
        pos.y = Camera.main.transform.position.y;
    }
}
