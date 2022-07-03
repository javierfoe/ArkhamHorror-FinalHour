using System.Collections;
using UnityEngine;

public class Seal : MonoBehaviour
{
    private const int MaxUses = 2;
    [SerializeField] private UnityEngine.Color full, half;
    private SpriteRenderer _sprite;
    private int _uses;

    public bool Enabled => _uses > 0;

    public IEnumerator Disable()
    {
        _uses = 0;
        gameObject.SetActive(false);
        yield return null;
    }
    
    public void Use()
    {
        _uses--;
        RefreshSprite();
        if (_uses > 0) return;
        Disable();
    }

    public void Restore(int uses = MaxUses)
    {
        gameObject.SetActive(true);
        _uses = uses;
        RefreshSprite();
    }

    private void RefreshSprite()
    {
        _sprite.color = _uses > 1 ? full : half;
    }
    
    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        RefreshSprite();
        Restore();
    }
}
