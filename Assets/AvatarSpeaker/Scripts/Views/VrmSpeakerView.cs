using System;
using System.Threading;
using AvatarSpeaker.Core.Models;
using AvatarSpeaker.Cushion.VRM;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

namespace AvatarSpeaker.Scripts.Views
{
    public sealed class VrmSpeakerView : MonoBehaviour
    {
        private VrmSpeaker _vrmSpeaker;
        private RuntimeAnimatorController _runtimeAnimatorController;

        public void SetVrmSpeaker(VrmSpeaker vrmSpeaker, RuntimeAnimatorController runtimeAnimatorController)
        {
            _vrmSpeaker = vrmSpeaker;
            _runtimeAnimatorController = runtimeAnimatorController;
            // ヒエラルキー上の位置を調整
            _vrmSpeaker.GameObject.transform.SetParent(transform);
            WaitForDisposeAsync(destroyCancellationToken).Forget();

            SetUp();
        }

        private void SetUp()
        {
            var animator = _vrmSpeaker.GameObject.GetComponent<Animator>();
            animator.runtimeAnimatorController = _runtimeAnimatorController;

            // IdlePoseの変更をAnimatorに反映
            _vrmSpeaker.IdlePose
                .Subscribe(idlePose => animator.SetTrigger(ToTrigger(idlePose)))
                .AddTo(this);
        }

        private async UniTaskVoid WaitForDisposeAsync(CancellationToken ct)
        {
            await _vrmSpeaker.OnDisposeAsync.AttachExternalCancellation(ct);
            Destroy(gameObject);
        }

        private static string ToTrigger(IdlePose idlePose)
        {
            return idlePose switch
            {
                IdlePose.Pose1 => "Pose1",
                IdlePose.Pose2 => "Pose2",
                IdlePose.Pose3 => "Pose3",
                IdlePose.Pose4 => "Pose4",
                IdlePose.Pose5 => "Pose5",
                _ => throw new ArgumentOutOfRangeException(nameof(idlePose), idlePose, null)
            };
        }
    }
}