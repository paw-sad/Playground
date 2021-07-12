namespace TransfersModule.Persistence
{
    public class NoSalary : ISalary
    {
        public bool Equals(ISalary other)
        {
            if (other is NoSalary)
            {
                return true;
            }

            return false;
        }
    }
}