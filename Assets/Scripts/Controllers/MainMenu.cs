using System;
using Cinemachine;
using Cinemachine.Utility;
using UnityEngine;

namespace Controllers
{
    public class MainMenu : MonoBehaviour
    {
        private const float MenuOrbitHeight = 2.0f;
        private static readonly Vector3 MenuPos = new Vector3(-2.0f, 1.0f, -24.0f);
        
        [SerializeField] private GameObject loadingScreenPrefab;
        [SerializeField] private AudioSource menuMusic;
        [SerializeField] private CinemachineFreeLook freeLook;

        enum MenuState
        {
            Initialising,
            InMenu,
            StartingGame,
            Playing,
            OpeningMenu
        }
        
        private MenuState _menuState = MenuState.Initialising;
        private float _startOrbitHeight;
        private Vector3 _startPos;


        private void Start()
        {
            _startPos = freeLook.Follow.position;
            _startOrbitHeight = freeLook.m_Orbits[1].m_Height;

            freeLook.Follow.position = MenuPos;
            freeLook.m_Orbits[1].m_Height = MenuOrbitHeight;
        }

        private void Update()
        {
            switch (_menuState)
            {
                case MenuState.Initialising:
                    _menuState = MenuState.InMenu;
                    break;
                case MenuState.InMenu:
                    break;
                case MenuState.StartingGame:
                    StartGame();
                    break;
                case MenuState.Playing:
                    break;
                case MenuState.OpeningMenu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartGame()
        {
            freeLook.Follow.position = Vector3.Lerp(
                freeLook.Follow.position, _startPos, Time.deltaTime + 0.01f);
            freeLook.m_Orbits[1].m_Height = Mathf.Lerp(
                freeLook.m_Orbits[1].m_Height, _startOrbitHeight, Time.deltaTime);
            if ((_startPos - freeLook.Follow.position).magnitude < UnityVectorExtensions.Epsilon + 1)
            {
                _menuState = MenuState.Playing;
            }
        }

        // private void Start()
        // {
        //     // if (!isMainMenuScene) return;
        //     // // Fade in music
        //     // StartCoroutine(Jukebox.Instance.FadeTo(
        //     //     Jukebox.MusicVolume, Jukebox.FullVolume, 3f));
        //     // StartCoroutine(Jukebox.DelayCall(1f, ()=>menuMusic.Play()));
        //     //
        //     // var loadingScreen = Instantiate(loadingScreenPrefab).GetComponent<LoadingScreen>();
        //     // loadingScreen.LoadMain();
        //     // StartCoroutine(Jukebox.Instance.FadeTo(
        //     //     Jukebox.MusicVolume, Jukebox.LowestVolume, 1f));
        //     // StartCoroutine(Jukebox.DelayCall(2f, ()=>menuMusic.Stop()));
        // }

        public void Play()
        {
            _menuState = MenuState.StartingGame;
        }
    }
}
