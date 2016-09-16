using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grabber2.Infrastructure.Components;

namespace Grabber2.Infrastructure.Services.Configuration
{
    public class ConfigurationService
    {
        private readonly ConcurrentDictionary<Guid, ConfigurationModel> _configurations = new ConcurrentDictionary<Guid, ConfigurationModel>();

        public ConfigurationService()
        {
            //temp
            _configurations.TryAdd(new Guid("3B5C3E28-DC1B-4033-9B69-64F82DF77CFC"), new ConfigurationModel { AutoStart = true, AutoRestart = true});
            _configurations.TryAdd(new Guid("037049F7-41A3-47F0-ADDE-F99F1D55AFD2"), new ConfigurationModel { AutoStart = true });
            _configurations.TryAdd(new Guid("CB8E34B4-D13A-4D64-AC16-FB329D86334B"), new ConfigurationModel { AutoStart = true });
            Refresh();
            Run();
        }

        public void Refresh()
        {
            //initial config load from db, creation if not exists
            Task.Delay(1000).Wait(); 
        }

        private void Run()
        {
            //TODO: pereodical config reloader
            //while Refresh();
        }
        public ConfigurationModel GetConfiguration(IServerComponent component)
        {
            ConfigurationModel c;
            _configurations.TryGetValue(component.GetId(), out c);
            return c;
        }
    }
}
