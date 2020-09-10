using System;
using System.Drawing;
using System.Windows.Forms;
using Utilities;

namespace Render_Engine_Test
{
    public partial class Form1 : Form
    {
        public static Bitmap BMP = new Bitmap(800, 600);
        public static Graphics GRX = Graphics.FromImage(BMP);
        protected Mesh CubeMesh;
        protected Matrix4x4 projMatrix;
        protected Matrix4x4 transMatrix;
        protected Matrix4x4 transMatrix2;
        public float angle1 = 0.0f;
        public float angle2 = 0.0f;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            CubeMesh = LoadObj.loadObj(@"C:\Users\35196\source\repos\Render Engine Test\Render Engine Test\obj\torus.obj");
            float fov = 1.0f / (float)Math.Tan(Math.PI / 4.0f);
            float ar = 1.0f;
            float znear = 0.1f;
            float zfar = 1000f;
            projMatrix = Matrix4x4.createProjection(fov, ar, zfar, znear);
            transMatrix = Matrix4x4.createTranslation(new Vector3(0f, 0f, 3f));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GRX.FillRectangle(Brushes.Black, 0, 0, pictureBox1.Width, pictureBox1.Height);
            Matrix4x4 rotationx = Matrix4x4.createRotationX(angle1);
            Matrix4x4 rotationz = Matrix4x4.createRotationZ(angle2);
            Matrix4x4 final =  (transMatrix * (rotationx * rotationz));
            Mesh unProjMesh = CubeMesh.projectTriangles(final);

            Vector3 cam = new Vector3(0f, 0f, 0f);
            Mesh checkedMesh = unProjMesh.checkDir(cam);

            checkedMesh.sort();

            Mesh projMesh = checkedMesh.projectTriangles(projMatrix);
            drawMesh(projMesh);
            pictureBox1.Image = BMP;
            angle1 += 0.01f;
            angle2 += 0.02f;
        }

        private void drawMesh(Mesh mesh)
        {
            foreach(Triangle triangle in mesh.triangles)
            {  
                PointF p1 = new PointF(((triangle.p1.x / triangle.p1.w) + 1.0f) * (0.5f * pictureBox1.Width), ((triangle.p1.y / triangle.p1.w) + 1.0f) * (0.5f * pictureBox1.Height));
                PointF p2 = new PointF(((triangle.p2.x / triangle.p2.w) + 1.0f) * (0.5f * pictureBox1.Width), ((triangle.p2.y / triangle.p2.w) + 1.0f) * (0.5f * pictureBox1.Height));
                PointF p3 = new PointF(((triangle.p3.x / triangle.p3.w) + 1.0f) * (0.5f * pictureBox1.Width), ((triangle.p3.y / triangle.p3.w) + 1.0f) * (0.5f * pictureBox1.Height));
                PointF[] points = new PointF[] { p1, p2, p3 };
                //GRX.DrawPolygon(Pens.Black, points);
                Color c = Color.FromArgb((int)((triangle.lightIntensity + 1f) * (220f / 2f)), (int)((triangle.lightIntensity + 1f) * (220f / 2f)), (int)((triangle.lightIntensity + 1f) * (220f / 2f)));
                SolidBrush b = new SolidBrush(c);
                GRX.FillPolygon(b, points);
            }
        }
    }
}
