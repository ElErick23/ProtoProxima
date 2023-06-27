namespace ProtoProxima.Models;

public class Category
{
    public string Name { get; set; }
    public Category? Parent { get; set; }

    public override string ToString()
    {
        return Name;
    }
}