using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class PreyRobot : Robot
    {

        public PreyRobot()
        {
            name = "PreyRobot";
            autopilot = true;
        }

        public override int defaultSensorDensity()
        {
            return 11;
        }

        public override float defaultRobotSize()
        {
            return 10f;
        }

        public override void onCollision()
        {
            base.onCollision();
        }

        public override void populateSensors(int size)
        {
            sensors = new List<ISensor>();
            double maxRange = 250;

            if (size == 1)
            {
                sensors.Add(new PreySensor(0, this, maxRange, this.sensorNoise));
                return;
            }

            double startAngle = -Math.PI / 2.0;
            double delta = Math.PI / (size - 1);

            for (int i = 0; i < size; i++)
            {
                sensors.Add(new PreySensor(startAngle + (delta * i), this, maxRange, this.sensorNoise));
            }
        }

        public void decideAction(float[] outputs, double timeStep)
        {
            if (autopilot)
            {
                doAutopilot(timeStep);
                return;
            }

            if (stopped) 
            {
                velocity = 0;
                return;
            }

            velocity = outputs[1] * 40;
            heading += (outputs[2] - outputs[0]) * .05;
            collide_last = false;
        }

        private void doAutopilot(double timestep)
        {
            velocity = 40;
        }

        public override void networkResults(float[] outputs)
        {
            double distortion;
            if (effectorNoise > 0)
            {
                for (int k = 0; k < outputs.Length; k++)
                {

                    distortion = 1.0 + (EngineUtilities.random.NextDouble() - 0.5) * 2.0 * (effectorNoise) / 100.0;
                    outputs[k] *= (float)distortion;
                    outputs[k] = (float)EngineUtilities.clamp(outputs[k], 0, 1);
                }
            }
            decideAction(outputs, timeStep);
            updatePosition();
        }

        public override void doAction()
        {
            float[] inputs = new float[sensors.Count];

            for (int i = 0; i < sensors.Count; i++)
            {
                inputs[i] = (float)sensors[i].get_value();
            }

            agentBrain.setInputSignals(id, inputs);

        }
    }
}
