namespace Application.Interfaces;

public interface IUserEncyrptionKeyService
{
    Task<byte[]> GetUserEncryptionKeyAsync(Guid userId);
}