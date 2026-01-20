using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations
{
    public class DestinationImageService : IDestinationImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DestinationImageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DestinationImageDto>> GetByDestinationIdAsync(Guid destinationId)
        {
            var images = await _unitOfWork.DestinationImage.GetAllAsync(i => i.DestinationId == destinationId);
            return _mapper.Map<IEnumerable<DestinationImageDto>>(images);
        }

        public async Task<DestinationImageDto?> GetByIdAsync(Guid destinationImageId)
        {
            var image = await _unitOfWork.DestinationImage.GetAsync(i => i.DestinationImageId == destinationImageId);
            return image == null ? null : _mapper.Map<DestinationImageDto>(image);
        }

        public async Task<DestinationImageDto> CreateAsync(DestinationImageDto dto)
        {
            var entity = _mapper.Map<DestinationImage>(dto);
            entity.DestinationImageId = Guid.NewGuid();
            await _unitOfWork.DestinationImage.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DestinationImageDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid destinationImageId)
        {
            var entity = await _unitOfWork.DestinationImage.GetAsync(i => i.DestinationImageId == destinationImageId);
            if (entity == null)
            {
                return false;
            }

            await _unitOfWork.DestinationImage.RemoveAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
