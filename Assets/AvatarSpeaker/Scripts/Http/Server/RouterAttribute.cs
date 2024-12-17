using UnityEngine.Scripting;

namespace AvatarSpeaker.Http.Server
{
    public abstract class RouterAttribute : PreserveAttribute
    {
        protected RouterAttribute(string localPath)
        {
            LocalPath = localPath;
        }

        public string LocalPath { get; }
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