namespace TransfersModule.Persistence
{
    internal class NoSalary : ISalary
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