using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal.Interfaces
{
    public interface IValueStateTracker<TValue>
    {
        TValue CurrentValue { get; }
        void ChangeValue(TValue newValue);
    }
}
