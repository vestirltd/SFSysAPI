using SFSysAPI.Models;
namespace SFSysAPI.Interfaces;

public interface IContactsService
{
    public Task<List<SendContact>> GetContacts();
}