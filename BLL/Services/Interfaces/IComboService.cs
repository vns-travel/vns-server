using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IComboService
    {
        Task<IEnumerable<ComboDto>> GetAllCombosAsync(bool includeInactive = false);
        Task<IEnumerable<ComboDto>> GetPartnerCombosAsync(Guid userId);
        Task<ComboDto?> GetComboByIdAsync(Guid comboId, bool includeInactive = false);
        Task<ComboDto> CreateComboAsync(Guid userId, ComboDto comboDto);
        Task<ComboDto> ApproveComboAsync(Guid comboId);
        Task<bool> DeleteComboAsync(Guid userId, Guid comboId);        
    }
}
