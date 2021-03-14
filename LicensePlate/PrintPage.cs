using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicensePlate
{


    public class PrintPage
    {
        public void PrintPageTest()
        {
            PrintDataGridView.Print(new DataGridView(), "Hello", "world");
        }

    }

    /// <summary>
    /// 打印
    /// 开心懒人
    /// 2014-10-10
    /// </summary>
    public class PrintDataGridView
    {

        static DataGridView dgv;
        static string titleName = ""; //标题名称       
        static string titleName2 = ""; //第二标题名称     
        static int rowIndex = 0;   //当前行       
        static int page = 1; //当前页      
        static int rowsPerPage = 0;  //每页显示多少行
                                     /// <summary>
                                     /// 打印DataGridView
                                     /// </summary>
                                     /// <param name="dataGridView">要打印的DataGridView</param>
                                     /// <param name="title">标题</param>
                                     /// <param name="title2">第二标题,可以为null</param>
        public static void Print(DataGridView dataGridView, string title, string title2)
        {
            try
            {
                if (dataGridView == null) { return; }
                titleName = title;
                titleName2 = title2;
                dgv = dataGridView;
                PrintPreviewDialog ppvw = new PrintPreviewDialog();
                ppvw.PrintPreviewControl.Zoom = 1.0; //显示比例为100%
                PrintDocument printDoc = new PrintDocument();
                PrintDialog MyDlg = new PrintDialog();
                MyDlg.Document = printDoc;
                printDoc.DefaultPageSettings.PaperSize = new PaperSize("A4", 850, 500);
                printDoc.DefaultPageSettings.Margins = new Margins(60, 60, 60, 60); //设置边距             
                ppvw.Document = printDoc;   //设置要打印的文档               
                ((Form)ppvw).WindowState = FormWindowState.Normal; //最大化               
                rowIndex = 0; //当前行              
                page = 1;  //当前页                             
                printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage); //打印事件 
                printDoc.EndPrint += new PrintEventHandler(printDoc_EndPrint);
                ppvw.Document.DefaultPageSettings.Landscape = false;    // 设置打印为横向   
                if (!Manager.Instance.isPreviewPrint)
                {
                    printDoc.Print();   //直接打印，不预览的样式   
                }
                else
                {
                    ppvw.ShowDialog(); //打开预览
                }
                                           


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        static void printDoc_EndPrint(object sender, PrintEventArgs e)
        {
            rowIndex = 0; //当前行          
            page = 1;  //当前页            
            rowsPerPage = 0;//每页显示多少行
        }
        private static void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {

            //标题字体
            Font titleFont = new Font("微软雅黑", 16, FontStyle.Bold);
            //标题尺寸
            SizeF titleSize = e.Graphics.MeasureString(titleName, titleFont, e.MarginBounds.Width);
            //x坐标
            int x = e.MarginBounds.Left;
            //y坐标
            int y = Convert.ToInt32(e.MarginBounds.Top - titleSize.Height);
            //边距以内纸张宽度
            int pagerWidth = e.MarginBounds.Width;
            //画标题
            e.Graphics.DrawString(titleName, titleFont, Brushes.Black, x + (pagerWidth - titleSize.Width) / 2, y);
            y += (int)titleSize.Height;
            if (titleName2 != null && titleName2 != "")
            {

                //画第二标题
                e.Graphics.DrawString(titleName2, dgv.Font, Brushes.Black, x + (pagerWidth - titleSize.Width) / 2 + 200, y);
                //第二标题尺寸
                SizeF titleSize2 = e.Graphics.MeasureString(titleName2, dgv.Font, e.MarginBounds.Width);
                y += (int)titleSize2.Height;

            }

            //表头高度
            int headerHeight = 0;
            //纵轴上 内容与线的距离
            int padding = 6;
            //所有显示列的宽度
            int columnsWidth = 0;
            //计算所有显示列的宽度
            foreach (DataGridViewColumn column in dgv.Columns)
            {

                //隐藏列返回
                if (!column.Visible) continue;
                //所有显示列的宽度
                columnsWidth += column.Width;
            }

            //计算表头高度
            foreach (DataGridViewColumn column in dgv.Columns)
            {

                //列宽
                int columnWidth = (int)(Math.Floor((double)column.Width / (double)columnsWidth * (double)pagerWidth));
                //表头高度
                int temp = (int)e.Graphics.MeasureString(column.HeaderText, column.InheritedStyle.Font, columnWidth).Height + 2 * padding;
                if (temp > headerHeight) headerHeight = temp;
            }

            //画表头

            foreach (DataGridViewColumn column in dgv.Columns)
            {

                //隐藏列返回
                if (!column.Visible) continue;
                //列宽
                int columnWidth = (int)(Math.Floor((double)column.Width / (double)columnsWidth * (double)pagerWidth));
                //内容居中要加的宽度
                float cenderWidth = (columnWidth - e.Graphics.MeasureString(column.HeaderText, column.InheritedStyle.Font, columnWidth).Width) / 2;
                if (cenderWidth < 0) cenderWidth = 0;
                //内容居中要加的高度
                float cenderHeight = (headerHeight + padding - e.Graphics.MeasureString(column.HeaderText, column.InheritedStyle.Font, columnWidth).Height) / 2;
                if (cenderHeight < 0) cenderHeight = 0;
                //画背景
                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(x, y, columnWidth, headerHeight));
                //画边框
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(x, y, columnWidth, headerHeight));
                ////画上边线

                //e.Graphics.DrawLine(Pens.Black, x, y, x + columnWidth, y);

                ////画下边线

                //e.Graphics.DrawLine(Pens.Black, x, y + headerHeight, x + columnWidth, y + headerHeight);

                ////画右边线

                //e.Graphics.DrawLine(Pens.Black, x + columnWidth, y, x + columnWidth, y + headerHeight);

                //if (x == e.MarginBounds.Left)

                //{

                //    //画左边线

                //    e.Graphics.DrawLine(Pens.Black, x, y, x, y + headerHeight);

                //}

                //画内容
                e.Graphics.DrawString(column.HeaderText, column.InheritedStyle.Font, new SolidBrush(column.InheritedStyle.ForeColor), new RectangleF(x + cenderWidth, y + cenderHeight, columnWidth, headerHeight));
                x += columnWidth;

            }

            x = e.MarginBounds.Left;
            y += headerHeight;
            while (rowIndex < dgv.Rows.Count)
            {

                DataGridViewRow row = dgv.Rows[rowIndex];
                if (row.Visible)
                {

                    int rowHeight = 0;
                    foreach (DataGridViewCell cell in row.Cells)
                    {

                        DataGridViewColumn column = dgv.Columns[cell.ColumnIndex];
                        if (!column.Visible || cell.Value == null) continue;
                        int tmpWidth = (int)(Math.Floor((double)column.Width / (double)columnsWidth * (double)pagerWidth));
                        int temp = (int)e.Graphics.MeasureString(cell.Value.ToString(), column.InheritedStyle.Font, tmpWidth).Height + 2 * padding;
                        if (temp > rowHeight) rowHeight = temp;
                    }

                    foreach (DataGridViewCell cell in row.Cells)
                    {

                        DataGridViewColumn column = dgv.Columns[cell.ColumnIndex];
                        if (!column.Visible) continue;
                        int columnWidth = (int)(Math.Floor((double)column.Width / (double)columnsWidth * (double)pagerWidth));
                        e.Graphics.DrawRectangle(Pens.Black, new Rectangle(x, y, columnWidth, rowHeight));

                        if (cell.Value != null)
                        {

                            //内容居中要加的宽度

                            float cenderWidth = (columnWidth - e.Graphics.MeasureString(cell.Value.ToString(), cell.InheritedStyle.Font, columnWidth).Width) / 2;

                            if (cenderWidth < 0) cenderWidth = 0;

                            //内容居中要加的高度

                            float cenderHeight = (rowHeight + padding - e.Graphics.MeasureString(cell.Value.ToString(), cell.InheritedStyle.Font, columnWidth).Height) / 2;

                            if (cenderHeight < 0) cenderHeight = 0;

                            ////画下边线

                            //e.Graphics.DrawLine(Pens.Black, x, y + rowHeight, x + columnWidth, y + rowHeight);

                            ////画右边线

                            //e.Graphics.DrawLine(Pens.Black, x + columnWidth, y, x + columnWidth, y + rowHeight);

                            //if (x == e.MarginBounds.Left)

                            //{

                            //    //画左边线

                            //    e.Graphics.DrawLine(Pens.Black, x, y, x, y + rowHeight);

                            //}

                            //画内容

                            e.Graphics.DrawString(cell.Value.ToString(), column.InheritedStyle.Font, new SolidBrush(cell.InheritedStyle.ForeColor), new RectangleF(x + cenderWidth, y + cenderHeight, columnWidth, rowHeight));

                        }

                        x += columnWidth;

                    }

                    x = e.MarginBounds.Left;

                    y += rowHeight;

                    if (page == 1) rowsPerPage++;

                    //打印下一页

                    if (y + rowHeight > e.MarginBounds.Bottom)
                    {

                        e.HasMorePages = true;

                        break;

                    }

                }

                rowIndex++;

            }

            //页脚
            string footer = " 第 " + page + " 页，共 " + Math.Ceiling(((double)dgv.Rows.Count / rowsPerPage)).ToString() + " 页";
            //画页脚
            e.Graphics.DrawString(footer, dgv.Font, Brushes.Black, x + (pagerWidth - e.Graphics.MeasureString(footer, dgv.Font).Width) / 2, e.MarginBounds.Bottom);
            page++;

        }


    }
}

  