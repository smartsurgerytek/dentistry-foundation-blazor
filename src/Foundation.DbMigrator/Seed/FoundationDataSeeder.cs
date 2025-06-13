using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.Entities;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace Foundation.Application.Seed
{
    public class FoundationDataSeedContributor
        : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Organization, Guid> _organisationRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IIdentityRoleRepository _roleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IdentityRoleManager _roleManager;
        private readonly IGuidGenerator _guidGenerator;

        // Constants for default doctor user and role
        private const string doctorRoleName = "Doctor";
        private const string defaultDoctorUserName = "drjohnsmith";
        private const string defaultDoctorEmail = "drjohnsmith@dental.com";
        private const string defaultDoctorPassword = "Doctor123!";
        // Default values for the doctor entity
        private const string defaultDoctorFullName = "Dr. John Smith";
        private const string defaultDoctorSpecialty = "Dentist";
        private const string defaultDoctorDepartment = "Dental";
        private const string defaultOrganizationName = "Dental Organization";
        private const string defaultOrganizationAddress = "123 Foundation St, Health City";

        public FoundationDataSeedContributor(IRepository<Organization, Guid> organisationRepository, IdentityUserManager userManager, IIdentityRoleRepository roleRepository, IUnitOfWorkManager unitOfWorkManager, IGuidGenerator guidGenerator, IdentityRoleManager roleManager)
        {
            _organisationRepository = organisationRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _guidGenerator = guidGenerator;
            _roleManager = roleManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await SeedDefaultDoctorUserAsync(context);
            // Seed initial data for the organization and departments
            await SeedInitialDataAsync();
            // You can add more seed methods here for other entities as needed
        }

        public async Task SeedDefaultDoctorUserAsync(DataSeedContext context)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true);

            // Step 1: Create the Doctor role if it doesn't exist
            var doctorRole = await _roleRepository.FindByNormalizedNameAsync(doctorRoleName.ToUpperInvariant());
            if (doctorRole == null)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(Guid.NewGuid(), doctorRoleName));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to create role '{doctorRoleName}': {string.Join(", ", roleResult.Errors)}");
                }

                doctorRole = await _roleRepository.FindByNormalizedNameAsync(doctorRoleName.ToUpperInvariant());
            }

            // Step 2: Create the default doctor user if it doesn't exist
            var user = await _userManager.FindByNameAsync(defaultDoctorUserName);
            if (user == null)
            {
                user = new IdentityUser(_guidGenerator.Create(), defaultDoctorUserName, defaultDoctorEmail);
                user.SetEmailConfirmed(true);
                var result = await _userManager.CreateAsync(user, defaultDoctorPassword);

                if (result.Succeeded)
                {
                    if (doctorRole != null)
                    {
                        await _userManager.AddToRoleAsync(user, doctorRole.Name);
                    }
                }
                else
                {
                    throw new Exception($"Failed to create user '{defaultDoctorUserName}': {string.Join(", ", result.Errors)}");
                }
            }

            await uow.CompleteAsync();
        }

        private async Task SeedInitialDataAsync()
        {
            if (await _organisationRepository.GetCountAsync() > 0)
            {
                return;
            }

            var organisation = new Organization
            {
                Name = defaultOrganizationName,
                Address = defaultOrganizationAddress,
                Departments =
                [
                    new() {
                        Name = defaultDoctorDepartment,
                        Doctors =
                        [
                            new Doctor
                            {
                                Name = defaultDoctorFullName,
                                Specialty = defaultDoctorSpecialty
                            }
                        ]
                    }
                ]
            };

            await _organisationRepository.InsertAsync(organisation);
        }
    }
}
