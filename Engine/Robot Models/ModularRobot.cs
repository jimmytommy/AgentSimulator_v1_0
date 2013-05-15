using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class ModularRobot : Robot
    {
        public ModularRobot nextBot;
        public bool firstBot;
        public double range;

        private double maxSpeed = 40.0;

        public ModularRobot()
        {
            name = "ModularRobot";
            autopilot = true;
        }

        public void initBot(ModularRobot botBehind, double range)
        {
            this.nextBot = botBehind;
            this.firstBot = false;
            this.range = range;
        }

        public void initFirstBot(ModularRobot botAhead, double range)
        {
            this.nextBot = botAhead;
            this.firstBot = true;
            this.range = range;
        }

        public override int defaultSensorDensity()
        {
            return 1;
        }

        public override float defaultRobotSize()
        {
            return 10f;
        }

        public void setConnectionRange(double range)
        {
            this.range = range;
        }

        public override void onCollision()
        {
 	        base.onCollision();
        }

        public override void populateSensors(int size)
        {
            sensors = new List<ISensor>();

            sensors.Add(new TimeSensor(this, this.sensorNoise, this.timeStep));

            //potentially add sensors for the angle of botBehind?
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

            heading = (outputs[1] - outputs[0]);
            velocity = heading * maxSpeed;
         
            collide_last = false;
        }

        private void doAutopilot(double timestep)
        {
            velocity = 0;
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

        public override void updatePosition()
        {
            //record old coordinates
            temp_dist = (float)old_location.distance_sq(location);
            dist_trav+=temp_dist;

            old_location.x = location.x;
            old_location.y = location.y;

            //update current coordinates (may be revoked if new position forces collison
            //robot may move up or down, but must maintain constant distance to robot behind
            if (!stopped)
            {
                //double nextX = nextBot.location.x;
                //double nextY = nextBot.location.y;
                double dist = velocity * timeStep;

                //double deltaX;
                //double deltaY;

                double theta = (dist / range);
                location.rotate(theta, nextBot.location);

                double di = location.distance(nextBot.location);
                while (di > (range+.5))
                {
                    double dx = location.x - nextBot.location.x;
                    double dy = location.y - nextBot.location.y;
                    double c = di - range;

                    double newX = location.x - dx * (c / di);
                    double newY = location.y - dy * (c / di);

                    location.x = newX;
                    location.y = newY;

                    di = location.distance(nextBot.location);
                }
               /* if (firstBot)
                {
                    deltaX = range - (range * Math.Cos(dist / (range * -1.0)));
                    deltaY = range * Math.Sin(dist / (range * -1.0));
                }
                else
                {
                    deltaX = range - (range * Math.Sin(dist / range));
                    deltaY = range * Math.Sin(dist / range);
                }

                location.x += deltaX;
                location.y += deltaY; */
            }
            history.Add(new Point2D(location));
        }

        public override void draw(System.Drawing.Graphics g, CoordinateFrame frame)
        {
            Point upperleft = frame.to_display((float)(circle.p.x - radius), (float)(circle.p.y - radius));
            int size = (int)((radius * 2) / frame.scale);
            Rectangle r = new Rectangle(upperleft.X, upperleft.Y, size, size);
            if (disabled)
                g.DrawEllipse(EngineUtilities.YellowPen, r);
            else if (collide_last)
                g.DrawEllipse(EngineUtilities.RedPen, r);
            else if (corrected)
                g.DrawEllipse(EngineUtilities.YellowPen, r);
            else
                g.DrawEllipse(EngineUtilities.BluePen, r);

            Point loc = frame.to_display((float)circle.p.x, (float)circle.p.y);
            Point nextLoc = frame.to_display((float)nextBot.circle.p.x, (float)nextBot.circle.p.y);

            if (!firstBot)
                g.DrawLine(EngineUtilities.BluePen, loc, nextLoc);

            int sensCount = 0;
            foreach (ISensor sensor in sensors)
            {
                sensor.draw(g, frame);
            }

            if (display_debug)
                foreach (ISensor sensor in sensors)
                {
                    if (draw_sensors)
                    {
                        double val = sensor.get_value();
                        if (val < 0.0) val = 0.0;
                        if (val > 1.0) val = 1.0;
                        Color col = Color.FromArgb((int)(val * 255), 0, 0); //(int)(val*255),(int)(val*255));
                        SolidBrush newpen = new SolidBrush(col);
                        g.FillRectangle(newpen, sensCount * 40, 500 + 30 * id, 40, 30);
                        sensCount += 1;
                    }
                }
        }

    }
}
