using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MipWindowLib.MipRobot;
using MipEchoApp.Robot;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MipEchoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        RobotService robotService;

        public MainPage()
        {
            this.InitializeComponent();
            this.robotService = new RobotService(this.Instruction);
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            await MipRobotFinder.Instance.ScanForRobots();

            MipRobot mip = MipRobotFinder.Instance.FoundRobotList.FirstOrDefault();
            if (mip != null)
            {
                if (await mip.Connect())
                {
                    this.ConnectButton.Content = "Connected: " + mip.DeviceName;

                    this.ConnectButton.IsEnabled = false;
                    this.robotService.StartProcessingCommands();
                }
            }
        }
    }
}
