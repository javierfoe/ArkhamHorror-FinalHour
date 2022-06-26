using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField] private Building one, two;
    [SerializeField] private Seal seal;

    public void AddSeal()
    {
        seal.Restore();
    }

    public bool MonsterDiesToSeal(Monster monster)
    {
        if (!seal.isActiveAndEnabled) return false;
        monster.Destroy();
        seal.Use();
        return true;
    }
    
    private void Awake()
    {
        one.AddAdjacent(two, this);
        two.AddAdjacent(one, this);
        AddSeal();
    }
}
