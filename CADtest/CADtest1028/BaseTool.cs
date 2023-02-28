using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CAD
{
    public abstract class BaseTool
    {
        private BaseShape operShape = null;//操作图形
        private CADtest1028.CADFrame refCADPanel = null;//关联画板

        private Point upPoint = new Point();//鼠标弹起点
        private Point downPoint = new Point();//鼠标按下点
        private Point newMovePoint = new Point();//新的鼠标移动点
        private Point oldMovePoint = new Point();//老的鼠标移动点
        private Point newDragPoint = new Point();//新的鼠标拖动点
        private Point oldDragPoint = new Point();//老的鼠标拖动点

        public Point getDownPoint()
        {
            return downPoint;
        }
        public void setDownPoint(Point downPoint)
        {
            this.downPoint = downPoint;
        }
        public Point getDragPoint()
        {
            return newDragPoint;
        }
        public void setNewDragPoint(Point newDragPoint)
        {
            this.newDragPoint = newDragPoint;
        }
        public Point getNewMovePoint()
        {
            return newMovePoint;
        }
        public void setNewMovePoint(Point newMovePoint) //新的鼠标移动点的设定【捕捉时用到】
        {
            this.newMovePoint = newMovePoint;
        }
        public Point getOldDragPoint()
        {
            return oldDragPoint;
        }
        public void setOldDragPoint(Point oldDragPoint)
        {
            this.oldDragPoint = oldDragPoint;
        }
        public Point getOldMovePoint()
        {
            return oldMovePoint;
        }
        public void setOldMovePoint(Point oldMovePoint)
        {
            this.oldMovePoint = oldMovePoint;
        }
        public Point getUpPoint()
        {
            return upPoint;
        }
        public void setUpPoint(Point upPoint)
        {
            this.upPoint = upPoint;
        }
        //***********************************************************//
        public CADtest1028.CADFrame getRefCADPanel()
        {
            return refCADPanel;
        }
        public void setRefCADPanel(CADtest1028.CADFrame refCADPanel) //在CADFrame面板构造函数逐个赋值
        {
            this.refCADPanel = refCADPanel;
        }
        public BaseShape getOperShape() // 鼠标拖动调用
        {
            return operShape;
        }
        public void setOperShape(BaseShape operShape) //在各工具中用
        {
            this.operShape = operShape;
        }

        public abstract void mouseUp(object sender, MouseEventArgs e);//鼠标弹起的处理
        public abstract void mouseDown(object sender, MouseEventArgs e, CADtest1028.CADFrame objC);//鼠标按下的处理
        public abstract void mouseMove(object sender, MouseEventArgs e);//鼠标移动的处理
        public abstract void mouseDrag(object sender, MouseEventArgs e);//鼠标拖动的处理

        public void superMouseUp(object sender, MouseEventArgs e)//鼠标释放
        {
            //this.setUpPoint(new Point(e.X, e.Y));//鼠标的弹起点的设定
            this.mouseUp(sender, e);//鼠标的弹起的设定
            //this.setUpPoint(new Point());//鼠标弹起点的设定
            //this.setDownPoint(new Point());//鼠标按下点的设定
            //this.setOldMovePoint(new Point());//老的鼠标移动点的设定
            //this.setNewMovePoint(new Point());//新的鼠标移动点的设定
            //this.setOldDragPoint(new Point());//老的鼠标拖动点的设定
            //this.setNewDragPoint(new Point());//新的鼠标拖动点归零
            this.getRefCADPanel().record();//保存，用来返回和前进
        }

        public void superMouseDown(object sender, MouseEventArgs e, CADtest1028.CADFrame objCAD)//鼠标按下
        {
            this.setUpPoint(new Point(e.X, e.Y));//鼠标的弹起点的设定
            this.setDownPoint(new Point(e.X, e.Y));//鼠标按下点的设定
            this.setOldMovePoint(new Point(e.X, e.Y));//老的鼠标移动点的设定
            this.setNewMovePoint(new Point(e.X, e.Y));//新的鼠标移动点的设定
            this.setOldDragPoint(new Point(e.X, e.Y));//老的鼠标拖动点的设定
            this.setNewDragPoint(new Point(e.X, e.Y));//新的鼠标拖动点的设定
            this.mouseDown(sender, e, objCAD);//鼠标按下的处理
        }

        public void superMouseMove(object sender, MouseEventArgs e)//捕捉热点，特别热点
        {
            this.setNewMovePoint(new Point(e.X, e.Y));//新的鼠标移动点的设定
            this.mouseMove(sender, e);//鼠标移动
            this.setOldMovePoint(this.getNewMovePoint());//老的鼠标移动点的设定
        }

        public void superMouseDrag(object sender, MouseEventArgs e)//鼠标拖动
        {
            this.setNewDragPoint(new Point(e.X, e.Y));//新的鼠标拖动点的设定
            this.mouseDrag(sender, e);//鼠标拖动
            this.setOldDragPoint(this.getDragPoint());//老的鼠标拖动点的设定
        }

        public abstract void set();//装载
        public abstract void unSet();//卸载
    }
}
