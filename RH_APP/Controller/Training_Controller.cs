using Mallaca;
using RH_APP.Classes;
using System;
using System.Linq;
using System.Timers;

namespace RH_APP.Controller
{
    class Training_Controller
    {
        private RH_Controller _controller;
        private Timer _trainingTimer;
        private int _count = 36;

        public Training_Controller(string port)
        {
            _controller = new RH_Controller(new COM_Bike(port), true);
            _controller.UpdatedList += _controller_UpdatedList;
        }

        private void _controller_UpdatedList(object sender, EventArgs e)
        {
            OnUpdatedList(e as MeasurementEventArgs);
        }

        public delegate void MessageDelegate(String sender, String message);

        public event MessageDelegate MessageEvent;

        private void OnMessageEvent(string sender, string message)
        {
            MessageDelegate handler = MessageEvent;
            if (handler != null) handler(sender, message);
        }

        public event EventHandler UpdatedList;

        private void OnUpdatedList(MeasurementEventArgs e)
        {
            var handler = UpdatedList;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void StartPreTraining()
        {

            _trainingTimer = new System.Timers.Timer(30000);
            _trainingTimer.Elapsed += PowerCheckAndSet;
            _trainingTimer.Enabled = true;
            
        }

        public void StartTraining()
        {
            _trainingTimer.Enabled = false;
            _trainingTimer = new System.Timers.Timer(5000);
            _trainingTimer.Elapsed += MainTraining;
            _trainingTimer.Enabled = true;

        }

        public void MainTraining(Object source, ElapsedEventArgs e)
        {
            if (_controller.LatestMeasurement.RPM < 50 || _controller.LatestMeasurement.RPM > 60)
            {
                OnMessageEvent("System",
                    _controller.LatestMeasurement.RPM < 50
                        ? "RPM is too low. Minimum = 50 RPM"
                        : "RPM is to high, Maximum = 60 RPM");
            }

            if(_count >= 48 && ((_count - 48) % 3) == 0){
                var msmt = (from m in _controller.Measurements
                        where m.DATE > DateTime.Now.Subtract(new TimeSpan(0,0,15))
                        && m.DATE < DateTime.Now.Subtract(new TimeSpan(0,0,14))
                        select m).FirstOrDefault();

                if (msmt != null)
                {
                    var difference = _controller.LatestMeasurement.PULSE - msmt.PULSE;
                    if (difference > -3 && difference < 3)
                    {
                        Console.WriteLine("End training");
                        
                    }
                }
            }
            _count++;


        }

        private void PowerCheckAndSet(Object source, ElapsedEventArgs e)
        {
            if (Settings.GetInstance().CurrentUser.Gender == "m" && _controller.LatestMeasurement.PULSE < 120)
            {
                _controller.SetPower(_controller.LatestMeasurement.POWER + 50);
    
            }
            else if (Settings.GetInstance().CurrentUser.Gender == "v" && _controller.LatestMeasurement.PULSE < 120)
            {
                    _controller.SetPower(_controller.LatestMeasurement.POWER + 25);                
            }

            if (_controller.LatestMeasurement.PULSE > 120 && _controller.LatestMeasurement.PULSE < 170)
            {               
                StartTraining();
                OnMessageEvent("System", "Training started");

            }
        }
    }
}
