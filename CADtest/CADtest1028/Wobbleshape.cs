using CAD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAD
{
    [Serializable]
    class WobbleShape : BaseShape
    {
        public override bool catchShape(Point testPoint)
        {
            throw new NotImplementedException();
        }

        public override BaseShape copySelf()
        {
            throw new NotImplementedException();
        }

        public override void draw(Graphics g)
        {
            throw new NotImplementedException();
        }

        public override Point[] getAllHitPoint()
        {
            throw new NotImplementedException();
        }

        public override void setHitPoint(int hitPointIndex, Point newPoint)
        {
            throw new NotImplementedException();
        }
    }
}
