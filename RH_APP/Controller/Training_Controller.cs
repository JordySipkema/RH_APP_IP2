using Mallaca;
using RH_APP.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RH_APP.Controller
{
    class Training_Controller
    {
        private RH_Controller _controller;
        private Timer _aTimer; 

        public Training_Controller(string port)
        {
            _controller = new RH_Controller(new COM_Bike(port), true);
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

            _aTimer = new System.Timers.Timer(10000);
            _aTimer.Elapsed += PowerCheckAndSet;
            _aTimer.Enabled = true;
            
        }

        private void PowerCheckAndSet(Object source, ElapsedEventArgs e)
        {
            if (Settings.GetInstance().CurrentUser.Gender == "m" && _controller.LatestMeasurement.PULSE < 120)
            {
                _controller.SetPower(_controller.LatestMeasurement.POWER + 50);
    
            }
            else if (Settings.GetInstance().CurrentUser.Gender == "f" && _controller.LatestMeasurement.PULSE < 120)
            {
                    _controller.SetPower(_controller.LatestMeasurement.POWER + 25);                
            }

            if (_controller.LatestMeasurement.PULSE > 120 && _controller.LatestMeasurement.PULSE < 170)
            {
                Console.WriteLine("PRE TRAINING STOPPED!! MAIN TRAINING STARTED!!");
                _aTimer.Enabled = false;
                Console.WriteLine("TIMER STOPPED!");
            }
        }
    }
}
