using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField, Header("移動速度")] private float _moveSpeed;
    [SerializeField, Header("ジャンプ速度")] private float _jumpSpeed;
    [SerializeField, Header("体力")] private int _hp;
    [SerializeField, Header("無敵時間")] private float _damageTime;
    [SerializeField, Header("点滅時間")] private float _flashTime;

    private Vector2 _inputDirection;
    private Rigidbody2D _rigid;
    private bool _bjump; //bool型はtrue or false
    private Animator _anime;
    private SpriteRenderer _spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _anime = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _bjump = false;
    }

    // Update is called once per frame
    void Update()
    {
        _Move();
        //Debug.Log(_hp);
        _LookMoveDirec();
        //_HitFloor();
    }
    
    private void _Move()
    {
        //if (_bjump) return;
        _rigid.velocity = new Vector2(_inputDirection.x * _moveSpeed, _rigid.velocity.y); //x座標入力方向に移動速度分移動
        _anime.SetBool("Walk", Mathf.Abs(_inputDirection.x) > 0.01f); //横キーの入力があるかどうかで切り替え
    }

    private void _LookMoveDirec()
    {
        if (_inputDirection.x > 0.0f)
        {
            transform.eulerAngles = Vector3.zero;
        } else if (_inputDirection.x < 0.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "Floor")
    //     {
    //         _bjump = false; //床に当たったらまたジャンプできる
    //         _anime.SetBool("Jump", _bjump);
    //     }
    //
    //     if (collision.gameObject.tag == "Enemy")
    //     {
    //         _HitEnemy(collision.gameObject);
    //         //gameObject.layer = LayerMask.NameToLayer("PlayerDamage"); //敵に衝突したらこのレイヤーに切り替え
    //     }
    // }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _bjump = false;
            _anime.SetBool("Jump", _bjump);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            _HitEnemy(collision);
        }

        if (collision.gameObject.tag == "Goal")
        {
            FindObjectOfType<MainManager>()._ShowGoalUI();
            this.enabled = false;
            GetComponent<Player>().enabled = false;
        }
    }


    private void _HitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor");
        Vector3 rayPos = transform.position - new Vector3(0.0f, transform.lossyScale.y / 2.0f); //playerの足元の座標
        Vector3 raySize = new Vector3(transform.lossyScale.x - 0.1f, 0.1f);
        RaycastHit2D rayHit = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.down, 0.0f, layerMask);
        
        if (rayHit.transform == null) //playerがなにとも接触していないときつまり空中にいるとき
        {
            _bjump = true;
            _anime.SetBool("Jump", _bjump);
            return;
        }

        if (rayHit.transform.tag == "Floor" && _bjump)
        {
            _bjump = false;
            _anime.SetBool("Jump", _bjump);
        }
    }

    // private void _HitEnemy(GameObject enemy)
    // {
    //     ContactPoint2D[] contacts = new ContactPoint2D[1];
    //     int contactCount = enemy.GetComponent<Collider2D>().GetContacts(contacts);
    //
    //     if (contactCount > 0 && contacts[0].point.y < transform.position.y) // 敵の衝突点がプレイヤーより下なら
    //     {
    //         Destroy(enemy);
    //         _rigid.AddForce(Vector2.up * _jumpSpeed, ForceMode2D.Impulse); //最初加速してだんだんゆっくり
    //     }
    //     else
    //     {
    //         enemy.GetComponent<Enemy>().PlayerDamage(this); //thisで自分のクラスを引数に入れる
    //         gameObject.layer = LayerMask.NameToLayer("PlayerDamage"); //敵に衝突したらこのレイヤーに切り替え
    //         StartCoroutine(_Damage());
    //     }
    // }
    
    private void _HitEnemy(Collision2D collision)
    {
        GameObject enemy = collision.gameObject;
        ContactPoint2D contact = collision.contacts[0]; // 最初の接触点を取得

        // **敵の衝突点がプレイヤーより下なら踏みつけ成功**
        if (contact.point.y < transform.position.y)
        {
            Debug.Log("Enemy Defeated");
            Destroy(enemy);
            _rigid.AddForce(Vector2.up * _jumpSpeed, ForceMode2D.Impulse);
        }
        else
        {
            Debug.Log("Player Damaged");
            enemy.GetComponent<Enemy>().PlayerDamage(this);
            gameObject.layer = LayerMask.NameToLayer("PlayerDamage");
            StartCoroutine(_Damage());
        }
    }

    IEnumerator _Damage()　//コルーチン
    {
        Color color = _spriteRenderer.color;
        for (int i = 0; i < _damageTime; i++)
        {
            yield return new WaitForSeconds(_flashTime);
            _spriteRenderer.color = new Color(color.r, color.g, color.b, 0.0f); //α値を0にすることで透明になって消える
            
            yield return new WaitForSeconds(_flashTime);
            _spriteRenderer.color = new Color(color.r, color.g, color.b, 1.0f);
            
        }

        _spriteRenderer.color = color;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private void _Dead()
    {
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Camera camera = Camera.main;

        //Debug.Log($"Camera Y: {camera.transform.position.y}, Player Y: {transform.position.y}");

        if (camera.transform.position.y > transform.position.y)
        {
            Destroy(gameObject);
        }
    }


    public void _OnMove(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>(); //入力情報をVector2型に変換
    }

    public void _OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || _bjump) return;
        _rigid.AddForce(Vector2.up * _jumpSpeed, ForceMode2D.Impulse); //上方向の加速処理
        _bjump = true;
        _anime.SetBool("Jump", _bjump);
    }

    public void Damage(int damage)
    {
        _hp = Mathf.Max(_hp - damage, 0); //0未満にならないように処理
        _Dead();
    }

    public int GetHP()
    {
        return _hp;
    }
}
