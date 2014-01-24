using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevatorSimulator.PhysicalDomain
{
    class ShaftData
    {
        private readonly int _topFloor;
        private readonly int _bottomFloor;
        private readonly int _interfloorDistance;

        public int TopFloor { get { return _topFloor; } }
        public int BottomFloor { get { return _bottomFloor; } }
        public int InterfloorDistance { get { return _interfloorDistance; } }

        public ShaftData(int topFloor, int bottomFloor, int interfloor)
        {
            _topFloor = topFloor;
            _bottomFloor = bottomFloor;
            _interfloorDistance = interfloor;
        }
    }
}
