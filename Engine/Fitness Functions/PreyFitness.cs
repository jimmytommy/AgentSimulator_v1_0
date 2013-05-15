using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class PreyFitness : IFitnessFunction
    {

        int numPrey;
        int preyCaught;
        double finishTime;
        bool finished;
        //double traveled;

        public IFitnessFunction copy()
        {
            return new PreyFitness();
        }

        public PreyFitness()
        {
            finished = false;
        }
        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Fitness Awarded for speed and number of Prey caught"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            objectives = new double[6];
            double caughtFitness = preyCaught * 100.0;
            objectives[0] = caughtFitness;

            double timeFitness = 0;
            if (finished)
                timeFitness = (double)engine.evaluationTime - finishTime;
            if (timeFitness < 0) timeFitness = 0.0;
            objectives[1] = timeFitness;

            //double travelFitness = traveled * .0002;

            return caughtFitness + timeFitness * 2;  // +travelFitness;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            if (finished) return;

            preyCaught = 0;
            numPrey = environment.preys.Count;
            foreach (Prey p in environment.preys)
            {
                if (p.caught) preyCaught++;
            }

            if (preyCaught == numPrey)
            {
                finished = true;
                finishTime = ip.elapsed;
            }

          /*  traveled = 0;
            foreach (Robot r in ip.robots)
            {
                traveled += r.dist_trav;
            }*/
        }

        void IFitnessFunction.reset()
        {
            preyCaught = 0;
            finishTime = 0;
            finished = false;
            //traveled = 0;
        }

        #endregion
    }
}