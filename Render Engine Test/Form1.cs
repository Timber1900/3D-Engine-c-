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
        public Boolean isWPressed = false;
        public Boolean isAPressed = false;
        public Boolean isSPressed = false;
        public Boolean isDPressed = false;
        public Boolean isUpPressed = false;
        public Boolean isDownPressed = false;
        public Camera c = new Camera(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f), 0);


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
            c.checkKeys(isWPressed, isAPressed, isSPressed, isDPressed, isUpPressed, isDownPressed);

            Matrix4x4 rotationx = Matrix4x4.createRotationX(angle1);
            Matrix4x4 rotationz = Matrix4x4.createRotationZ(angle2);
            Matrix4x4 matView   = createPointAt();
            Matrix4x4 final  =  matView * (transMatrix * (rotationx * rotationz));

            Mesh unProjMesh = CubeMesh.projectTriangles(final);

            Mesh checkedMesh = unProjMesh.checkDir(new Vector3(0f, 0f, 0f));
            Mesh clippedMesh = checkedMesh.clipTriangles(new Vector3(0f, 0f, 0.1f), new Vector3(0f, 0f, 1f));
            clippedMesh.sort();

            Mesh projMesh = clippedMesh.projectTriangles(projMatrix);
            Mesh finalMesh = --projMesh;
            finalMesh = finalMesh.clipTriangles(new Vector3(0f, -1f, 0f), new Vector3(0f, 1f, 0f));
            finalMesh = finalMesh.clipTriangles(new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f));
            finalMesh = finalMesh.clipTriangles(new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f));
            finalMesh = finalMesh.clipTriangles(new Vector3(1f, 0f, 0f), new Vector3(-1f, 0f, 0f));
            drawMesh(finalMesh);
            pictureBox1.Image = BMP;
            angle1 += 0.01f;
            angle2 += 0.02f;
        }

        private void drawMesh(Mesh mesh)
        {
            foreach(Triangle triangle in mesh.triangles)
            {  
                PointF p1 = new PointF(((triangle.p1.x) + 1.0f) * (0.5f * pictureBox1.Width), -(((triangle.p1.y / triangle.p1.w) + 1.0f) * (0.5f * pictureBox1.Height) - pictureBox1.Height));
                PointF p2 = new PointF(((triangle.p2.x) + 1.0f) * (0.5f * pictureBox1.Width), -(((triangle.p2.y / triangle.p2.w) + 1.0f) * (0.5f * pictureBox1.Height) - pictureBox1.Height));
                PointF p3 = new PointF(((triangle.p3.x) + 1.0f) * (0.5f * pictureBox1.Width), -(((triangle.p3.y / triangle.p3.w) + 1.0f) * (0.5f * pictureBox1.Height) - pictureBox1.Height));
                PointF[] points = new PointF[] { p1, p2, p3 };
                //GRX.DrawPolygon(Pens.White, points);
                Color c;
                if (triangle.hasColor)
                {
                    c = Color.FromArgb((int)triangle.c.x, (int)triangle.c.y, (int)triangle.c.z);
                    //c = Color.FromArgb((int)((triangle.lightIntensity + 1f) * (220f / 2f)), (int)((triangle.lightIntensity + 1f) * (220f / 2f)), (int)((triangle.lightIntensity + 1f) * (220f / 2f)));

                }
                else
                {
                    c = Color.FromArgb((int)((triangle.lightIntensity + 1f) * (220f / 2f)), (int)((triangle.lightIntensity + 1f) * (220f / 2f)), (int)((triangle.lightIntensity + 1f) * (220f / 2f)));
                }
                SolidBrush b = new SolidBrush(c);
                GRX.FillPolygon(b, points);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    isWPressed = true;
                    break;
                case Keys.A:
                    isAPressed = true;
                    break;
                case Keys.S:
                    isSPressed = true;
                    break;
                case Keys.D:
                    isDPressed = true;
                    break;
                case Keys.Up:
                    isUpPressed = true;
                    break;
                case Keys.Down:
                    isDownPressed = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    isWPressed = false;
                    break;
                case Keys.A:
                    isAPressed = false;
                    break;
                case Keys.S:
                    isSPressed = false;
                    break;
                case Keys.D:
                    isDPressed = false;
                    break;
                case Keys.Up:
                    isUpPressed = false;
                    break;
                case Keys.Down:
                    isDownPressed = false;
                    break;
            }
        }
        public Matrix4x4 createPointAt()
        {
            Vector3 vUp = new Vector3(0f, 1f, 0f);
            Vector4 vTarget = new Vector4(0f, 0f, 1f, 1f);
            Matrix4x4 matCameraRot = Matrix4x4.createRotationY(c.fyaw);
            Vector4 temp = matCameraRot * vTarget;
            c.dir = new Vector3(temp.x, temp.y, temp.z);
            Vector3 _vTarget = c.pos + c.dir;
            Matrix4x4 matCamera = Matrix4x4.createLookAt(c.pos, _vTarget, vUp);
            return Matrix4x4.quickInverse(matCamera);
        }
    }
}
