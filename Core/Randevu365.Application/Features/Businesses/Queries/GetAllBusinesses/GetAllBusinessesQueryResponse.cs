using Randevu365.Domain.Entities;

namespace Randevu365.Application.Features.Businesses.Queries.GetAllBusinesses;

public class GetAllBusinessesQueryResponse
{
    public required Business Business { get; set; }
}
