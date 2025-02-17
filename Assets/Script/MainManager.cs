using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    [SerializeField, Header("ゲームオーバー")] private GameObject _gameOverUI;
    [SerializeField, Header("ゴール")] private GameObject _GoalUI;

    private GameObject _player;
    private bool _bShowUI;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>().gameObject;
        _bShowUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        _ShowGameOverUI();
    }
    public void _ShowGameOverUI()
    {
        if (_player != null) return; //体力が0になったらプレイヤーのオブジェクトがDestroy消えるから
        _gameOverUI.SetActive(true);
        _bShowUI = true;
    }

    public void _ShowGoalUI()
    {
        _GoalUI.SetActive(true);
        _bShowUI = true;
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        if (!_bShowUI || !context.performed) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //指定したシーンに移動
    }
}
