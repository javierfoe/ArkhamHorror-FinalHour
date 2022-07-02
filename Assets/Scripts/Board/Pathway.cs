using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField] private Building one, two;
    [SerializeField] private Seal seal;
    [SerializeField] private bool activateSeal;

    public void AddSeal()
    {
        seal.Restore();
    }

    public bool MonsterDiesToSeal(Monster monster)
    {
        if (!seal.isActiveAndEnabled) return false;
        seal.Use();
        return true;
    }
    
    private void Awake()
    {
        one.AddAdjacent(two, this);
        two.AddAdjacent(one, this);
        if (!activateSeal) return;
        AddSeal();
    }
}
