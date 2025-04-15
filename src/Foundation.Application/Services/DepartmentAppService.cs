using AutoMapper.Internal.Mappers;
using Foundation.Dtos;
using Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Foundation.Services
{
    public class DepartmentAppService : ApplicationService, IDepartmentAppService, ITransientDependency
    {
        private readonly IRepository<Department, Guid> _departmentRepository;

        public DepartmentAppService(IRepository<Department, Guid> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetListAsync();
            return ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto> GetDepartmentAsync(Guid departmentId)
        {
            var department = await _departmentRepository.FirstOrDefaultAsync(x => x.Id == departmentId, CancellationToken.None);
            if (department == null)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }
            return ObjectMapper.Map<Department, DepartmentDto>(department);
        }

        public async Task CreateDepartmentAsync(CreateUpdateDepartmentDto input)
        {
            var department = ObjectMapper.Map<CreateUpdateDepartmentDto, Department>(input);
            await _departmentRepository.InsertAsync(department);
        }

        public async Task UpdateDepartmentAsync(Guid departmentId, CreateUpdateDepartmentDto input)
        {
            var department = await _departmentRepository.FirstOrDefaultAsync(x => x.Id == departmentId, CancellationToken.None);
            if (department == null)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }

            ObjectMapper.Map(input, department);
            await _departmentRepository.UpdateAsync(department);
        }

        public async Task DeleteDepartmentAsync(Guid departmentId)
        {
            var department = await _departmentRepository.FirstOrDefaultAsync(x => x.Id == departmentId, CancellationToken.None);
            if (department == null)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }

            await _departmentRepository.DeleteAsync(department);
        }

        public async Task<List<DepartmentDto>> GetDepartmentsByAsync(Guid OrganizationId)
        {
            var departments = await _departmentRepository.GetListAsync(x => x.OrganizationId == OrganizationId);
            return ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments);
        }
    }
}