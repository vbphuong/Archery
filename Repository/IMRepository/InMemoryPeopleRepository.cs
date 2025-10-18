using Archery.Models.DTO;
using Archery.Repository;

public class InMemoryPeopleRepository : IPeopleRepository
{
    private readonly List<UserDTO> _users = new();
    private readonly List<string> _roles = new() { "Admin", "Recruiter", "Applicant" };
    private int _nextId = 1;

    public Task<List<UserDTO>> GetAllAsync()
    {
        return Task.FromResult(_users.ToList());
    }

    public Task<UserDTO?> GetByIdAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.UserId == id);
        return Task.FromResult(user);
    }

    public Task<UserDTO> CreateAsync(UserDTO dto)
    {
        dto.UserId = _nextId++;
        if (string.IsNullOrEmpty(dto.RoleName))
        {
            dto.RoleName = _roles[1]; // mặc định Recruiter
        }
        _users.Add(dto);
        return Task.FromResult(dto);
    }

    public Task<bool> UpdateAsync(int id, UserDTO dto)
    {
        var existing = _users.FirstOrDefault(u => u.UserId == id);
        if (existing == null) return Task.FromResult(false);

        if (existing.RoleName == "Admin") return Task.FromResult(false);

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;

        if (!string.IsNullOrEmpty(dto.RoleName) && dto.RoleName != "Admin")
        {
            existing.RoleName = dto.RoleName;
        }

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.UserId == id);
        if (user == null) return Task.FromResult(false);

        if (user.RoleName == "Admin") return Task.FromResult(false);

        _users.Remove(user);
        return Task.FromResult(true);
    }
}
