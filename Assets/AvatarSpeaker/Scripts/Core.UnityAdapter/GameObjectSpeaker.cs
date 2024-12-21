using UnityEngine;

namespace AvatarSpeaker.Core.UnityAdapter
{
    /// <summary>
    /// UnityのGameObjectに依存したSpeaker
    /// </summary>
    public abstract class GameObjectSpeaker : Speaker
    {
        public abstract GameObject GameObject { get; }
    }
}