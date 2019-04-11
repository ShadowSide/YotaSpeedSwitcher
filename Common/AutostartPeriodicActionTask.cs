using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AutostartPeriodicActionTask
    {
        public AutostartPeriodicActionTask(Action process, TimeSpan period, bool waitAndPublicFirstIterationException = false)
        {
            if (process is null)
                throw new ArgumentNullException(nameof(process));
            _process = process;
            _period = period;
            if (waitAndPublicFirstIterationException)
            {
                _timer = Task.Factory
                            .StartNew(Process)
                            .ContinueWith(ReturnFirstIterationResult)
                            .ContinueWith(Loop, TaskContinuationOptions.NotOnFaulted);
                _firstIterationResult.Task.Wait();
            }
            else
                Task.Factory
                    .StartNew(() => { })
                    .ContinueWith(Loop);
            _firstIterationResult.Task.Dispose();
        }

        private async void Loop(Task lastItertion)
        {
            await Task.Delay(_period)
                .ContinueWith(ProcessNoThrow)
                .ContinueWith(Loop);
        }

        private void ReturnFirstIterationResult(Task firstIterationResult)
        {
            try
            {
                firstIterationResult.Wait();
            }
            catch
            {
                _firstIterationResult.SetException(firstIterationResult.Exception);
            }
            firstIterationResult.Dispose();
        }

        private void ProcessNoThrow(Task dummy)
        {
            try
            {
                Process();
            }
            catch { }
        }

        private void Process()
        {
            _process();
        }

        private readonly TaskCompletionSource<object> _firstIterationResult = new TaskCompletionSource<object>();
        private readonly TimeSpan _period;
        private readonly Task _timer;
        private readonly Action _process;
    }
}
