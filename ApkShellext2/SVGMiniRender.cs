using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ApkShellext2
{
    public class SVGMiniRender
    {
        private string[] cmd;
        private Size _size;

        public SVGMiniRender() {

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

        public Image image {
            get {
                return getImage(cmd);
            }
        }

        public static Image getImage(string[] command) {
            return null;            
        }
    }
}
