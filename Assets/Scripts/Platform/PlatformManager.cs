namespace Platform
{
    public class PlatformManager
    {
        public static readonly PlatformManager Instance = new PlatformManager();
        public AchievementsDelegate Achievements { get; }
        public InputDelegate Input { get; }
        public FileSystemDelegate FileSystem { get; }
        public GameplayDelegate Gameplay { get; }

        private PlatformManager()
        {
            // Register Steam delegates
            Achievements = new SteamAchievementsDelegate();
            Input = new InputDelegate();
            FileSystem = new FileSystemDelegate();
            Gameplay = new GameplayDelegate();
        }
    }
}