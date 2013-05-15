using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class PreySensor : ISensor
    {
        public Robot owner;
        public bool seePrey;
        public double angle;
        public double distance;
        public double maxRange;
        public double noise;

        public PreySensor(double angle, Robot owner, double maxRange, double noise)
        {
            this.angle = angle;
            this.owner = owner;
            this.maxRange = maxRange;
            this.noise = noise/100.0;

            this.seePrey = false;
            this.distance = maxRange;
        }

        public double get_value()
        {
            return distance / maxRange;
        }

        public double get_value_raw()
        {
            return distance;
        }

        public void update(Environment env, List<Robot> robots, CollisionManager cm)
        {
            distance = maxRange;
            seePrey = false;
            Point2D casted = new Point2D(owner.location);
            casted.x += Math.Cos(angle + owner.heading) * maxRange;
            casted.y += Math.Sin(angle + owner.heading) * maxRange;

            Line2D cast = new Line2D(owner.location, casted);

            foreach (Prey prey in env.preys)
            {
                bool found = false;

                double newDistance = cast.nearest_intersection(prey.circle, out found);

                if (found)
                {
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        seePrey = true;
                    }
                }
            }
        }

        public void draw(Graphics g, CoordinateFrame frame)
        {
            Point a = frame.to_display((float)(owner.location.x), (float)(owner.location.y));
            Point b = frame.to_display((float)(owner.location.x + Math.Cos(angle + owner.heading) * distance),
                                       (float)(owner.location.y + Math.Sin(angle + owner.heading) * distance));
            g.DrawLine(EngineUtilities.GreendPen, a, b);
        }


    }
}
