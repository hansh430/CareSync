using CareSync.Common;
using CareSync.Dtos;
using CareSync.Models;

namespace CareSync.Services
{
    public interface IMedicineService
    {
        Task<ServiceResponse<Medicine>> AddMedicineAsync(AddMedicineDto model);

        Task<ServiceResponse<List<Medicine>>> GetMedicinesAsync();
    }
}
