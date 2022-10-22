namespace Platform
{
    public class PlatformManager
    {
        public AchievementsDelegate Achievements { get; }
        public InputDelegate Input { get; }
        public FileSystemDelegate FileSystem { get; }
        public GameplayDelegate Gameplay { get; }

        public PlatformManager()
        {
            // Register Steam delegates
            Achievements = new SteamAchievementsDelegate();
            Input = new InputDelegate();
            FileSystem = new FileSystemDelegate();
            Gameplay = new GameplayDelegate();
        }
    }
}