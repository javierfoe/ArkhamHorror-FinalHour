using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] private UnityEngine.Color[] colors;
    private SpriteRenderer _spriteRenderer;

    public void ChangeSkill(MonsterSkill monsterSkill)
    {
        _spriteRenderer.color = colors[(int)monsterSkill];
    }
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
