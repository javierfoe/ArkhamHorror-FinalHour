using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField] private Building one, two;
    
    private void Awake()
    {
        one.AddAdjacent(two, this);
        two.AddAdjacent(one, this);
    }
}
