using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation.Dtos;
using Newtonsoft.Json;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Foundation.Services
{
    public interface IImageEnhancerAppService : IRemoteService, IApplicationService
    {
        public Task<SegmentationApiResponseDto> PostSegmentedImageAsync(SegmentationApiRequestDtoWrapper imageRequest);
        public Task<PaPanoClassificationResponseDto> PostPaPanoClassificationAsync(PaPanoClassificationRequestDto imageRequest);
    }
}