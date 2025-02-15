public class ActivationKey
    {
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Signature { get; set; } 
    }