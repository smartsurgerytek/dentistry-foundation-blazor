using AutoMapper.Internal.Mappers;
using Foundation.Dtos;
using Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        private readonly IRepository<AuditLog, Guid> _auditLogRepository;

        public DepartmentAppService(
            IRepository<Department, Guid> departmentRepository,
            IRepository<AuditLog, Guid> auditLogRepository)
        {
            _departmentRepository = departmentRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetListAsync();

            await LogAudit("GetDepartments", string.Empty);

            return ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto> GetDepartmentAsync(Guid departmentId)
        {
            var department = await _departmentRepository.FirstOrDefaultAsync(x => x.Id == departmentId);
            if (department == null)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }

            await LogAudit("GetDepartment", departmentId);

            return ObjectMapper.Map<Department, DepartmentDto>(department);
        }

        public async Task CreateDepartmentAsync(CreateUpdateDepartmentDto input)
        {
            var department = ObjectMapper.Map<CreateUpdateDepartmentDto, Department>(input);
            await _departmentRepository.InsertAsync(department);

            await LogAudit("CreateDepartment", input);
        }

        public async Task UpdateDepartmentAsync(Guid departmentId, CreateUpdateDepartmentDto input)
        {
            var department = await _departmentRepository.FirstOrDefaultAsync(x => x.Id == departmentId);
            if (department == null)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }

            ObjectMapper.Map(input, department);
            await _departmentRepository.UpdateAsync(department);

            await LogAudit("UpdateDepartment", input);
        }

        public async Task DeleteDepartmentAsync(Guid departmentId)
        {
            var department = await _departmentRepository.FirstOrDefaultAsync(x => x.Id == departmentId);
            if (department == null)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }

            await _departmentRepository.DeleteAsync(department);

            await LogAudit("DeleteDepartment", departmentId);
        }

        public async Task<List<DepartmentDto>> GetDepartmentsByAsync(Guid organizationId)
        {
            var departments = await _departmentRepository.GetListAsync(x => x.OrganizationId == organizationId);

            await LogAudit("GetDepartmentsBy", organizationId);

            return ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments);
        }

        private async Task LogAudit(string methodName, object parameters)
        {
            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(DepartmentAppService),
                MethodName = $"Department - {methodName}",
                Parameters = JsonSerializer.Serialize(parameters),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });
        }
    }

}