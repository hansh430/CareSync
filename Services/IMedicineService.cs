using CareSync.Common;
using CareSync.Dtos;
using CareSync.Models;

namespace CareSync.Services
{
    public interface IMedicineService
    {
        Task<ServiceResponse<Medicine>> AddMedicineAsync(AddMedicineDto model);

        Task<ServiceResponse<PagedResultDto<Medicine>>> GetMedicinesAsync(int page, int pageSize);
        Task<ServiceResponse<Medicine>> GetMedicineByIdAsync(int id);
        Task<ServiceResponse<Medicine>> UpdateMedicineAsync(int id, UpdateMedicineDto model);
        Task<ServiceResponse<bool>> DeleteMedicineAsync(int id);
    }
}
