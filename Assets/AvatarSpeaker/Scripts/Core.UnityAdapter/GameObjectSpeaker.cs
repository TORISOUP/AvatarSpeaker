using AvatarSpeaker.Core;
using UnityEngine;

namespace AvatarSpeaker.Cushion
{
    /// <summary>
    /// UnityのGameObjectに依存したSpeaker
    /// </summary>
    public abstract class GameObjectSpeaker : Speaker
    {
        public abstract GameObject GameObject { get; }
    }
}