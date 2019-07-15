using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using Svg;

namespace ApkShellext2
{

    /*
    <!-- res/drawable/battery_charging.xml -->
    <vector xmlns:android="http://schemas.android.com/apk/res/android"
        <!-- intrinsic size of the drawable -->
        android:height="24dp"
        android:width="24dp"
        <!-- size of the virtual canvas -->
        android:viewportWidth="24.0"
        android:viewportHeight="24.0">
       <group
             android:name="rotationGroup"
             android:pivotX="10.0"
             android:pivotY="10.0"
             android:rotation="15.0" >
          <path
            android:name="vect"
            android:fillColor="#FF000000"
            android:pathData="M15.67,4H14V2h-4v2H8.33C7.6,4 7,4.6 7,5.33V9h4.93L13,7v2h4V5.33C17,4.6 16.4,4 15.67,4z"
            android:fillAlpha=".3"/>
          <path
            android:name="draw"
            android:fillColor="#FF000000"
            android:pathData="M13,12.5h2L11,20v-5.5H9L11.93,9H7v11.67C7,21.4 7.6,22 8.33,22h7.33c0.74,0 1.34,-0.6 1.34,-1.33V9h-4v3.5z"/>
       </group>
    </vector>
    */
    public class VectorDrawableRender
    {
        private static char[] SVGCommandAbsolute = { 'M', 'L', 'H', 'V', 'C', 'S', 'Q', 'T', 'A', 'Z', 'z' };
        private static char[] SVGCommandRelated = { 'm', 'l', 'h', 'v', 'c', 's', 'q', 't', 'a' };
        private static char[] SVGCommand = { 'M', 'L', 'H', 'V', 'C', 'S', 'Q', 'T', 'A', 'Z', 'z', 'm', 'l', 'h', 'v', 'c', 's', 'q', 't', 'a' };
        private string[] cmd;
        private Size _size;
        private XmlDocument _xml;
        public ApkQuickReader.ApkReader apkreader { set; get; }

        public VectorDrawableRender() {

        }

        public VectorDrawableRender(XmlDocument xml) {
            _xml = xml;
        }

        public void addCmd() {

        }

        public Size size {
            get {
                return _size;
            }
            set {
                _size = value;
            }
        }

        public Bitmap image {
            get {
                return getImage(_xml);
            }
        }

        public Bitmap getImage(XmlDocument xml) {
            XmlElement vector = (XmlElement)xml.DocumentElement.SelectSingleNode("/vector");
            int viewportWidth = 0, viewportHeight = 0;
            //int width = 0, height = 0;
            if (vector.HasAttribute("viewportWidth")) {
                viewportWidth = int.Parse(vector.GetAttribute("viewportWidth"));
            }
            if (vector.HasAttribute("viewportHeight")) {
                viewportHeight = int.Parse(vector.GetAttribute("viewportHeight"));
                if (viewportWidth == 0) viewportWidth = viewportHeight;
            } else {
                viewportHeight = viewportWidth;
            }
            //if (vector.HasAttribute("width")) {
            //    width = int.Parse(vector.GetAttribute("width"));
            //} else {
            //    width = viewportWidth;
            //}
            //if (vector.HasAttribute("height")) {
            //    height = int.Parse(vector.GetAttribute("height"));
            //} else {
            //    height = viewportWidth;
            //}
            Bitmap b = new Bitmap(viewportWidth, viewportHeight);
            using (Graphics g = Graphics.FromImage(b)) {
                XmlElement group = (XmlElement)vector.SelectSingleNode("group");
                XmlNodeList nl;
                if (group != null) {
                    nl = group.SelectNodes("path");
                } else {
                    nl = vector.SelectNodes("path");
                }
                foreach (XmlNode n in nl) {
                    XmlElement elem = (XmlElement)n;
                    string pathdata = elem.GetAttribute("pathData");
                    GraphicsPath path = Convert2Path(pathdata);
                    Brush fill = null;
                    if (elem.HasAttribute("fillColor")) {
                        string fillcolor = elem.GetAttribute("fillColor");
                        if (fillcolor.EndsWith(".xml")) {//gradien

                        } else {
                            int color = int.Parse(elem.GetAttribute("fillColor").Substring(2), System.Globalization.NumberStyles.HexNumber);
                            fill = new SolidBrush(Color.FromArgb(color));
                        }
                    } else {
                        fill = new SolidBrush(Color.Black);
                    }
                    g.FillPath(fill, path);
                    //g.DrawPath(new Pen(fill, 2), path);                    
                }
            }
            return b;
        }

