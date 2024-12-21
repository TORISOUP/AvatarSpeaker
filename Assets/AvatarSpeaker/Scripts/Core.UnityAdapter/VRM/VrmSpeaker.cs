using UniVRM10;

namespace AvatarSpeaker.Core.UnityAdapter.VRM
{
    /// <summary>
    /// VRM10に依存したSpeaker
    /// </summary>
    public abstract class VrmSpeaker : GameObjectSpeaker
    {
        public abstract Vrm10Instance Vrm10Instance { get; }
    }
}