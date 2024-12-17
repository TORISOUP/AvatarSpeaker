using System;
using UnityEngine;

namespace AvatarSpeaker.Views.RoomSpaces
{
    /// <summary>
    /// 背景を表示するView
    /// </summary>
    public interface IBackgroundView : IDisposable
    {
        GameObject Root { get; }
    }
}