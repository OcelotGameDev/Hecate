using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTransition
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; } = null;

        [SerializeField] private int _firstSceneIndex;

        private void Awake()
        {
            //Guard Clause
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnEnable()
        {
            LoadFirstScene();
        }

        private void LoadFirstScene()
        {
            SceneManager.LoadScene(_firstSceneIndex, LoadSceneMode.Additive);
            
            
        }
    }
}