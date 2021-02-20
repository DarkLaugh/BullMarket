using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BullMarket.Application.Interfaces.Services
{
    public interface ITimerService
    {
        public void Execute(Task action);
    }
}
