using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation.Application.Contracts.Dtos;
using Foundation.Dtos;
using Newtonsoft.Json;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Foundation.Services
{
    public interface IDentistryApiAppService : IRemoteService, IApplicationService
    {
        public Task<SegmentationApiResponseDto> PostSegmentedImageAsync(SegmentationApiRequestDtoWrapper imageRequest);
        public Task<PaPanoClassificationResponseDto> PostPaPanoClassificationAsync(PaPanoClassificationRequestDto imageRequest);
        public Task<FDISegmentationResponseDto> PostPanoFdiSegmentationCvatAsync(SegmentationApiRequestDtoWrapper imageRequest);
        public Task<SegmentationApiResponseDto> PostMeasurementImageAsync(SegmentationApiRequestDtoWrapper imageRequest);
        public Task<bool> IsPanoEnabledAsync();
    }
}