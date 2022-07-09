using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ClueSymbol : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Clue clue;
    

    public Clue Clue
    {
        get => clue;
        set
        {
            clue = value;
            _sprite.sprite = ClueSprites.GetSprite(clue);
        }
    }
    
    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
}
