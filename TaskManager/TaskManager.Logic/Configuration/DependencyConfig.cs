namespace TaskManager.Logic.Configuration
{
    public static class DependencyConfig
    {
        public static void Configure()
        {
            DAL.Configuration.DependencyConfig.Configure();
        }
    }
}