        public static GraphicsPath Convert2Path(string pathdata) {
            GraphicsPath path = new GraphicsPath(FillMode.Alternate);
#if USE_NATIVE_SVG_PARSER
            #region native
            List<string> token = new List<string>();
            string num = "";

            foreach (char c in pathdata) {
                if (SVGCommand.Contains(c)) {
                    if (num != "") {
                        token.Add(num);
                        num = "";
                    }
                    token.Add(c.ToString());
                } else if ((c >= '0' && c <= '9') || c == '.' || c == '-') {
                    num = num + c.ToString();
                } else if (c == ',' || c == ' ') {
                    if (num != "" && num != "-") {
                        token.Add(num);
                        num = "";
                    }
                }
            }
            if (num != "") token.Add(num);
            
            PointF startpoint = new Point(0, 0), endpoint = startpoint;
            PointF controlp1 = new PointF(-1, -1), controlp2 = controlp1, controlp = startpoint;
            
            char lastCmd = ' ';
            char command = ' ';
            try {
                for (int i = 0; i < token.Count();i++) {
                    command = (lastCmd == ' ' || SVGCommand.Contains(token[i][0])) ?token[i][0] : lastCmd;
                    bool isrelative;
                    if (SVGCommand.Contains(command))
                        isrelative = SVGCommandRelated.Contains(command);
                    else
                        throw new Exception("Unsupported command in " + pathdata);

                    switch (command) {
                        case 'M':
                        case 'm':
                            path.StartFigure();
                            endpoint = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            break;
                        case 'L':
                        case 'l':
                            endpoint = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            path.AddLine(startpoint, endpoint);
                            break;
                        case 'H':
                        case 'h':
                            endpoint = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(startpoint.X, float.Parse(token[++i])));
                            path.AddLine(startpoint, endpoint);
                            break;
                        case 'V':
                        case 'v':
                            endpoint = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(float.Parse(token[++i]), startpoint.Y));
                            path.AddLine(startpoint, endpoint);
                            break;
                        case 'C':
                        case 'c': // cubiccurve
                            controlp1 = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            controlp2 = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            endpoint = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            path.AddBezier(startpoint, controlp1, controlp2, endpoint);
                            break;
                        case 'S':
                        case 's': // cubiccurve                            
                            controlp1 = (lastCmd == 'C' || lastCmd == 'c' ||
                                lastCmd == 'S' || lastCmd == 's' || lastCmd == ' ') ? controlp2 : startpoint;
                            controlp2 = ConvertAbsolute(isrelative, startpoint, new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            endpoint = ConvertAbsolute(isrelative, startpoint, new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            path.AddBezier(startpoint, controlp1, controlp2, endpoint);
                            break;
                        case 'Q':
                        case 'q': // q & t -> QuadraticCurve
                            controlp = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            endpoint = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            controlp1 = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(startpoint.X + (controlp.X - startpoint.X) * 2 / 3,
                                                                                    startpoint.Y + (controlp.Y - startpoint.Y) * 2 / 3));
                            controlp2 = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(controlp.X + (endpoint.X - controlp.X) / 3,
                                           controlp.Y + (endpoint.Y - controlp.Y) / 3));
                            path.AddBezier(startpoint, controlp1, controlp2, endpoint);
                            break;
                        case 'T':
                        case 't': // q & t -> QuadraticCurve                                                        
                            endpoint = ConvertAbsolute(isrelative, startpoint, new PointF(float.Parse(token[++i]), float.Parse(token[++i])));
                            controlp = (lastCmd == 'Q' || lastCmd == 'q' || lastCmd == 'T' || lastCmd == 't') ?
                                Reflection(controlp, startpoint) :
                                startpoint;
                            controlp1 = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(startpoint.X + (controlp.X - startpoint.X) * 2 / 3,
                                           startpoint.Y + (controlp.Y - startpoint.Y) * 2 / 3));
                            controlp2 = ConvertAbsolute(isrelative, startpoint, 
                                new PointF(controlp.X + (endpoint.X - controlp.X) / 3,
                                           controlp.Y + (endpoint.Y - controlp.Y) / 3));
                            path.AddBezier(startpoint, controlp1, controlp2, endpoint);
                            break;
                        case 'A':
                        case 'a':
                            double rx = float.Parse(token[++i]);
                            double ry = float.Parse(token[++i]);
                            double xrotate = double.Parse(token[++i]);
                            double φ = xrotate * Math.PI / 180;
                            int largeArcFlag = int.Parse(token[++i]);
                            int sweepFlag = int.Parse(token[++i]);
                            endpoint = ConvertAbsolute(isrelative, startpoint, new PointF(float.Parse(token[++i]), float.Parse(token[++i])));

                            // refer to https://www.w3.org/TR/SVG/implnote.html
                            if (endpoint.X == startpoint.X && endpoint.Y == startpoint.Y) continue;
                            //Correction: Step 1: Ensure radii are non-zero
                            if (rx == 0 || ry == 0)
                                path.AddLine(startpoint, endpoint);
                            //Correction:Step 2: Ensure radii are positive
                            rx = Math.Abs(rx); ry = Math.Abs(ry);

                            //Step 1: Compute(x1′, y1′)
                            double cosφ = Math.Cos(φ), sinφ = Math.Sin(φ);
                            double[,] x1_y1 = MultiplyMatrix(new double[,] { { cosφ, sinφ },
                                                                             { -sinφ, cosφ } },
                                                             new double[,] { { (startpoint.X - endpoint.X) / 2 }, 
                                                                             { (startpoint.Y - endpoint.Y) / 2 } });
                            double x1_ = x1_y1[0, 0];
                            double y1_ = x1_y1[1, 0];                            
                            

                            //Step 2: Compute (cx′, cy′)
                            double rx2 = rx * rx, ry2 = ry * ry, y1_2 = y1_ * y1_, x1_2 = x1_ * x1_;

                            //Correction:Step 3: Ensure radii are large enough
                            double lamda = x1_2 / rx2 + y1_2 / ry2;
                            double cx_, cy_;
                            if (lamda > 1) {
                                rx = Math.Sqrt(lamda) * rx;
                                ry = Math.Sqrt(lamda) * ry;
                                cx_ = 0; cy_ = 0;
                            } else {
                                double t = Math.Sqrt((rx2 * ry2 - rx2 * y1_2 - ry2 * x1_2) / (rx2 * y1_2 + ry2 * x1_2));
                                cx_ = t * rx * y1_ / ry;
                                cy_ = -t * ry * x1_ / rx;
                                if (largeArcFlag == sweepFlag) {
                                    cx_ = -cx_;
                                    cy_ = -cy_;
                                }
                            }
                            //Step 3: Compute(cx, cy) from(cx′, cy′)                        
                            double cx = cosφ * cx_ - sinφ * cy_ + (startpoint.X + endpoint.X) / 2;
                            double cy = sinφ * cx_ + cosφ * cy_ + (startpoint.Y + endpoint.Y) / 2;
                            PointF center = new PointF((float)cx, (float)cy);

                            //Step 4: Compute θ1 and Δθ
                            //double θ1 = Math.Acos((y1_ - cy_) * ry / (length(new double[] { 1, 0 }) * length(new double[] { (x1_ - cx_) / rx, (y1_ - cy_) / ry })));
                            double θ1 = VectorAngle(new double[,] { { 1 }, { 0 } },
                                new double[,] {
                                { (x1_ - cx_ / rx) },
                                { (y1_ - cy / ry) }
                                });
                            double deltaθ = VectorAngle(
                                new double[,] { { (x1_ - cx_ / rx) },
                                            { (y1_ - cy / ry)  } },
                                new double[,] { { (-x1_ - cx_) / rx },
                                            { (-y1_ - cy_) / ry } });

                            Matrix trans = new Matrix();
                            trans.RotateAt((float)xrotate, center);
                            path.Transform(trans);
                            path.AddArc(new Rectangle((int)(cx - rx), (int)(cy - ry), 2 * (int)rx, 2 * (int)ry), (float)θ1, (float)deltaθ);
                            trans.RotateAt((float)-xrotate, center);
                            path.Transform(trans);
                            break;
                        case 'Z':
                        case 'z':
                            path.CloseFigure();
                            endpoint = new PointF(0, 0);
                            break;
                        default:
                            throw new Exception("Unsurpported command");
                    }
                    lastCmd = (command == 'M') ? 'L' :
                              (command == 'm') ? 'l' :
                              (command == 'Z' || command == 'z') ? ' ' :
                                                                   command;
                    startpoint = endpoint;
                }
            } catch {
                //throw new Exception("Error happens during parsing path data: " + pathdata);
                SharpShell.Diagnostics.Logging.Log("Error happens during parsing path data: " + pathdata);
                //return path;
            }
            
            #endregion
