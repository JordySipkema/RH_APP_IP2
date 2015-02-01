
namespace RH_APP.Controller
{
    internal static class ResultGenerator
    {
        public static double CalculateVO2Max(int avgHeartrate, int weight, int resistance, bool isMale)
        {
            // Convert resistance from watts to kg.m/min (factor = 6,12)
            var workload = resistance*6.12;

            // Calculate the VO2max using the formula (depending on the gender)
            // VO2max is in L/min
            var Vo2max = isMale
                //Male formula:
                ? (0.00212*workload + 0.299)/(0.769*avgHeartrate - 48.5)*100
                //Female formula:
                : (0.00193*workload + 0.326)/(0.796*avgHeartrate - 56.1)*100;

            // Convert VO2 max to ml/kg/min
            var x = Vo2max*1000/weight;
            if (x < 0)
            {
                x = -x;
            }
            return x;
        }
    }
}
