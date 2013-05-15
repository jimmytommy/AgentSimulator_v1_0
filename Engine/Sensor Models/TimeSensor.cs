using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class TimeSensor : ISensor
    {
        public Robot owner;
        double time;
        double noise;
        double timestep;

        public TimeSensor(Robot owner, double noise, double timestep)
        {
            this.owner = owner;
            this.noise = noise;
            this.timestep = timestep;
        }

        public double get_value()
        {
            return time;
        }

        public double get_value_raw()
        {
            return time;
        }

        public void update(Environment env, List<Robot> robots, CollisionManager cm)
        {
            int steps = robots[0].history.Count;
            time = (double)steps * timestep;
        }

        public void draw(Graphics g, CoordinateFrame frame)
        {
            return;
        }
    }
}