#else
            #region Use SVG lib
            try {
                Svg.Pathing.SvgPathSegmentList l = SvgPathBuilder.Parse(pathdata);
                foreach (Svg.Pathing.SvgPathSegment s in l) {
                    s.AddToPath(path);
                }
            } catch (Exception e){
                Utility.Log(null, "VectorDrawable, d=" + pathdata, e.Message);
            }
            #endregion
#endif
            return path;
        }

        private static PointF ConvertAbsolute(bool isrelative, PointF start, PointF p) {
            return (isrelative) ? new PointF(start.X + p.X, start.Y + p.Y) : p;
        }

        private static PointF Reflection(PointF p, PointF center) {
            return new PointF((2 * center.X) - p.X, (2 * center.Y) - p.Y);
        }

        // 2-d * 1d
        // | a b | * | e | = |a*e + b*f |
        // | c d |   | f |   |c*e + d*f |
        // 1d * 2-d
        // |a| * |c d| = |a*c a*d|
        // |b|   |e f|   |b*e b*f|
        // 2d *2d
        // |a b| * |e f| = |ae+bg af+bh|
        // |c d|   |g h|   |ce+dg cf+dh|
        private static double[,] MultiplyMatrix(double[,] a, double[,] b) {
            if (a.GetLength(1) == b.GetLength(0)) {
                double[,] c = new double[a.GetLength(0), b.GetLength(1)];
                for (int i = 0; i < c.GetLength(0); i++) {
                    for (int j = 0; j < c.GetLength(1); j++) {
                        for (int k = 0; k < a.GetLength(1); k++) // OR k<b.GetLength(0)
                            c[i, j] += a[i, k] * b[k, j];
                    }
                }
                return c;
            } else {
                throw new Exception("Matrix cannot be mutilplied");
            }
        }

        private static double VectorLength(double[,] vector) {
            return Math.Sqrt(vector[0,0] * vector[0,0] + vector[1,0] * vector[1,0]);
        }

        private static double VectorAngle(double[,] a, double[,] b) {
            double axb = a[0, 0] * b[0, 0] + a[1, 0] * b[1, 0];
            double temp= Math.Acos(axb / (VectorLength(a) * VectorLength(b)));
            if (a[0,0]*b[1,0]-a[1,0]*b[0,0] < 0) {
                return -temp;
            }
            return temp;
        }
    }
}
