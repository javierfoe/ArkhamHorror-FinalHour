using UnityEngine;

public class Seal : MonoBehaviour
{
    private const int MaxUses = 2;
    [SerializeField] private UnityEngine.Color full, half;
    private SpriteRenderer _sprite;
    private int _uses;
    
    public void Use()
    {
        _uses--;
        if (_uses > 0) return;
        gameObject.SetActive(false);
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
