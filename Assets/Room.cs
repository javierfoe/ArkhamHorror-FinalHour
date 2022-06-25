using UnityEngine;

public class Room : Location
{
    [SerializeField] private UnityEngine.Color normal, wrecked;
    private SpriteRenderer _sprite;
    private bool _wreckage;

    public bool Wreckage
    {
        get => _wreckage;
        set
        {
            _wreckage = value;
            _sprite.color = _wreckage ? wrecked : normal;
        }
    }

    public override bool IsFree => base.IsFree && !Wreckage;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
}
