using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.BusinessSlots.Queries.GetMySlots;

public class GetMySlotsQueryRequest : IRequest<ApiResponse<GetMySlotsQueryResponse>> { }
