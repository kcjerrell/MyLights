using MyLights.Bridges;
using MyLights.Bridges.Node2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyLights.Windows
{
    /// <summary>
    /// Interaction logic for Scratch2.xaml
    /// </summary>
    public partial class Scratch2 : Window
    {
        public Scratch2()
        {
            InitializeComponent();

            nodeService.MessageReceived += NodeService_MessageReceived;
        }

        private void NodeService_MessageReceived(object sender, Bridges.Node2.MessageReceivedEventArgs e)
        {
            Trace.WriteLine(e.Message);
        }

        NodeService nodeService = new();
        int sent = 0;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            nodeService.Connect();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            sent += 1;
            await nodeService.SendMessage($"party/hey message {sent}");
        }
    }
}
