using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class objectalphachange : MonoBehaviour
{
    private float _alpha = 0.5f;
    private SpriteRenderer _SpriteRenderer;
    private Color _Color;

    // Start is called before the first frame update
    void Start()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _Color = _SpriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _SpriteRenderer.color = new Color(_Color.r, _Color.g, _Color.b, _alpha);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _SpriteRenderer.color = _Color;
        }
    }
}
