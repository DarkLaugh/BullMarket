using BullMarket.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BullMarket.Infrastructure.Services
{
    public class TimerService : ITimerService
    {
        private Timer _timer;
        private Task _action;
        private DateTime TimerStarted { get; set; }

        public TimerService()
        {
        }

        public void Execute(Task action)
        {
            _action = action;
            _timer = new Timer(DoWork, new AutoResetEvent(false), 1000, 2000);
            TimerStarted = DateTime.Now;

            if ((DateTime.Now - TimerStarted).Minutes > 60)
            {
                _timer.Dispose();
            }
        }

        private void DoWork(object state)
        {
            _action.Wait();
        }
    }
}
