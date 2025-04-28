namespace SemanticCursorJsonMapper;

public record PersonModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Address PrimaryAddress { get; set; } = new();
    public List<Address> PreviousAddresses { get; init; } = [];
    public string ReferenceId { get; set; } = string.Empty;
    public bool IsMatch { get; set; }
}