using System;
using System.Threading;
using AvatarSpeaker.Core.Models;
using AvatarSpeaker.Core.UnityAdapter.VRM;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace AvatarSpeaker.Views
{
    /// <summary>
    /// VrmSpeakerを表示するView
    /// </summary>
    public sealed class VrmSpeakerView : MonoBehaviour
    {
        private RuntimeAnimatorController _runtimeAnimatorController;
        private VrmSpeaker _vrmSpeaker;

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
            _vrmSpeaker.CurrentIdlePose
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