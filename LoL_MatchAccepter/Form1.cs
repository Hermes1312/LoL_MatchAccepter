using LoL_MatchAccepter.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoL_MatchAccepter
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        
        public const int BM_CLICK = 0x00F5;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private IntPtr ptr;

        private Point rsize, rpos;

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            Image imgToSave = pictureBox1.Image;

            if (sfd.ShowDialog() == DialogResult.OK)
                imgToSave.Save(sfd.FileName + ".jpg");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            ResourceManager MyResourceClass = new ResourceManager(typeof(Resources));
            ResourceSet resourceSet = MyResourceClass.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                comboBox1.Items.Add(entry.Key.ToString());
            }

            comboBox1.SelectedIndex = 0;

            SetRects();
        }

        private void SetRects()
        {
            string[] s = rsizeTB.Text.Split(';');
            string[] s1 = rposTB.Text.Split(';');
             
            rsize = new Point(int.Parse(s[0]), int.Parse(s[1]));
            rpos = new Point(int.Parse(s1[0]), int.Parse(s1[1]));
        }

        private int ImageSimilarity(Bitmap img1, Bitmap img2)
        {
            int count1 = 0, count2 = 0;
            bool flag = true;
            string img1_ref, img2_ref;

            if (img1.Width == img2.Width && img1.Height == img2.Height)
            {
                for (int i = 0; i < img1.Width; i++)
                {
                    for (int j = 0; j < img1.Height; j++)
                    {
                        img1_ref = img1.GetPixel(i, j).ToString();
                        img2_ref = img2.GetPixel(i, j).ToString();
                        if (img1_ref != img2_ref)
                        {
                            count2++;
                            flag = false;
                            break;
                        }
                        count1++;
                    }
                }

                return count1;
            }
            else
                return 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetRects();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bitmap bm = (Bitmap)Resources.ResourceManager.GetObject(comboBox1.Text);
            progressBar1.Maximum = bm.Width * bm.Height;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("LeagueClientUx");
            Process lol = processes[0];
            ptr = lol.MainWindowHandle;

            Rect rect = new Rect();
            GetWindowRect(ptr, ref rect);

            Rectangle _rect = new Rectangle(0, 0, rsize.X, rsize.Y);

            Bitmap bmp = new Bitmap(_rect.Width, _rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);

            g.CopyFromScreen(rect.Left + rpos.X, rect.Top + rpos.Y, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            pictureBox1.Image = bmp;

            if (checkBox1.Checked)
            {

                Bitmap bm = (Bitmap)Resources.ResourceManager.GetObject(comboBox1.Text);
                int sim = ImageSimilarity(bm, bmp);

                progressBar1.Value = sim;

                if (sim > (bm.Width*bm.Height)*0.8)
                {
                    panel1.BackColor = Color.Green;
                    //Cursor.Position = new Point(rect.Left + 640, rect.Top + 560);
                    //mouse_event(Mouse.MOUSEEVENTF_LEFTDOWN | Mouse.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else
                    panel1.BackColor = Color.Red;
            }
            else
                panel1.BackColor = Color.Blue;

        }

    }

    internal class Mouse
    {
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
    }
}
