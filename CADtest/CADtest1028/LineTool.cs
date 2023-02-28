using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CAD
{
    public partial class LineTool : BaseTool
    {
        public override void mouseDown(object sender, MouseEventArgs e, CADtest1028.CADFrame objC)//重写线的鼠标按下
        {
            this.setOperShape(new LineShape());
            this.getOperShape().setP1(this.getDownPoint());
            //this.getOperShape().setP1(e.Location);
            this.getOperShape().penColor = objC.clr;
            this.getOperShape().penwidth = objC.lineWidth;
            this.getRefCADPanel().getCurrentShapes().Add(this.getOperShape());//图形添加到画板
        }

        public override void mouseDrag(object sender, MouseEventArgs e)//重写线的鼠标拖动
        {
            this.getOperShape().setP2(this.getDragPoint());// 鼠标按下的拖动点
            //this.getOperShape().setP2(e.Location);
            this.getRefCADPanel().Refresh(); //刷新窗体
        }

        public override void mouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void mouseUp(object sender, MouseEventArgs e)
        {
        }

        public override void unSet()
        {
        }

        public override void set()
        {
        }
    }
}
