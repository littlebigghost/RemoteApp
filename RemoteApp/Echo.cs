using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RemoteApp
{
    public class Echo : WebSocketBehavior
    {
        private bool isClose = false;

        protected override void OnClose(CloseEventArgs e)
        {
            isClose = true;
            Trace.WriteLine("Closed...");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Trace.WriteLine(e.Data);
            HandleData(e.Data);
        }

        protected override void OnOpen()
        {
            Trace.WriteLine("Open...");
            //Sessions.Broadcast("Open...");

            Thread thread = new Thread(UpdateDesktop);
            thread.Start();
        }

        private void UpdateDesktop()
        {
            while (true)
            {
                if (isClose) return;
                Bitmap bitmap = GetBitmapFromScreen();
                Sessions.Broadcast(BitmapToByte(bitmap));
            }
        }

        private void HandleData(string data)
        {
            try
            {
                JObject item = (JObject)JsonConvert.DeserializeObject(data);
                int type = Int32.Parse(item["type"].ToString());
                byte keyCode;
                int x, y;
                switch (type)
                {
                    // Key Down
                    case 0:
                        keyCode = byte.Parse(item["key"].ToString());
                        KeyBoard.keyDown(keyCode);
                        break;
                    // Key Up
                    case 1:
                        keyCode = byte.Parse(item["key"].ToString());
                        KeyBoard.keyUp(keyCode);
                        break;
                    // Mouse move
                    case 2:
                        x = Convert.ToInt32(Convert.ToDouble(item["x"].ToString()));
                        y = Convert.ToInt32(Convert.ToDouble(item["y"].ToString()));
                        MouseFlag.SetCursorPos(x, y);
                        break;
                    // Mouse Left Click
                    case 3:
                        x = Convert.ToInt32(Convert.ToDouble(item["x"].ToString()));
                        y = Convert.ToInt32(Convert.ToDouble(item["y"].ToString()));
                        MouseFlag.MouseLeftClickEvent(x, y, 0);
                        break;
                    // Mouse Right Click
                    case 4:
                        x = Convert.ToInt32(Convert.ToDouble(item["x"].ToString()));
                        y = Convert.ToInt32(Convert.ToDouble(item["y"].ToString()));
                        MouseFlag.MouseRightClickEvent(x, y, 0);
                        break;
                    // Mouse Left Down
                    case 5:
                        x = Convert.ToInt32(Convert.ToDouble(item["x"].ToString()));
                        y = Convert.ToInt32(Convert.ToDouble(item["y"].ToString()));
                        MouseFlag.MouseLeftDownEvent(x, y, 0);
                        break;
                    // Mouse Left Up
                    case 6:
                        x = Convert.ToInt32(Convert.ToDouble(item["x"].ToString()));
                        y = Convert.ToInt32(Convert.ToDouble(item["y"].ToString()));
                        MouseFlag.MouseLeftUpEvent(x, y, 0);
                        break;
                    // Mouse Wheel
                    case 7:
                        x = Convert.ToInt32(Convert.ToDouble(item["x"].ToString()));
                        y = Convert.ToInt32(Convert.ToDouble(item["y"].ToString()));
                        var d = Convert.ToInt32(Convert.ToDouble(item["deltaY"].ToString())) * Convert.ToInt32(Convert.ToDouble(item["deltaFactor"].ToString()));
                        MouseFlag.MouseWheelEvent(x, y, d);
                        break;

                }
            }
            catch(Exception e)
            {

            }
        }

        private Bitmap GetBitmapFromScreen()
        {

            System.Drawing.Rectangle rc = System.Windows.Forms.SystemInformation.VirtualScreen;
            var bitmap = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, System.Drawing.CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }

        private Byte[] BitmapToByte(Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            Byte[] buffer = new Byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            return buffer;
        }

    }
}
