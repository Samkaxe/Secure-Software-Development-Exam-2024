using System.Text;
using Application.Interfaces;
using Infrastructure.DataAccessInterfaces;
using Infrastructure.DataAccessServices;

namespace Application.Services;

public class UserEncryptionKeyService: IUserEncyrptionKeyService
{
    
    private readonly IUserRepository _userRepository;
    private readonly EncryptionHelper _encryptionHelper;

    public UserEncryptionKeyService(IUserRepository userRepository, EncryptionHelper encryptionHelper)
    {
        _userRepository = userRepository;
        _encryptionHelper = encryptionHelper;
    }

    public async Task<byte[]> GetUserEncryptionKeyAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        // Extract the encrypted Uek
        byte[] encryptedUekFromDb = user.EncryptedUek;

        // Decrypt the encrypted Uek
        byte[] decryptedUek = _encryptionHelper.DecryptWithMasterKey(encryptedUekFromDb);
        return decryptedUek;
    }
}