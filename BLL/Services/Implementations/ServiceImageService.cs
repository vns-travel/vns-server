using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations
{
    public class ServiceImageService : IServiceImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceImageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ServiceImageDto>> GetByServiceIdAsync(Guid serviceId)
        {
            var images = await _unitOfWork.ServiceImage.GetAllAsync(i => i.ServiceId == serviceId);
            return _mapper.Map<IEnumerable<ServiceImageDto>>(images);
        }

        public async Task<ServiceImageDto?> GetByIdAsync(Guid serviceImageId)
        {
            var image = await _unitOfWork.ServiceImage.GetAsync(i => i.ServiceImageId == serviceImageId);
            return image == null ? null : _mapper.Map<ServiceImageDto>(image);
        }

        public async Task<ServiceImageDto> CreateAsync(ServiceImageDto dto)
        {
            var entity = _mapper.Map<ServiceImage>(dto);
            entity.ServiceImageId = Guid.NewGuid();
            await _unitOfWork.ServiceImage.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ServiceImageDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid serviceImageId)
        {
            var entity = await _unitOfWork.ServiceImage.GetAsync(i => i.ServiceImageId == serviceImageId);
            if (entity == null)
            {
                return false;
            }

            await _unitOfWork.ServiceImage.RemoveAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
