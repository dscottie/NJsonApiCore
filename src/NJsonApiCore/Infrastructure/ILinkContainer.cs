using NJsonApi.Infrastructure;

namespace NJsonApi.Serialization.Representations
{
    public interface ISerializableObjectLinkContainer
    {
        ILinkData GetLinks();
    }

    public interface IDeserializableObjectLinkContainer
    {
        void SetLinks(ILinkData linkData);
    }

    public class ObjectLinkContainer : ISerializableObjectLinkContainer, IDeserializableObjectLinkContainer
    {
        private ILinkData _linkData = new LinkData();

        public ILinkData GetLinks()
        {
            return _linkData;
        }

        public void SetLinks(ILinkData linkData)
        {
            _linkData = linkData;
        }
    }

}
