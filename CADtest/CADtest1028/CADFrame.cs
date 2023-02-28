using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CAD;
using System.Drawing.Drawing2D;

namespace CADtest1028
{
    public partial class CADFrame : Form
    {
        private CAD.BaseTool currentTool = null;//当前的应用工具
        private ArrayList currentShapes = null;//当前显示的图形集合
        private Hashtable registerToolMap = null;//当前注册的工具集合
        private ArrayList historyShapes = null;//历史图形的快照集合

        public Color clr = Color.Green; // 默认
        public int lineWidth = 1;

        public const string LINETOOL_REGISTERNAME = "LINETOOL_REGISTERNAME";
        public const string HANDTOOL_REGISTERNAME = "HANDTOOL_REGISTERNAME";
        public const string RECTANGLET0OL_REGISTERNAME = "RECTANGLET0OL_REGISTERNAME";
        public const string CIRCLETOOL_REGISTERNAME = "CIRCLETOOL_REGISTERNAME";

        private Matrix transform = new Matrix();//该类由System.Drawing.Drawing2D支持
        private float m_dZoomscale = 1.0f;
        public const float s_dScrollValue = 0.1f;


        public CADFrame()
        {
            InitializeComponent();
            timer1.Start();
            currentShapes = new ArrayList();//实例当前显示的图形集合对象
            registerToolMap = new Hashtable();//注册工具的集合对象
            historyShapes = new ArrayList();//历史图形的快照集合对象
            this.registerTool(LINETOOL_REGISTERNAME, new LineTool());//注册线工具
            this.registerTool(HANDTOOL_REGISTERNAME, new HandTool());//注册抓取工具
            this.registerTool(RECTANGLET0OL_REGISTERNAME, new RectangleTool());//注册矩形工具
            this.registerTool(CIRCLETOOL_REGISTERNAME, new CircleTool());//注册圆形工具
            this.record();
        }
        public ArrayList getCurrentShapes()//泛型
        {
            return currentShapes;
        }

        //set当前显示的图形集合对象
        public void setCurrentShapes(ArrayList currentShapes)
        {
            this.currentShapes = currentShapes; //左为当前类的成员，右为方法的参数
        }

        public CAD.BaseTool getCurrentTool()
        {
            return currentTool;
        }

        public void setCurrentTool(BaseTool currentTool)
        {
            this.currentTool = currentTool;
        }

        public Hashtable getRegisterToolMap()
        {
            return registerToolMap;
        }

        public void setRegisterToolMap(Hashtable registerToolMap)
        {
            this.registerToolMap = registerToolMap;
        }

        public ArrayList getHistoryShape()
        {
            return historyShapes;
        }

        private void setHistoryShapes(ArrayList historyShapes)//用于撤销
        {
            this.historyShapes = historyShapes;
        }

        private void registerTool(string registerName, BaseTool registerTool)//构造函数中注册工具
        {
            this.getRegisterToolMap().Add(registerName, registerTool);
            registerTool.setRefCADPanel(this);
        }

        private void useTool(string registerName)//应用工具
        {
            if (this.getCurrentTool() != null) this.getCurrentTool().unSet();//卸载之前的工具
            BaseTool setTool = (BaseTool)this.getRegisterToolMap()[registerName];//加载现在要用的工具
            if (setTool != null)
            {
                setTool.set();
                this.setCurrentTool(setTool);//装载工具
            }
        }

        int undoIndex = 0;//回退的索引
        public void record()//快照保存的方法
        {
            if (undoIndex > 0)//当有回退时，清空回退获得快照
            {
                while (undoIndex != 0)
                {
                    this.getHistoryShape().RemoveAt(this.getHistoryShape().Count - 1);
                    undoIndex--;
                }
            }
            this.getHistoryShape().Add(this.cloneShapArray(this.getCurrentShapes()));//保存快照
        }

        public void redo()//重做
        {
            if (undoIndex > 0)//当有回退时才可重做
            {
                undoIndex--;//将历史快照取回到当前图形中
                this.setCurrentShapes(this.cloneShapArray((ArrayList)this.getHistoryShape()[this.getHistoryShape().Count - 1 - undoIndex]));
            }
            this.Refresh();
        }

        public void undo()//回退
        {
            if ((this.getHistoryShape().Count - 1 - undoIndex) > 0)//历史快照中还有历史才能回退
            {
                undoIndex++;//将历史快照取回到当前图形中
                this.setCurrentShapes((this.cloneShapArray((ArrayList)this.getHistoryShape()[this.getHistoryShape().Count - 1 - undoIndex])));
            }
            this.Refresh();
        }

