using System.Threading;
using AvatarSpeaker.Cushion.VRM;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AvatarSpeaker.Scripts.Views
{
    public sealed class VrmSpeakerView : MonoBehaviour
    {
        public VrmSpeaker VrmSpeaker { get; private set; }

        public void SetVrmSpeaker(VrmSpeaker vrmSpeaker)
        {
            VrmSpeaker = vrmSpeaker;
            
            // ヒエラルキー上の位置を調整
            VrmSpeaker.GameObject.transform.SetParent(transform);
            
            WaitForDisposeAsync(destroyCancellationToken).Forget();
        }

        private async UniTaskVoid WaitForDisposeAsync(CancellationToken ct)
        {
            await VrmSpeaker.OnDisposeAsync.AttachExternalCancellation(ct);
            Destroy(gameObject);
        }
    }
}