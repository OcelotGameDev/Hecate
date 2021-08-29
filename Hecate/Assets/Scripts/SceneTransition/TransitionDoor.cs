using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SceneTransition
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TransitionDoor : MonoBehaviour
    {
        [SerializeField]
        private float _playerTravelDistance = 0;
    
    #if UNITY_EDITOR
        public float PlayerTravelDistance
        {
            get => _playerTravelDistance;
            set => _playerTravelDistance = value;
        }
    #else
        public float PlayerTravelDistance => _playerDestiny;
    #endif
        

        public event Action<TransitionDoor, Transform> OnPlayerEnterDoor;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Debug.LogWarning("Player Entrou np trigger", this.gameObject);
            OnPlayerEnterDoor?.Invoke(this, other.transform);
        }

        private void OnValidate()
        {
            this.gameObject.layer = LayerMask.NameToLayer("Door");
        }
    }
        
#if UNITY_EDITOR
    [CustomEditor(typeof(TransitionDoor)), CanEditMultipleObjects]
    public class TransitionDoorEditor : Editor
    {
        private void OnSceneGUI()
        {
            TransitionDoor door = (TransitionDoor)target;
            
            EditorGUI.BeginChangeCheck();

            var transform = door.transform;
            
            float size = HandleUtility.GetHandleSize(transform.position) * 0.5f;
            Vector2 right = transform.right;
            
            Handles.color = Color.green;
            // Vector3 newPlayerDestiny = Handles.Slider2D(door.transform.TransformPoint(door.PlayerDestiny),
            //     Vector3.forward, Vector3.right, Vector3.up, size / 5f, Handles.DotHandleCap, 0.1f);

            Vector3 newPlayerDestiny = Handles.Slider((Vector2)transform.position + (right * door.PlayerTravelDistance),
                door.transform.TransformDirection(right), size / 5f, Handles.DotHandleCap, 0.1f);

            
            // Handles.color = new Color(0, 1, 0, 0.2f);
            // Handles.DrawSolidDisc(newPlayerDestiny, Vector3.forward, size);
            
            newPlayerDestiny = door.transform.InverseTransformPoint(newPlayerDestiny);
            
            Handles.color = Color.white;
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(door, "Change Player Destiny");
                door.PlayerTravelDistance = Vector2.Distance(door.transform.position, newPlayerDestiny);
            }
        }
    }
#endif
}