namespace Api.Config
{
    public class CustomEnvironmentVariablesConfigurationSource : IConfigurationSource
    {
        public string Prefix { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CustomEnvironmentVariablesConfigurationProvider(Prefix);
        }
    }

}
