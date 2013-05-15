using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class ModularFitness : IFitnessFunction
    {
        double latDist;

        public IFitnessFunction copy()
        {
            return new ModularFitness();
        }

        public ModularFitness()
        {
            latDist = 0.0;
        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Fitness Awarded for cumulative lateral movement"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            objectives = null;

            if (latDist < 0) return 0;

            return latDist;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            foreach (Robot r in ip.robots)
            {
                double dx = r.location.x - r.old_location.x;
                latDist += dx;
            }
        }

        void IFitnessFunction.reset()
        {
            latDist = 0.0;
        }

        #endregion
    }
}
