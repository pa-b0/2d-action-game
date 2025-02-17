using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField, Header("移動速度")] private float _moveSpeed;
    [SerializeField, Header("攻撃力")] private int _attackPower;

    private Rigidbody2D _rigid;
    private Vector2 _moveDirection;
    private Animator _anime;
    private bool _bFloor;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _anime = GetComponent<Animator>();
        _moveDirection = Vector2.left;
        _bFloor = true;
    }

    // Update is called once per frame
    void Update()
    {
        _Move();
        _ChangeMoveDirection();
        _LookMoveDirection();
        _HitFloor();
    }

    private void _Move()
    {
        if (!_bFloor) return;
        _rigid.velocity = new Vector2(_moveDirection.x * _moveSpeed, _rigid.velocity.y);
    }

    private void _ChangeMoveDirection()
    {
        Vector2 halfSize = transform.lossyScale / 2.0f;
        int layerMask = LayerMask.GetMask("Floor");
        RaycastHit2D ray = Physics2D.Raycast(transform.position, -transform.right, halfSize.x + 0.1f, layerMask);
        
        if (ray.transform == null) return;
        if (ray.transform.tag == "Floor")
        {
            _moveDirection = -_moveDirection;
        }
    }

    private void _LookMoveDirection()
    {
        if (_moveDirection.x < 0.0f)
        {
            transform.eulerAngles = Vector3.zero;
        } else if (_moveDirection.x > 0.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
    }
    
    private void _HitFloor() 
    {
        int layerMask = LayerMask.GetMask("Floor");
        Vector3 rayPos = transform.position - new Vector3(0.0f, transform.lossyScale.y / 2.0f);
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.1f, 0.1f);
        RaycastHit2D rayHIt = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.down, layerMask);

        if (rayHIt.transform == null)
        {
            _bFloor = false;
            _anime.SetBool("Idle", _bFloor);
            return;
        }  else if (rayHIt.transform.tag == "Floor" && !_bFloor)
        {
            _bFloor = true;
            _anime.SetBool("Idle", true);
        }

    }

    public void PlayerDamage(Player player)
    {
        player.Damage(_attackPower);
    }
}