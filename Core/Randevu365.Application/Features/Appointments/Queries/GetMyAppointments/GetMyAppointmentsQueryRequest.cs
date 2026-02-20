using MediatR;
using Randevu365.Application.Common.Responses;

namespace Randevu365.Application.Features.Appointments.Queries.GetMyAppointments;

public class GetMyAppointmentsQueryRequest : IRequest<ApiResponse<IList<GetMyAppointmentsQueryResponse>>>
{
}
