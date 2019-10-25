using Lunatics.Mathematics;
using Lunatics.SDLGL;
using SharpCEGui.Base;

//#if __MACOS__
//using OpenGL;
//using Icehole.OpenGL;
//#else
//using OpenTK;
//using OpenTK.Graphics.OpenGL;
//#endif

namespace SharpCEGui.OpenGLRenderer
{
    public abstract class OpenGLRenderTarget : RenderTarget 
    {
        protected OpenGLRenderTarget(OpenGLRendererBase owner)
        {
            Owner = owner;
        }

        public override void Activate()
        {
	        OpenGL.GL.Viewport((int) d_area.Left,
	                           (int) d_area.Top,
	                           (int) d_area.Width,
	                           (int) d_area.Height);

            if (!d_matrixValid)
                UpdateMatrix();

            Owner.SetViewProjectionMatrix(d_matrix);

            base.Activate();
        }

        public override void UnprojectPoint(GeometryBuffer buff, Vector2 pIn, out Vector2 pOut)
        {
	        if (!d_matrixValid)
		        UpdateMatrix();

	        var gb = (OpenGLGeometryBufferBase) buff;

	        var vp = new[] {(int) d_area.Left, (int) d_area.Top, (int) d_area.Width, (int) d_area.Height};

	        var viewPort = new Vector4(vp[0], vp[1], vp[2], vp[3]);
	        var projMatrix = d_matrix;
	        var modelMatrix = gb.GetModelMatrix();
	        var wvp = modelMatrix * Matrix.Identity * projMatrix;

	        // unproject the ends of the ray
	        var inX = vp[2] * 0.5f;
	        var inY = vp[3] * 0.5f;
	        var inZ = -d_viewDistance;
	        var unprojected1 = Vector3.Unproject(new Vector3(inX, inY, inZ),
	                                             viewPort.X, viewPort.Y, viewPort.Z, viewPort.W, 0f,
	                                             1f,
	                                             wvp);
	        inX = pIn.X;
	        inY = vp[3] - pIn.Y;
	        inZ = 0.0f;
	        var unprojected2 = Vector3.Unproject(new Vector3(inX, inY, inZ),
	                                             viewPort.X, viewPort.Y, viewPort.Z, viewPort.W, 0f,
	                                             1f,
	                                             wvp);

	        // project points to orientate them with GeometryBuffer plane
	        inX = 0.0f;
	        inY = 0.0f;
	        var projected1 = Vector3.Project(new Vector3(inX, inY, inZ),
	                                         viewPort.X, viewPort.Y, viewPort.Z, viewPort.W, 0f, 1f,
	                                         wvp);
	        inX = 1.0f;
	        inY = 0.0f;
	        var projected2 = Vector3.Project(new Vector3(inX, inY, inZ),
	                                         viewPort.X, viewPort.Y, viewPort.Z, viewPort.W, 0f, 1f,
	                                         wvp);
	        inX = 0.0f;
	        inY = 1.0f;
	        var projected3 = Vector3.Project(new Vector3(inX, inY, inZ),
	                                         viewPort.X, viewPort.Y, viewPort.Z, viewPort.W, 0f, 1f,
	                                         wvp);

	        // calculate vectors for generating the plane
	        var pv1 = projected2 - projected1;
	        var pv2 = projected3 - projected1;
	        // given the vectors, calculate the plane normal
	        var planeNormal = Vector3.Cross(pv1, pv2);
	        // calculate plane
	        var planeNormalNormalized = Vector3.Normalize(planeNormal);
	        var pl_d = -Vector3.Dot(projected1, planeNormalNormalized);
	        // calculate vector of picking ray
	        var rv = unprojected1 - unprojected2;
	        // calculate intersection of ray and plane
	        var pn_dot_r1 = Vector3.Dot(unprojected1, planeNormal);
	        var pn_dot_rv = Vector3.Dot(rv, planeNormal);
	        var tmp1 = pn_dot_rv != 0.0 ? (pn_dot_r1 + pl_d) / pn_dot_rv : 0.0;
	        var isX = unprojected1.X - rv.X * tmp1;
	        var isY = unprojected1.Y - rv.Y * tmp1;

	        pOut = new Vector2((float) isX, (float) isY);
        }

        public override Renderer GetOwner()
        {
            return Owner;
        }

        /// <summary>
        /// helper that initialises the cached matrix
        /// </summary>
        protected virtual void UpdateMatrix()
        {
            UpdateMatrix(CreateViewProjMatrixForOpenGL());
            ////UpdateMatrix(CreateViewProjMatrixForDirect3D());

            //var w = d_area.Width;
            //var h = d_area.Height;

            //// We need to check if width or height are zero and act accordingly to prevent running into issues
            //// with divisions by zero which would lead to undefined values, as well as faulty clipping planes
            //// This is mostly important for avoiding asserts
            //var widthAndHeightNotZero = (w != 0.0f) && (h != 0.0f);

            //var aspect = widthAndHeightNotZero ? w / h : 1.0f;
            //var midx = widthAndHeightNotZero ? w * 0.5f : 0.5f;
            //var midy = widthAndHeightNotZero ? h * 0.5f : 0.5f;
            //d_viewDistance = midx / (aspect * d_yfov_tan);

            //var eye = new OpenTK.Vector3(midx, midy, -d_viewDistance);
            //var center = new OpenTK.Vector3(midx, midy, 1);
            //var up = new OpenTK.Vector3(0, -1, 0);

            //var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver6, aspect, d_viewDistance * 0.5f, d_viewDistance * 2.0f);
            //// Projection matrix abuse!
            //var viewMatrix = Matrix4.LookAt(eye, center, up);

            //var matrix = viewMatrix*projectionMatrix;
            //UpdateMatrix(new Matrix(matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            //                        matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            //                        matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            //                        matrix.M41, matrix.M42, matrix.M43, matrix.M44));
        }

        /// <summary>
        /// OpenGLRendererBase that created this object
        /// </summary>
        protected OpenGLRendererBase Owner;
    }
}
