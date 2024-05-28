using System;
using UnityEngine;

public abstract class Potion : Item
{
    [SerializeField] private float leftTime;
    
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private float _speed;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _speed = 0.5f;
    }

    public virtual void Use()
    {
        transform.position = Managers.Game.SelectedCharacter.transform.position;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = Managers.Game.SelectedCharacter.GetComponent<SpriteRenderer>().sortingOrder + 1;
        Managers.Game.SelectedCharacter.Items.Remove(this);
        gameObject.SetActive(true);
        _animator.SetTrigger("Use");
        _rigidbody2D.velocity = transform.up * _speed;
        Destroy(gameObject, leftTime);
    }
}