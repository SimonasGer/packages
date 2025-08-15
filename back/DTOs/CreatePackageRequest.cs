public record CreatePackageRequest
(
    string SenderName,
    string SenderPhone,
    string SenderAddress,
    string RecipientName,
    string RecipientPhone,
    string RecipientAddress
);