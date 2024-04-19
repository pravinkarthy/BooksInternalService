using Books.Service.Internal.Api.Infrastructure;
using Books.Service.Internal.Api.Infrastructure.IdentityModels;
using Microsoft.EntityFrameworkCore;

public interface IUserRepository
{
    Task<List<TblUser>> GetAll();
}

public class UserRepository : IUserRepository
{

    private readonly ReadersDBContext _context;
    public UserRepository(ReadersDBContext context)
    {
        _context = context;
    }

    public async Task<List<TblUser>> GetAll() =>
    await _context.TblUsers.ToListAsync();

}