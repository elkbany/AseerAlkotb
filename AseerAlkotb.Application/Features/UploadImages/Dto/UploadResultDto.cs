using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.UploadImages.Dto
{
    public class UploadResultDto
    {
        public string LocalUrl { get; set; } = string.Empty;
        public string CloudUrl { get; set; } = string.Empty;
    }
}
