using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace SceneTransition
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] 
        private int _thisSceneIndex;
        [SerializeField] 
        private int[] _adjacentSceneIndexes;
        
        private TransitionDoor[] _doors;

        [SerializeField] 
        private CinemachineVirtualCamera _camera;

        private static RoomManager _currentRoom = null;

        private int[] AdjacentSceneIndexes => _adjacentSceneIndexes;

        private void Awake()
        {
            _doors = this.gameObject.GetComponentsInChildren<TransitionDoor>();
        }

        private void OnEnable()
        {
            if (_currentRoom == null)
            {
                _currentRoom = this;
                StartCoroutine(LoadAdjacentRooms(this));
                this.RoomEnter();
            }

            foreach (var door in _doors)
            {
                door.OnPlayerEnterDoor += PlayerSwitchedRooms;
            }    
        }

        private void OnDisable()
        {
            foreach (var door in _doors)
            {
                door.OnPlayerEnterDoor -= PlayerSwitchedRooms;
            }    
        }

        private void PlayerSwitchedRooms(TransitionDoor door, Transform player)
        {
            if (_doors.Contains(door) && _currentRoom != this)
            {
                StartCoroutine(OnChangeRooms(door, player));
            }
        }

        private void RoomEnter()
        {
            // foreach (var door in _doors)
            //     door.gameObject.SetActive(false);
            
            _camera.Priority = 11;
            // Activate Spawners
        }

        private void RoomExit()
        {
            // foreach (var door in _doors)
            //     door.gameObject.SetActive(false);
            
            _camera.Priority = 10;
            // Destroy/Deactivate Monsters
            // Reset Scene
        }

        private IEnumerator OnChangeRooms(TransitionDoor door, Transform player)
        {
            Debug.LogWarning("Player entered a room", this.gameObject);

            Time.timeScale = 0f;

            // Move Player 

            // player.position = door.transform.position;
            // Tween playerMove = player.DOMove(door.PlayerDestinyGlobal, 0.5f)
            //     .SetAutoKill(false).SetEase(Ease.Linear).SetUpdate(true);
            
            Tween playerMove = player.DOMove(GetPlayerEndPosition(door, player), 0.5f)
                .SetAutoKill(false).SetEase(Ease.OutQuad).SetUpdate(true);

            
            _currentRoom.RoomExit();
            this.RoomEnter();

            yield return WaitUnloadAdjacentRooms(_currentRoom, this);
            
            _currentRoom.ResetScene();
            
            yield return LoadAdjacentRooms(_currentRoom, this);

            _currentRoom = this;

            //Unpause if (player Ended Move, Current Unloaded, Adjacent Loaded)
            while (!playerMove.IsComplete())
            {
                yield return null;
            }

            Time.timeScale = 1f;
        }

        private void ResetScene()
        {
            
        }

        private void ReleaseScene()
        {
            // foreach (var door in _doors)
            //     door.gameObject.SetActive(true);
        }
        
        private static IEnumerator LoadAdjacentRooms(RoomManager current)
        {
            var loadOperations = new List<AsyncOperation>();

            var indexesToLoad = current.AdjacentSceneIndexes;

            for (var i = 0; i < indexesToLoad.Length; i++)
            {
                loadOperations.Add(
                    SceneManager.LoadSceneAsync(indexesToLoad[i],
                        LoadSceneMode.Additive));
                loadOperations[i].allowSceneActivation = true;
            }

            yield return null;
        }

        private static IEnumerator LoadAdjacentRooms(RoomManager current, RoomManager next)
        {
            var loadOperations = new List<AsyncOperation>();

            var indexesToLoad = next.AdjacentSceneIndexes.Except(current.AdjacentSceneIndexes).ToArray();
            indexesToLoad = indexesToLoad.Except(new int[] { current._thisSceneIndex }).ToArray();
            
            for (var i = 0; i < indexesToLoad.Length; i++)
            {
                loadOperations.Add(
                    SceneManager.LoadSceneAsync(indexesToLoad[i],
                        LoadSceneMode.Additive));
                loadOperations[i].allowSceneActivation = true;
            }

            yield return null;
        }

        private static IEnumerator WaitUnloadAdjacentRooms(RoomManager current, RoomManager next)
        {
            var unloadOperations = new List<AsyncOperation>();

            var indexesToUnload = current.AdjacentSceneIndexes.Except(next.AdjacentSceneIndexes).ToArray();
            indexesToUnload = indexesToUnload.Except(new int[] { next._thisSceneIndex }).ToArray();
            
            foreach (var index in indexesToUnload)
            {
                unloadOperations.Add(SceneManager.UnloadSceneAsync(index, 
                    UnloadSceneOptions.UnloadAllEmbeddedSceneObjects));
            }

            yield return new WaitUntil(() => unloadOperations.All(op => op.isDone));
        }

        private void OnValidate()
        {
            _thisSceneIndex = this.gameObject.scene.buildIndex;
        }

        private static Vector2 GetPlayerEndPosition(TransitionDoor door, Transform player)
        {
            Vector2 playerLocalPosition = door.transform.InverseTransformPoint(player.position);

            Vector2 projection = Vector3.Project(playerLocalPosition,  door.transform.InverseTransformDirection(door.transform.up));

            return (Vector2)door.transform.TransformPoint(projection + new Vector2(door.PlayerTravelDistance, 0)) ;
        }
    }
}

