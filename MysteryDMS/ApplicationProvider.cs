using Mystery.Applications;
using System;
using System.Collections.Generic;

namespace MysteryDMS
{
    public class ApplicationProvider : IApplicationProvider
    {
        public IEnumerable<MysteryApplication> getApplications()
        {
            return new List<MysteryApplication>()
            {
                new MysteryApplication()
                {
                    guid = Guid.Parse("5fadb977-3252-45f0-9ead-b458b85a7c77"),
                    name = "DMS",
                    short_label = "DMS.DMS",
                    label = "DMS.DMS_NAME",
                    start_page = "start",
                }
            };
        }
    }
}
