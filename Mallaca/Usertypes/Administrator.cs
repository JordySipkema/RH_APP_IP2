namespace Mallaca.Usertypes
{
    public class Administrator : Specialist
    {

        public Administrator() 
        {
        }

        public override UserType UserType
        {
            get
            {
                return UserType.Administrator;
            }
        }
    }
}
