using Mallaca;
namespace RH_APP.Classes
{
    class IP_Bike : IBike
    {
        private Measurement m = new Measurement();

        public void setMeasurement(Measurement m)
        {
            this.m = m;
        }

        public override Measurement RecieveData()
        {
            return m;
        }

        public override void SendData(string command)
        {
            //TODO: throw new NotImplementedException();
        }
    }
}
