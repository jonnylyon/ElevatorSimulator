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
        private readonly double _interfloorDistance;
        private readonly int _shaftId;

        public int TopFloor { get { return _topFloor; } }
        public int BottomFloor { get { return _bottomFloor; } }
        public double InterfloorDistance { get { return _interfloorDistance; } }
        public int ShaftId { get { return _shaftId; } }

        public ShaftData(int topFloor, int bottomFloor, double interfloor, int shaftId)
        {
            _topFloor = topFloor;
            _bottomFloor = bottomFloor;
            _interfloorDistance = interfloor;
            _shaftId = shaftId;
        }
    }
}
