namespace ShipDock.Notices
{
    public class ParamNotice<T> : Notice, IParamNotice<T>
    {
        protected override void Purge()
        {
            base.Purge();

            ParamValue = default;
        }

        public T ParamValue { get; set; }
    }

    public interface IParamNotice<T> : INotice
    {
        T ParamValue { get; set; }
    }
}
