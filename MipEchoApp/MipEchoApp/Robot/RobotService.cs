using MipWindowLib.MipRobot;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using MipEchoApp.Db;

namespace MipEchoApp.Robot
{
    public class RobotService
    {
        private bool stopDancing = false;
        private readonly TextBox _textBox;

        public RobotService(TextBox textBox)
        {
            _textBox = textBox;
        }

        public IAsyncAction Move(ActionCommand command, int direction)
        {
            var robot = MipRobotFinder.Instance.FoundRobotList.FirstOrDefault();

            double distanceCm = command.Distance;

            if (distanceCm == 0)
            {
                distanceCm = 10;
            }

            distanceCm = distanceCm * direction;

            return robot.SendMipCommand(
                  MipRobotConstants.COMMAND_CODE.DRIVE_FIXED_DISTANCE,
                  //direction
                  (distanceCm > 0) ? MipRobotConstants.DRIVE_DIRECTION_FORWARD : MipRobotConstants.DRIVE_DIRECTION_BACKWARD,
                  //distance
                  (byte)Math.Abs(distanceCm),
                  //turn direction
                  (command.Degree > 0) ? MipRobotConstants.DRIVE_TURN_DIRECTION_CLOCKWISE : MipRobotConstants.DRIVE_TURN_DIRECTION_ANTI_CLOCKWISE,
                  //angle
                  (byte)(Math.Abs(command.Degree) << 8),
                  (byte)(Math.Abs(command.Degree) & 0x00ff)
              ).AsAsyncAction();
        }

        public async Task<IAsyncAction> Stop()
        {
            var robot = MipRobotFinder.Instance.FoundRobotList.FirstOrDefault();

            if (robot != null)
            {
                await robot.SetMipVolumeLevel(7);
                //await Task.Delay(1000);
                await robot.PlayMipSound(new MipRobotSound(MipRobotConstants.SOUND_FILE.MIP_IN_LOVE));
                //await Task.Delay(1000);
                return robot.SendMipCommand(MipRobotConstants.COMMAND_CODE.STOP).AsAsyncAction();
            }

            return null;
        }

        public async Task<IAsyncAction> Dance()
        {
            stopDancing = false;
            MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.SetMipVolumeLevel(7);
            await Task.Delay(1000);
            MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.PlayMipSound(new MipRobotSound(MipRobotConstants.SOUND_FILE.MIP_IN_LOVE));
            await Task.Delay(1000);
            MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.SetMipChestLedWithColor(0xee, 0xee, 0xee, 1);
            await Task.Delay(1000);
            var result2 = Move(new ActionCommand { Action = "backward", Distance = 5, Degree = 60 }, 1);
            await Task.Delay(1000);
            var result3 = Rotate(new ActionCommand { Action = "rotate", Distance = 0, Degree = -360 });
            await Task.Delay(1000);
            MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.SetMipChestLedWithColor(0xff, 0xff, 0xff, 1);
            await Task.Delay(1000);
            var random = new Random();
            while (!stopDancing)
            {
                MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.SetMipChestLedWithColor((byte)random.Next(10, 255), (byte)random.Next(10, 255), (byte)random.Next(10, 255), 1);
                await Task.Delay(1000);
                var direction = random.Next(-5, 5);
                if (direction > 0)
                {
                    var result33 = Move(new ActionCommand { Action = "backward", Distance = 5, Degree = 0 }, 1);
                    await Task.Delay(1000);
                    MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?
                        .MipTurnLeftByDegrees(random.Next(200, 360), 24);
                }
                else
                {
                    var result33 = Move(new ActionCommand { Action = "forward", Distance = 5, Degree = 0 }, 1);
                    await Task.Delay(1000);
                    MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?
                        .MipTurnRightByDegress(random.Next(200, 360), 24);
                }
                await Task.Delay(200);
            }

            return Move(new ActionCommand { Action = "forward", Distance = 5, Degree = 60 }, 1);
        }

        public IAsyncAction Rotate(ActionCommand command)
        {
            int rate = 10;
            
            return MipRobotFinder.Instance.FoundRobotList.FirstOrDefault()?.MipTurnWithRate(command.Degree, Math.Min(rate, 24));
        }

        public async void StartProcessingCommands()
        {
            while (true)
            {
                var command = GetCommand();

                if (command != null)
                {
                    stopDancing = true;
                    if (command.Action.StartsWith("for"))
                    {
                        await Move(command, 1);
                    }
                    else if (command.Action.StartsWith("back"))
                    {
                        await Move(command, -1);
                    }
                    else if (command.Action.StartsWith("dance"))
                    {
                        await this.Dance();
                    }
                    else if (command.Action.StartsWith("freeze"))
                    {
                        await this.Stop();
                    }
                    else if (command.Action.StartsWith("turn"))
                    {
                        var task = Rotate(command);
                    }
                }

                await Task.Delay(500);
            }

        }

        private ActionCommand GetCommand()
        {
            try
            {
                var db = new DbComponent();
                var result = db.Get();
                if (string.IsNullOrEmpty(result.Action)) return null;
                var action = JsonConvert.DeserializeObject<ActionCommand>(result.Action);
              
                var instruction = string.Format("{0} {1} {2}", action.Action, action.Distance, action.Degree);
                _textBox.Text = instruction;
                ClearAction(result, db);
                return action;
            }
            catch
            {
                return null;
            }
        }

        private static void ClearAction(RobotTable result, DbComponent db)
        {
            Task.Factory.StartNew(() =>
            {
                result.Action = null;
                db.Update(result);
            });
        }
    }
}

