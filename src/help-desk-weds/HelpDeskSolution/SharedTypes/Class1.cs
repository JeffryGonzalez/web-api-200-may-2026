namespace SharedTypes;

public record AddSoftwareItem(Guid Id, string Title, string Vendor);

public record RetireSoftwareItem(Guid Id, DateTimeOffset RetiredAt);