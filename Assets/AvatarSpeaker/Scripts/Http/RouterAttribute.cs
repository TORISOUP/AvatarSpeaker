using UnityEngine.Scripting;

namespace AvatarSpeaker.Http
{
    public abstract class RouterAttribute : PreserveAttribute
    {
        public string LocalPath { get; }

        protected RouterAttribute(string localPath)
        {
            this.LocalPath = localPath;
        }
    }

    public class Get : RouterAttribute
    {
        public Get(string localPath) : base(localPath)
        {
        }
    }

    public class Post : RouterAttribute
    {
        public Post(string localPath) : base(localPath)
        {
        }
    }

    public class Put : RouterAttribute
    {
        public Put(string localPath) : base(localPath)
        {
        }
    }

    public class Delete : RouterAttribute
    {
        public Delete(string localPath) : base(localPath)
        {
        }
    }
}