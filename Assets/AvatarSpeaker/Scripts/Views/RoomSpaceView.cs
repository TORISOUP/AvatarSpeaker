using System;
using AvatarSpeaker.Core;
using UnityEngine;

namespace AvatarSpeaker.Views
{
    /// <summary>
    /// RoomSpaceに紐づくView
    /// </summary>
    public sealed class RoomSpaceView : MonoBehaviour
    {
        private IBackgroundView _backgroundView;
        private RoomSpace _roomSpace;
        private SpeakerCameraView _speakerCameraView;
        public GameObject Root { get; private set; }


        private void OnDestroy()
        {
            if (_speakerCameraView != null) Destroy(_speakerCameraView.gameObject);

            _backgroundView.Dispose();
        }

        public void Initalize(RoomSpace roomSpace, IBackgroundView backgroundView, SpeakerCameraView speakerCameraView)
        {
            _roomSpace = roomSpace;

            // SpeakerCameraViewを登録
            _speakerCameraView = speakerCameraView;
            _speakerCameraView.gameObject.transform.SetParent(Root.transform);

            _backgroundView = backgroundView;
            _backgroundView.Root.transform.SetParent(Root.transform);
        }

        public static RoomSpaceView Create()
        {
            var root = new GameObject("RoomSpaceView");
            var view = root.AddComponent<RoomSpaceView>();
            view.Root = root;
            return view;
        }
    }

    public interface IBackgroundView : IDisposable
    {
        GameObject Root { get; }
    }
}