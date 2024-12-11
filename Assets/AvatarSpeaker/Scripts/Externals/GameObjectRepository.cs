using System;
using System.Collections.Generic;
using UnityEngine;


namespace AvatarSpeaker.Scripts.Externals
{
    /// <summary>
    /// UnityのGameObjectを管理するクラス
    /// </summary>
    public sealed class GameObjectRepository : IDisposable
    {
        private readonly Dictionary<string, GameObject> _gameObjects = new();

        public void Register(string key, GameObject gameObject)
        {
            _gameObjects[key] = gameObject;
        }

        public GameObject Get(string key)
        {
            return _gameObjects[key];
        }

        public void Remove(string key)
        {
            _gameObjects.Remove(key);
        }

        public void Dispose()
        {
            _gameObjects.Clear();
        }
    }
}