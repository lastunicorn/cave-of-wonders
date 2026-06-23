namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

public class ShortPotId
{
    private readonly Guid guid;

    public ShortPotId(Guid guid)
    {
        this.guid = guid;
    }
    
    public override string ToString()
    {
        return guid.ToString("N")[..8];
    }
    
    public static implicit operator ShortPotId(Guid guid)
    {
        return new ShortPotId(guid);
    }
    
    public static implicit operator Guid(ShortPotId shortPotId)
    {
        return shortPotId.guid;
    }
    
    public static implicit operator string(ShortPotId shortPotId)
    {
        return shortPotId.ToString();
    }
}