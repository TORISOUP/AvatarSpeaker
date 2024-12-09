using System;
using R3;
using UnityEngine;

namespace AvatarSpeaker.Core
{
    public sealed class RoomSpace : IDisposable
    {
        public ReadOnlyReactiveProperty<Color> BackgroundColor => _backgroundColor;
        private readonly ReactiveProperty<Color> _backgroundColor;

        public ReadOnlyReactiveProperty<Speaker> CurrentSpeaker => _currentSpeaker;
        private readonly ReactiveProperty<Speaker> _currentSpeaker = new();
        
        public RoomSpace()
        {
            _backgroundColor = new ReactiveProperty<Color>(Color.white);
        }

        public void Dispose()
        {
            _backgroundColor.Dispose();
            _currentSpeaker.Dispose();
        }
    }
}