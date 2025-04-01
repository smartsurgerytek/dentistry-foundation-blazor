using Foundation.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation.Services;

namespace Foundation.Controllers
{
    [Route("api/organizations")]
    [ApiController]
    public class OrganizationController : FoundationController
    {
        private readonly IOrganizationAppService _organizationAppService;

        public OrganizationController(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrganizationDto>>> GetOrganizations()
        {
            var organizations = await _organizationAppService.GetOrganizationsAsync();
            return Ok(organizations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDto>> GetOrganization(Guid id)
        {
            var organization = await _organizationAppService.GetOrganizationAsync(id);
            if (organization == null)
            {
                return NotFound();
            }
            return Ok(organization);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrganization([FromBody] CreateUpdateOrganizationDto input)
        {
            if (input == null)
            {
                return BadRequest();
            }

            await _organizationAppService.CreateOrganizationAsync(input);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrganization(Guid id, [FromBody] CreateUpdateOrganizationDto input)
        {
            await _organizationAppService.UpdateOrganizationAsync(id, input);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrganization(Guid id)
        {
            await _organizationAppService.DeleteOrganizationAsync(id);
            return NoContent();
        }
    }
}