        public ArrayList cloneShapArray(ArrayList shapeArrayList)//图形集合身复制
        {
            ArrayList returnShapeArrayList = new ArrayList();
            for (int i = 0; i < shapeArrayList.Count; i++)
            {
                returnShapeArrayList.Add(((BaseShape)shapeArrayList[i]).copySelf());
            }
            return returnShapeArrayList;
        }

        //delete的函数定义
        public void clear()
        {
            undoIndex = 0;
            this.setHistoryShapes(new ArrayList());
            this.setCurrentShapes(new ArrayList());
            this.record();
            this.pictureBox1.Refresh();
        }

        //Line的按键定义
        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.useTool(LINETOOL_REGISTERNAME);
        }

        //选中Hand的按键定义
        private void handToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.useTool(HANDTOOL_REGISTERNAME);
        }

        //Rec的按键定义
        private void recToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.useTool(RECTANGLET0OL_REGISTERNAME);
        }

        //Cir的按键定义
        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.useTool(CIRCLETOOL_REGISTERNAME);
        }

        //撤销的按键定义
        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undo();
        }

        //重做的按键定义
        private void 重做ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            redo();
        }

        //delete按键定义
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clear();
        }

        //函数save的定义
        public void save(string filePath) // 保存成二进制
        {
            Stream s = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter b = new BinaryFormatter();
            for (int i = 0; i < this.getCurrentShapes().Count; i++)
            {
                b.Serialize(s, this.getCurrentShapes()[i]);
            }

            s.Close();
        }

        //函数load的函数定义
        public void load(string filePath)
        {
            Stream s = File.Open(filePath, FileMode.Open, FileAccess.Read);
            BinaryFormatter c = new BinaryFormatter();
            ArrayList newShapes = new ArrayList();
            bool forFlat = true;
            for (int i = 0; forFlat; i++)
            {
                try
                {
                    newShapes.Add(c.Deserialize(s));
                }
                catch
                {
                    forFlat = false;
                }
            }
            s.Close();
            this.setCurrentShapes(newShapes);
            this.setHistoryShapes(new ArrayList());
            this.record();
            undoIndex = 0;
            this.pictureBox1.Refresh();
        }

        //新建文件的按键定义
        private void btnNew_Click(object sender, EventArgs e)
        {
            clear();
        }

        //保存文件的按键定义
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                save(saveFileDialog1.FileName);
            }
        }

        //打开文件的按键定义
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                load(openFileDialog1.FileName);
            }
        }

        //退出文件的按键定义
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        //画图事件
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Transform = transform;
            for (int i = 0; i < currentShapes.Count; i++)
            {

                string Type = ((BaseShape)currentShapes[i]).GetType().ToString();
                switch (Type)
                {
                    case "CAD.LineShape":
                        g.DrawLine(new Pen(((BaseShape)currentShapes[i]).penColor, ((BaseShape)currentShapes[i]).penwidth), ((BaseShape)currentShapes[i]).getP1(), ((BaseShape)currentShapes[i]).getP2());
                        break;
                    case "CAD.RectangleShape":
                        g.DrawRectangle(new Pen(((BaseShape)currentShapes[i]).penColor, ((BaseShape)currentShapes[i]).penwidth), ((BaseShape)currentShapes[i]).getP1().X, ((BaseShape)currentShapes[i]).getP1().Y, (((BaseShape)currentShapes[i]).getP2().X - ((BaseShape)currentShapes[i]).getP1().X), (((BaseShape)currentShapes[i]).getP2().Y - ((BaseShape)currentShapes[i]).getP1().Y));
                        g.DrawRectangle(new Pen(((BaseShape)currentShapes[i]).penColor, ((BaseShape)currentShapes[i]).penwidth), ((BaseShape)currentShapes[i]).getP1().X, ((BaseShape)currentShapes[i]).getP2().Y, (((BaseShape)currentShapes[i]).getP2().X - ((BaseShape)currentShapes[i]).getP1().X), (((BaseShape)currentShapes[i]).getP1().Y - ((BaseShape)currentShapes[i]).getP2().Y));
                        g.DrawRectangle(new Pen(((BaseShape)currentShapes[i]).penColor, ((BaseShape)currentShapes[i]).penwidth), ((BaseShape)currentShapes[i]).getP2().X, ((BaseShape)currentShapes[i]).getP2().Y, (((BaseShape)currentShapes[i]).getP1().X - ((BaseShape)currentShapes[i]).getP2().X), (((BaseShape)currentShapes[i]).getP1().Y - ((BaseShape)currentShapes[i]).getP2().Y));
                        g.DrawRectangle(new Pen(((BaseShape)currentShapes[i]).penColor, ((BaseShape)currentShapes[i]).penwidth), ((BaseShape)currentShapes[i]).getP2().X, ((BaseShape)currentShapes[i]).getP1().Y, (((BaseShape)currentShapes[i]).getP1().X - ((BaseShape)currentShapes[i]).getP2().X), (((BaseShape)currentShapes[i]).getP2().Y - ((BaseShape)currentShapes[i]).getP1().Y));
                        break;
                    case "CAD.CircleShape":
                        int r = (int)Math.Pow(Math.Pow(((BaseShape)currentShapes[i]).getP2().X - ((BaseShape)currentShapes[i]).getP1().X, 2) + Math.Pow(((BaseShape)currentShapes[i]).getP2().Y - ((BaseShape)currentShapes[i]).getP1().Y, 2), 0.5);
                        g.DrawEllipse(new Pen(((BaseShape)currentShapes[i]).penColor, ((BaseShape)currentShapes[i]).penwidth), ((BaseShape)currentShapes[i]).getP1().X - r, ((BaseShape)currentShapes[i]).getP1().Y - r, 2 * r, 2 * r);
                        break;
                }
                ((BaseShape)currentShapes[i]).superDraw(g);
            }
        }

        //鼠标状态初始化
        bool isMouseDown = false;

        //鼠标按下事件
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            if (this.getCurrentTool() != null) this.getCurrentTool().superMouseDown(sender, e, this);
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            }
        }

        //鼠标移动事件
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                if (this.getCurrentTool() != null) this.getCurrentTool().superMouseDrag(sender, e);
            }
            else
            {
                if (this.getCurrentTool() != null) this.getCurrentTool().superMouseMove(sender, e);
            }
        }

        //鼠标松开事件
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            if (this.getCurrentTool() != null) this.getCurrentTool().superMouseUp(sender, e);
        }



        //在Form设计器中双击Form1窗体。这将会放置一个Form1_Load()窗体加载函数,在你的代码文件中。
        private void Form1_Load(object sender, EventArgs e)
        {

        }


        //颜色选择Red、Yellow、Green、Blue、Purple
        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clr = Color.Red;
        }
        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clr = Color.Yellow;
        }
        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clr = Color.Green;
        }
        private void buleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clr = Color.Blue;
        }
        private void purpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clr = Color.Purple;
        }
        //自选more颜色
        private void moreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                clr = colorDialog.Color;
            }
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
            pictureBox1.Focus();
            if (pictureBox1.Focused == true && e.Delta != 0)
            {
                // 将以鼠标位置为中心，映射到客户端PictureBox坐标系统
                Point pictureBoxPoint = pictureBox1.PointToClient(this.PointToScreen(e.Location));
                ZoomScroll(pictureBoxPoint, e.Delta > 0);
            }
        }

        private void ZoomScroll(Point location, bool zoomIn)
        {
            // 算出新的秤是多少。确保比例因子保持在之间1% and 1000%
            float newScale = Math.Min(Math.Max(m_dZoomscale + (zoomIn ? s_dScrollValue : -s_dScrollValue), 0.1f), 10);
            if (newScale != m_dZoomscale)
            {
                float adjust = newScale / m_dZoomscale;
                m_dZoomscale = newScale;
                // Translate mouse point to origin
                transform.Translate(-location.X, -location.Y, MatrixOrder.Append);
                // Scale view
                transform.Scale(adjust, adjust, MatrixOrder.Append);
                // Translate origin back to original mouse point.
                transform.Translate(location.X, location.Y, MatrixOrder.Append);
                pictureBox1.Invalidate();
            }
        }

        private void 镜像ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Graphics g = e.Graphics;
            ////文字镜像
            //var state = g.Save();
            ////未镜像前
            //string text = $"镜像测试";
            //var font = new Font("黑体", 30, FontStyle.Regular);
            //g.DrawString(text, font, Brushes.Blue, new PointF(X, Y));
            ////X轴镜像
            //g.TranslateTransform(X, Y);
            //g.ScaleTransform(1, -1);
            //g.DrawString(text, font, Brushes.SkyBlue, new PointF(0, 0));
            //g.Restore(state);
            ////Y轴镜像
            //state = g.Save();
            //g.TranslateTransform(X, Y);
            //g.ScaleTransform(-1, 1);
            //g.DrawString(text, font, Brushes.Red, new PointF(0, 0));
            //g.Restore(state);
        }

        private void CADFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否关闭当前系统？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != dr)
            {
                e.Cancel = true;
            }
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(this.textBox1,"这是一个文本框");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //显示系统时间
            this.toolStripStatusLabel1.Text= "时间: " + DateTime.Now.ToString().Replace(" ", " ");
            //显示坐标
            this.toolStripStatusLabel2.Text = MousePosition.X + "," + MousePosition.Y;
        }
    }
}