using ClinicalScheduler.Domain.Entities;

namespace ClinicalScheduler.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Staff staff);
}
