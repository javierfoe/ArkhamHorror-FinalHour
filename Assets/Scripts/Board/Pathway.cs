using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Pathway : MonoBehaviour, IClickable<Pathway>
{
    [SerializeField] private Building one, two;
    [SerializeField] private Seal seal;
    [SerializeField] private bool activateSeal;

    public UnityEvent<Pathway> OnClick { get; } = new();
    public Seal Seal => seal;

    public IEnumerator AddSeal()
    {
        yield return Seal.Restore();
    }

    public IEnumerator AddGraySeal()
    {
        yield return Seal.Restore(1);
    }

    public bool MonsterDiesToSeal(Monster monster)
    {
        if (!Seal.Enabled) return false;
        Seal.Use();
        return true;
    }

    public void OnMouseDown()
    {
        OnClick.Invoke(this);
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
}