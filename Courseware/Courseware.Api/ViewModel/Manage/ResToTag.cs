using System.Collections.Generic;
using Courseware.Core.Entities;

namespace Courseware.Api.ViewModel.Manage
{
    public class ResToTag
    {
        public ResourceEntity Resource { get; set; }

        public List<long> Tags { get; set; }
    }
}
