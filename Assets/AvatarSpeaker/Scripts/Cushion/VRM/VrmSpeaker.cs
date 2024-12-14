using UniVRM10;

namespace AvatarSpeaker.Cushion.VRM
{
    /// <summary>
    /// VRM10に依存したSpeaker
    /// </summary>
    public abstract class VrmSpeaker : GameObjectSpeaker
    {
        public abstract Vrm10Instance Vrm10Instance { get; }
    }
}