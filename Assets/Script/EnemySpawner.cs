using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, Header("敵オブジェクト")] private GameObject _enemy;

    private Player _player;
    private GameObject _enemyObj;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
        _enemyObj = null;
    }

    // Update is called once per frame
    void Update()
    {
        _SpawnEnemy();
    }

    private void _SpawnEnemy()
    {
        if (_player == null) return;

        Vector3 playerPos = _player.transform.position;
        Vector3 cameraMaxPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)); //ゲーム画面の右上座標
        Vector3 scale = _enemy.transform.lossyScale;

        float distance = Vector2.Distance(transform.position, new Vector2(playerPos.x, transform.position.y));　//高さはEnemyの値をとることで横軸のみの距離を計算
        float spawnDis = Vector2.Distance(playerPos, new Vector2(cameraMaxPos.x + scale.x / 2.0f, playerPos.y));

        if (distance <= spawnDis && _enemyObj == null)
        {
            _enemyObj = Instantiate(_enemy);
            _enemyObj.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            transform.parent = _enemyObj.transform;
        }
    }
}
