namespace HowLong.DependencyServices
{
    public interface IShowNotify
    {
        void SetEnd(double minutes);
        void SetHalf(double minutes);
        void CancelAll();
    }
}
