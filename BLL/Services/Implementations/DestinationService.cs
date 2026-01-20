using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations
{
    public class DestinationService : IDestinationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DestinationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DestinationDto>> GetAllAsync()
        {
            var destinations = await _unitOfWork.Destination.GetAllAsync();
            return _mapper.Map<IEnumerable<DestinationDto>>(destinations);
        }

        public async Task<DestinationDto?> GetByIdAsync(Guid destinationId)
        {
            var destination = await _unitOfWork.Destination.GetAsync(d => d.DestinationId == destinationId);
            return destination == null ? null : _mapper.Map<DestinationDto>(destination);
        }

        public async Task<DestinationDto> CreateAsync(DestinationDto dto)
        {
            var entity = _mapper.Map<Destination>(dto);
            entity.DestinationId = Guid.NewGuid();
            await _unitOfWork.Destination.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DestinationDto>(entity);
        }

        public async Task<bool> UpdateAsync(Guid destinationId, DestinationDto dto)
        {
            var entity = await _unitOfWork.Destination.GetAsync(d => d.DestinationId == destinationId);
            if (entity == null)
            {
                return false;
            }

            _mapper.Map(dto, entity);
            entity.DestinationId = destinationId;
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Destination.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid destinationId)
        {
            var entity = await _unitOfWork.Destination.GetAsync(d => d.DestinationId == destinationId);
            if (entity == null)
            {
                return false;
            }

            await _unitOfWork.Destination.RemoveAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
