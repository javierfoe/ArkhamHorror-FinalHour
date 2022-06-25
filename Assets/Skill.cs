using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] private UnityEngine.Color[] colors;
    private SpriteRenderer _spriteRenderer;

    public void ChangeSkill(Monster.Skill skill)
    {
        _spriteRenderer.color = colors[(int)skill];
    }
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
