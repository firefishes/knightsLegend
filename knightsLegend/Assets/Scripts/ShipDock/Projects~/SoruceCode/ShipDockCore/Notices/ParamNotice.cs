using ShipDock.Pooling;

namespace ShipDock.Notices
{
    public class ParamNotice<T> : Notice, IParamNotice<T>
    {
        protected override void Purge()
        {
            base.Purge();

            ParamValue = default;
        }

        public override void ToPool()
        {
            Pooling<ParamNotice<T>>.To(this);
        }

        public virtual T ParamValue { get; set; }
    }

    public interface IParamNotice<T> : INotice
    {
        T ParamValue { get; set; }
    }
}
