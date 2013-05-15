using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    //A prey object moves in the opposite direction of Robots
    public class Prey : SimulatorObject
    {

        public Circle2D circle;
        public bool caught;
        public bool colored;

        public double range;
        public double speed;
        Circle2D start;

        public void update(List<Robot> rbts, double timestep)
        {
            if (start == null) start = new Circle2D(circle);

            Robot closest = null;
            double closestDist = range;
            foreach (Robot r in rbts)
            {
                double dist = circle.p.distance(r.location);
                //Console.WriteLine("dist: " + dist);
                if (dist <= closestDist)
                {
                    closestDist = dist;
                    closest = r;
                }
            }

            if (closestDist <= (circle.radius + rbts[0].radius))
            {
                caught = true;
                dynamic = false;
                return;
            }

            if (closest != null)
            {
                double dA = circle.p.x - closest.location.x;
                double dB = circle.p.y - closest.location.y;
                double c = closest.location.distance(circle.p);

                double z = speed * timestep;
                double dX = (dA * z) / c;
                double dY = (dB * z) / c;

                Point2D newP = new Point2D(dX + circle.p.x, dY + circle.p.y);
                circle.p = newP;
            }
        }

        public override void update()
        {
        }
        public override void undo()
        {
        }

        public Prey()
        {
        }

        public Prey(Prey k)
        {
            name = k.name;
            colored = false;

            circle = new Circle2D(k.circle);

            caught = k.caught;
            location = circle.p;

            range = k.range;
            speed = k.speed;

            dynamic = !caught;
            visible = k.visible;
        }

        public Prey(double x, double y, double rad, double range, double speed, bool vis, string n)
        {
            this.name = n;
            this.colored = false;
            this.dynamic = true;
            this.caught = false;
            this.visible = vis;
            this.speed = speed;
            this.range = range;

            Point2D p = new Point2D(x, y);
            this.circle = new Circle2D(p, rad);
            this.location = circle.p;
        }

        public void draw(Graphics g, CoordinateFrame frame)
        {
            Point upperleft = frame.to_display((float)(circle.p.x - circle.radius), (float)(circle.p.y - circle.radius));
            int size = (int)((circle.radius * 2) / frame.scale);
            Rectangle r = new Rectangle(upperleft.X, upperleft.Y, size, size);
            if (caught)
                g.DrawEllipse(EngineUtilities.BluePen, r);
            else if (visible)
                g.DrawEllipse(EngineUtilities.RedPen, r);
            else
                g.DrawEllipse(EngineUtilities.GreendPen, r);
        }

        public void reset()
        {
            if (start == null) return;
            circle = new Circle2D(start);
            caught = false;
            dynamic = true;
        }
    }
}
