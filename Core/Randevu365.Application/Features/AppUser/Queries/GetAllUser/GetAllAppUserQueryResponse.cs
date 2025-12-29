
using Randevu365.Domain.Enum;

namespace Randevu365.Application.Features.AppUser.Queries.GetAllUser;

public class GetAllAppUserQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public Roles Role { get; set; }
}