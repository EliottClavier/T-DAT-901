using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System.Globalization;

namespace Api.Config
{
    public class CustomEnvironmentVariablesConfigurationProvider : EnvironmentVariablesConfigurationProvider
    {
        public CustomEnvironmentVariablesConfigurationProvider(string prefix = null) : base(prefix)
        {
        }

        public override void Load()
        {
            base.Load();

            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var pair in Data)
            {
                var key = ConvertKey(pair.Key);
                data[key] = pair.Value;
            }

            Data = data;
        }

        private string ConvertKey(string key)
        {
            var segments = key.Split(new[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = ConvertToPascalCase(segments[i]);
            }
            return string.Join(':', segments);
        }

        private string ConvertToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var words = input.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(word => word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower())
                             .ToArray();

            return string.Join("", words);
        }
    }



}
