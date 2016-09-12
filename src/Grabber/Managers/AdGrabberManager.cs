//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Grabber.Grabbers;
//using Grabber.Managers;
//using Infrastructure;
//using PostgreSqlProvider;
//using PostgreSqlProvider.Entities;

//namespace Grabber.Managers
//{
//    public interface IAdGrabberManager
//    {
//    }

//    public class AdGrabberManager : IAdGrabberManager
//    {
//        private readonly IAdJobsService _adJobsService;

//        private readonly Dictionary<string, GrabberEntry> _grabberEntries = new Dictionary<string, GrabberEntry>();
//        private static readonly TimeSpan CycleDelay = TimeSpan.FromSeconds(1);

//        public AdGrabberManager()
//        {
//        }

//        public void AddGrabber(string name, IAdGrabber grabber)
//        {
//            _grabberEntries.Add(name, new GrabberEntry
//            {
//                Grabber = grabber
//            });
//        }

//        public Task Run(CancellationToken cancellationToken)
//        {
//            return Task.Run(() => DoRun(cancellationToken), cancellationToken);
//        }

//        private void DoRun(CancellationToken cancellationToken)
//        {
//            Task.Delay(CycleDelay, cancellationToken).Wait(cancellationToken);
//        }

//        public class GrabberEntry
//        {
//            public IAdGrabber Grabber;
//            public bool IsEnabled = true;
//            // TODO: some type of async jobs register
//            public int JobsLimit = 1;
//        }
//    }
//}