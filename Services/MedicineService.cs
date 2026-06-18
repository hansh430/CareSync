using CareSync.Common;
using CareSync.Data;
using CareSync.Dtos;
using CareSync.Models;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly EmedicineContext _context;

        public MedicineService(EmedicineContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<Medicine>> AddMedicineAsync(AddMedicineDto model)
        {
            var response = new ServiceResponse<Medicine>();

            string? imagePath = null;

            // Upload image if provided
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images");

                // Create folder if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(model.ImageFile.FileName);

                string filePath = Path.Combine(
                    uploadsFolder,
                    fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imagePath = $"/images/{fileName}";
            }

            var medicine = new Medicine
            {
                Name = model.Name,
                Manufacturer = model.Manufacturer,
                UnitPrice = model.UnitPrice,
                Discount = model.Discount,
                Quantity = model.Quantity,
                ExpDate = model.ExpDate,
                ImageUrl = imagePath,
                Status = 1
            };

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            response.Data = medicine;
            response.Message = "Medicine added successfully.";

            return response;
        }

        public async Task<ServiceResponse<List<Medicine>>> GetMedicinesAsync()
        {
            return new ServiceResponse<List<Medicine>>
            {
                Data = await _context.Medicines.ToListAsync()
            };
        }

        public async Task<ServiceResponse<Medicine>> GetMedicineByIdAsync(int id)
        {
            var response = new ServiceResponse<Medicine>();
            var medicine = await _context.Medicines.FirstOrDefaultAsync(x => x.Id == id);
            if (medicine == null)
            {
                response.Success = false;
                response.Message = "Medicine not found.";
                return response;
            }
            response.Data = medicine;

            return response;
        }
        public async Task<ServiceResponse<Medicine>> UpdateMedicineAsync(int id, UpdateMedicineDto model)
        {
            var response = new ServiceResponse<Medicine>();

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(x => x.Id == id);

            if (medicine == null)
            {
                response.Success = false;
                response.Message = "Medicine not found.";
                return response;
            }

            // Update normal fields
            medicine.Name = model.Name;
            medicine.Manufacturer = model.Manufacturer;
            medicine.UnitPrice = model.UnitPrice;
            medicine.Discount = model.Discount;
            medicine.Quantity = model.Quantity;
            medicine.ExpDate = model.ExpDate;
            medicine.Status = model.Status;
            // Upload new image if provided
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // Delete old image (optional but recommended)
                if (!string.IsNullOrEmpty(medicine.ImageUrl))
                {
                    string oldImagePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        medicine.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                // Upload new image
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images");

                // Create folder if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(model.ImageFile.FileName);

                string filePath = Path.Combine(
                    uploadsFolder,
                    fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // Update image path in database
                medicine.ImageUrl = $"/images/{fileName}";
            }

            await _context.SaveChangesAsync();

            response.Data = medicine;
            response.Message = "Medicine updated successfully.";

            return response;
        }
        public async Task<ServiceResponse<bool>> DeleteMedicineAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(x => x.Id == id);

            if (medicine == null)
            {
                response.Success = false;
                response.Message = "Medicine not found.";
                return response;
            }

            medicine.Status = 0;

            await _context.SaveChangesAsync();

            response.Data = true;
            response.Message = "Medicine deleted successfully.";

            return response;
        }


    }
}
