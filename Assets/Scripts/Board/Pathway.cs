using UnityEngine;

public class Pathway : Clickable<Pathway>
{
    [SerializeField] private Building one, two;
    [SerializeField] private Seal seal;
    [SerializeField] private bool activateSeal;
    public Seal Seal => seal;

    public void AddSeal()
    {
        Seal.Restore();
    }

    public bool MonsterDiesToSeal(Monster monster)
    {
        if (!Seal.Enabled) return false;
        Seal.Use();
        return true;
    }
    
    private void Awake()
    {
        one.AddAdjacent(two, this);
        two.AddAdjacent(one, this);
    }

    private void Start()
    {
        if (!activateSeal)
        {
            seal.Disable();
            return;
        }
        AddSeal();
    }

    protected override Pathway InvokeArgument()
    {
        return this;
    }
}
