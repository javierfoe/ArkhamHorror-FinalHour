using UnityEngine;

public abstract class Dweller : MonoBehaviour
{
    [SerializeField] private Transform hitPoints;
    private int _currentHp;
    protected int MaxHp;

    public void Hit(int damage)
    {
        for (var i = 0; i < damage; i++) Hit();
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    private void Hit()
    {
        _currentHp--;
        if (_currentHp == 0) return;
        hitPoints.GetChild(MaxHp - _currentHp - 1).gameObject.SetActive(false);
    }

    protected virtual void Start()
    {
        _currentHp = MaxHp;
        for (var i = 0; i < hitPoints.childCount - MaxHp; i++)
        {
            hitPoints.GetChild(i).gameObject.SetActive(false);
        }

        for (var i = hitPoints.childCount - MaxHp; i < hitPoints.childCount; i++)
        {
            hitPoints.GetChild(i).gameObject.SetActive(true);
        }
    }
}

public abstract class DwellerGeneric<T> : Dweller where T : Location
{
    private T _location;

    public T Location
    {
        get => _location;
        set
        {
            if (_location && _location != value) Empty(_location);
            _location = value;
            if (_location) Fill(_location);
        }
    }

    protected virtual void Empty(Location location)
    {
        _location.SetDweller(null);
    }

    protected virtual void Fill(Location location)
    {
        _location.SetDweller(this);
    }
}