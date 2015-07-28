namespace JsonApiNet
{
    public interface IJsonApiNetSerializer
    {
        dynamic ResourceFromDocument(string json);
    }
}