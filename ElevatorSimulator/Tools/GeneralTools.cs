using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Tools
{
    static class GeneralTools
    {
        public static Direction getDirectionFromHereToThere(int here, int there)
        {
            if (there > here)
            {
                return Direction.Up;
            }

            if (there < here)
            {
                return Direction.Down;
            }

            return Direction.None;
        }

        public static List<int> getRange(int? lower, int? higher)
        {
            if (!lower.HasValue || !higher.HasValue || lower > higher)
            {
                return new List<int>();
            }

            return Enumerable.Range(lower.Value, higher.Value + 1 - lower.Value).ToList<int>();
        }

        public static int? max(int? a, int? b)
        {
            if (a.HasValue)
            {
                if (b.HasValue)
                {
                    return Math.Max(a.Value, b.Value);
                }
                else
                {
                    return a;
                }
            }
            else
            {
                if (b.HasValue)
                {
                    return b;
                }
                else
                {
                    return null;
                }
            }
        }

        public static int? min(int? a, int? b)
        {
            if (a.HasValue)
            {
                if (b.HasValue)
                {
                    return Math.Min(a.Value, b.Value);
                }
                else
                {
                    return a;
                }
            }
            else
            {
                if (b.HasValue)
                {
                    return b;
                }
                else
                {
                    return null;
                }
            }
        }

        public static int? notNullOption(int? preferred, int? fallback)
        {
            if (!object.ReferenceEquals(preferred, null))
            {
                return preferred;
            }

            return fallback;
        }
    }
}
