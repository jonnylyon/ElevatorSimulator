using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElevatorSimulator.AbstractDomain;

namespace ElevatorSimulator.Tools
{
    static class CarMotionMaths
    {
        public static void CalculateDecisionPointSpeedAndTime(CarAttributes attributes, double floorDist, out double nextFloorDecisionPointSpeed, out  double nextFloorDecisionPointTime)
        {

            // First, using linear acc'n and dec'n and assuming no maximum speed, work out the parameters
            // of the point Q at which we must finally decide whether or not to stop at the next floor
            // (the decision point of the next floor)

            double Q_D; // The distance to point Q from here
            double Q_V; // The speed that this car will have reached by point Q
            double Q_T; // The time taken to get to point Q

            Q_D = (attributes.Deceleration * floorDist) / (attributes.Acceleration + attributes.Deceleration);
            Q_V = Math.Sqrt(2 * attributes.Acceleration * Q_D);
            Q_T = (1 / attributes.Acceleration) * Q_V;

            // If Q_V is less than or equal to the maximum speed, then Q is the decision point for whether or
            // not to stop at the next floor.
            // Otherwise, we must calculate the decision point R by working out the point P at which we reach
            // our maximum speed when accelerating from our current position.

            if (Q_V > attributes.MaxSpeed)
            {
                // note that P_V would be equal to the max speed of the car
                double P_D; // The distance to point P from here
                double P_T; // The time taken to get to point P

                P_T = attributes.MaxSpeed / attributes.Acceleration;
                P_D = 0.5 * attributes.Acceleration * Math.Pow(P_T, 2);
                // note that R_V = P_V = the max speed of the car
                double R_D; // The distance to point R from here
                double R_T; // The time taken to get to point R from here

                R_D = floorDist - (Math.Pow(attributes.MaxSpeed, 2) / (2 * attributes.Deceleration));
                R_T = P_T + ((R_D - P_D) / attributes.MaxSpeed);

                nextFloorDecisionPointSpeed = attributes.MaxSpeed;
                nextFloorDecisionPointTime = R_T;
            }
            else
            {
                nextFloorDecisionPointSpeed = Q_V;
                nextFloorDecisionPointTime = Q_T;
            }
        }

        public static double StoppingTime(double initialSpeed, double deceleration)
        {
            return initialSpeed / deceleration;
        }

        public static void CalculateDecisionPointSpeedAndTimev2(CarAttributes attributes, CarState state, double floorDist, out double nextFloorDecisionPointSpeed, out  double nextFloorDecisionPointTime)
        {
            // First, using linear acc'n and dec'n and assuming no maximum speed, work out the parameters
            // of the point Q at which we must finally decide whether or not to stop at the next floor
            // (the decision point of the next floor)

            double Q_D; // The distance to point Q from here
            double Q_V; // The speed that this car will have reached by point Q
            double Q_T; // The time taken to get to point Q

            Q_D = ((2 * attributes.Deceleration * floorDist) - Math.Pow(state.InitialSpeed, 2)) / (2 * (attributes.Acceleration + attributes.Deceleration));
            Q_V = Math.Sqrt(Math.Pow(state.InitialSpeed, 2) + (2 * attributes.Acceleration * Q_D));
            Q_T = (1 / attributes.Acceleration) * (Q_V - state.InitialSpeed);

            // If Q_V is less than or equal to the maximum speed, then Q is the decision point for whether or
            // not to stop at the next floor.
            // Otherwise, we must calculate the decision point R by working out the point P at which we reach
            // our maximum speed when accelerating from our current position.

            if (Q_V > attributes.MaxSpeed)
            {
                // note that P_V would be equal to the max speed of the car
                double P_D; // The distance to point P from here
                double P_T; // The time taken to get to point P

                P_T = (attributes.MaxSpeed - state.InitialSpeed) / attributes.Acceleration;
                P_D = state.InitialSpeed + (0.5 * attributes.Acceleration * Math.Pow(P_T, 2));

                // note that R_V = P_V = the max speed of the car
                double R_D; // The distance to point R from here
                double R_T; // The time taken to get to point R from here

                R_D = floorDist - (Math.Pow(attributes.MaxSpeed, 2) / (2 * attributes.Deceleration));
                R_T = P_T + ((R_D - P_D) / attributes.MaxSpeed);

                nextFloorDecisionPointSpeed = attributes.MaxSpeed;
                nextFloorDecisionPointTime = R_T;
            }
            else
            {
                nextFloorDecisionPointSpeed = Q_V;
                nextFloorDecisionPointTime = Q_T;
            }
        }
    }
}
