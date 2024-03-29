using UnityEngine;
using UnityEngine.Events;

public class Room : Location, IClickable<Room>
{
    [SerializeField] private UnityEngine.Color normal, wrecked;
    [SerializeField] private bool awakeWrecked;
    private SpriteRenderer _sprite;
    private bool _wreckage;
    public UnityEvent<Room> OnClick { get; } = new();

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
        Wreckage = awakeWrecked;
    }

    public void OnMouseDown()
    {
        OnClick.Invoke(this);
    }
}
