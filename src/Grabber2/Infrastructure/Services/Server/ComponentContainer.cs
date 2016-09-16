using System;
using System.Threading;
using System.Threading.Tasks;
using Grabber2.Infrastructure.Components;

namespace Grabber2.Infrastructure.Services.Server
{
    public class ComponentContainer
    {
        private Task _task;
        public readonly IServerComponent Component;
        public DateTime? StoppedAt { get; private set; }
        public AggregateException Exception { get; private set; }
        public bool IsRunning { get; private set; }

        private CancellationTokenSource _cancellationTokenSource;

        public ComponentContainer(IServerComponent component)
        {
            Component = component;
            Reset();
        }

        public void Start()
        {
            Reset();
            IsRunning = true;

            _task = new Task(() =>
            {
                Component.Start(_cancellationTokenSource.Token);

            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            _task.ContinueWith(t =>
            {
                IsRunning = false;
                StoppedAt = DateTime.Now;
                if (!t.IsCanceled)
                {
                    Exception = t.Exception;
                }
            });
            _task.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            try
            {
                _task.Wait();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Reset()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Exception = null;
            StoppedAt = null;
            IsRunning = false;
            _task = null;
        }
    }
}
