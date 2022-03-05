namespace Platform
{
    public class PlatformManager
    {
        public static readonly PlatformManager Instance = new PlatformManager();
        public AchievementsDelegate Achievements { get; }
        public InputDelegate Input { get; }
        public FileSystemDelegate FileSystem { get; }

        private PlatformManager()
        {
        }
    }
}