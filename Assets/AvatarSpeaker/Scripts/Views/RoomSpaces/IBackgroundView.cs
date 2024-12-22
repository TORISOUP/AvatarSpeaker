using System;
using UnityEngine;

namespace AvatarSpeaker.Views.RoomSpaces
{
    /// <summary>
    /// 背景を表示するView
    /// Cameraで実装する方法やuGUI実装などのバリエーションが考えられる
    /// </summary>
    public interface IBackgroundView : IDisposable
    {
        GameObject Root { get; }
    }
}