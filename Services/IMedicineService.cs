using CareSync.Common;
using CareSync.Dtos;
using CareSync.Models;

namespace CareSync.Services
{
    public interface IMedicineService
    {
        Task<ServiceResponse<Medicine>> AddMedicineAsync(AddMedicineDto model);

        Task<ServiceResponse<List<Medicine>>> GetMedicinesAsync();
        Task<ServiceResponse<Medicine>> GetMedicineByIdAsync(int id);
        Task<ServiceResponse<Medicine>> UpdateMedicineAsync(int id, UpdateMedicineDto model);
        Task<ServiceResponse<bool>> DeleteMedicineAsync(int id);
    }
}
