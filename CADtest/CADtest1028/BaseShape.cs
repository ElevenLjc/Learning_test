using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CAD
{
    [Serializable]// 序列化，保存和转移内存中的对象
    public abstract class BaseShape // 抽象类
    {
        private bool isSelected = false;//标识图形是否被选中

        private Point p1 = new Point(); //第一个点
        private Point p2 = new Point(); //第二个点 // 每个图形由两点确定

        public Color penColor;
        public int penwidth;

        public void setSelected()//设置为选中状态
        {
            this.isSelected = true;
        }
        public void setUnSelected()//设置为非选中状态
        {
            this.isSelected = false;
        }
        public Point getP1()
        {
            return p1;
        }
        public void setP1(Point p1)
        {
            this.p1 = p1;
        }
        public Point getP2()
        {
            return p2;
        }
        public void setP2(Point p2)
        {
            this.p2 = p2;
        }

        //抽象类用做基类
        //能被实例化
        //用途派生出其非抽象类
        //接口主要实现多重继承
        public abstract BaseShape copySelf();//不同的形状，都重写一次
        public abstract void draw(Graphics g);//画图形
        public abstract Point[] getAllHitPoint();//得到所有图形
        public abstract void setHitPoint(int hitPointIndex, Point newPoint);//设定热点
        public abstract bool catchShape(Point testPoint);//图形捕捉，鼠标是否在图形指定范围内

        public bool catchHitPoint(Point hitPoint, Point testPoint)//测试热点捕捉
        {
            return this.getHitPointRectangle(hitPoint).Contains(testPoint);
        }

        public int catchShapPoint(Point testPoint)//捕捉图形--Handtool中运用,点击变换按钮
        {
            int hitPointIndex = -1;
            Point[] allHitPoint = this.getAllHitPoint();//的到所有的热点
            for (int i = 0; i < allHitPoint.Length; i++)//循环捕捉判断
            {
                if (this.catchHitPoint(allHitPoint[i], testPoint))
                {
                    return i + 1;//如果捕捉到了热点，返回热点的索引
                }
            }
            if (this.catchShape(testPoint)) return 0;//没有捕捉到热点，捕捉到了图形，返回特别热点
            return hitPointIndex;//返回捕捉到的人点
        }

        public void drawHitPoint(Point hitPoint, Graphics g)//画热点
        {
            g.DrawRectangle(new Pen(Color.Red, 2), this.getHitPointRectangle(hitPoint));
        }

        public void drawAllHitPoint(Graphics g)//画所有热点
        {
            Point[] allHitPoint = this.getAllHitPoint();
            for (int i = 0; i < 2; i++)
            {
                this.drawHitPoint(allHitPoint[i], g);
            }
        }

        public Rectangle getHitPointRectangle(Point hitPoint)//得到热点矩形，以热点为中心高宽8像素的矩形
        {
            Rectangle rect = new Rectangle();
            rect.X = hitPoint.X - 4;
            rect.Y = hitPoint.Y - 4;
            rect.Width = 8;
            rect.Height = 8;
            return rect;
        }


        public void superDraw(Graphics g)//公共画法
        {
            if (this.isSelected) this.drawAllHitPoint(g);
        }
    }
}