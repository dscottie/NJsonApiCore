using NJsonApi.Infrastructure;

namespace NJsonApi.Serialization.Representations
{
    public interface ILink
    {
        string Href { get; set; }
    }

    public interface ILinkObject : ILink
    {
        MetaData Meta { get; set; }
    }
}