using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class FlockingFitness : IFitnessFunction
    {
        bool[] reachedPOI = new bool[7];
        bool reachedGoal;
        Point2D avgLoc;
        int inFormation;
        double formHeight;



        public IFitnessFunction copy()
        {
            return new FlockingFitness();
        }

        public FlockingFitness()
        {
            //Initialize variables
            avgLoc = null;
            reachedGoal = false;
            inFormation = 0;
            formHeight = 0.0;
        }
        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Fitness Awarded for flock reaching goal point in formation"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            objectives = new double[6];
            double trackingFitness = 0.0f;

            if (avgLoc == null) return 1;
            /*
            for (int i = 0; i < reachedPOI.Length; i++)
            {
                if (reachedPOI[i])
                    trackingFitness += 1.0f;
                else
                {
                    double dist = avgLoc.distance(new Point2D((int)environment.POIPosition[i].X, (int)environment.POIPosition[i].Y));
                    trackingFitness += ((1.0f - (dist / environment.maxDistance)) * 0.5);
                    break;
                }
            }*/
            if (reachedGoal)
            {
                trackingFitness = 10.0f;
            }
            else
            {
                double dist = avgLoc.distance(environment.goal_point);
                trackingFitness += ((1.0f - (dist / environment.maxDistance)) * 0.5);
            }
            objectives[0] = trackingFitness;
            objectives[1] = inFormation;
            if (formHeight == 0.0) formHeight = 0.00001;

            return trackingFitness*2 + inFormation*.35 + (10.0/formHeight);
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            if (reachedGoal) return;

            double rangeMin = 2.5;
            double rangeMax = 3.5;

            double sumx = 0;
            double sumy = 0;
            int sum = 0;
            inFormation = 0;

            double maxY = 0.0;
            double minY = 2000.0;

            foreach (Robot r in ip.robots)
            {
                if (!r.autopilot)
                {
                    double radius = r.radius;
                    Point2D loc = r.location;
                    sumx += loc.x;
                    sumy += loc.y;
                    sum++;

                    foreach (Robot r2 in ip.robots)
                    {
                        double dist = loc.distance(r2.location);
                        if ((dist >= radius * rangeMin) && (dist <= radius * rangeMax))
                            inFormation++;
                    }

                    if (loc.y > maxY) maxY = loc.y;
                    if (loc.y < minY) minY = loc.y;
                }
            }

            formHeight = maxY - minY;
            inFormation /= 2;

            double avgX = sumx / (double)sum;
            double avgY = sumy / (double)sum;
            avgLoc = new Point2D(avgX, avgY);

            if (avgLoc.distance(environment.goal_point) < 20.0f)
                reachedGoal = true;

            for (int i = 0; i < environment.POIPosition.Count; i++)
            {
                if (avgLoc.distance(new Point2D((int)environment.POIPosition[i].X, (int)environment.POIPosition[i].Y)) < 20.0f)
                    reachedPOI[i] = true;
            }

        }

        void IFitnessFunction.reset()
        {
            avgLoc = null;
            for (int i = 0; i < reachedPOI.Length; i++)
                reachedPOI[i] = false;
            reachedGoal = false;
            inFormation = 0;
            formHeight = 0.0;
        }

        #endregion
    }
}
