using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketSharp.Server;

namespace RemoteApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebSocketServer server;

        public MainWindow()
        {
            InitializeComponent();
            StartServer.IsEnabled = true;
            StopServer.IsEnabled = false;
            //OpenWebSocketServer();
            //TestKeyUp();
            //TestMouse();
        }

        private void TestMouse()
        {
            MouseFlag.MouseLeftClickEvent(20, 60, 0);
            MouseFlag.MouseLeftClickEvent(20, 60, 0);
            //MouseFlag.MouseLeftClickEvent(10, 100, 0);
        }

        //模拟win+r调出运行
        private void TestKeyUp()
        {
            KeyBoard.keyDown(KeyBoard.vKeyLWIN);
            KeyBoard.keyDown(KeyBoard.vKeyR);
            KeyBoard.keyUp(KeyBoard.vKeyR);
            KeyBoard.keyUp(KeyBoard.vKeyLWIN);
        }

        private void OpenWebSocketServer()
        {
            server = new WebSocketServer(int.Parse(PortName.Text));

            server.AddWebSocketService<Echo>("/Echo");
            server.Start();

        }

        private void CloseWebSocketServer()
        {
            server.Stop();
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            OpenWebSocketServer();
            StartServer.IsEnabled = false;
            StopServer.IsEnabled = true;
        }

        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            CloseWebSocketServer();
            StartServer.IsEnabled = true;
            StopServer.IsEnabled = false;
        }
    }
}
