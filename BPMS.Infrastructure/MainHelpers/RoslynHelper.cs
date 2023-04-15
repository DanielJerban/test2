namespace BPMS.Infrastructure.MainHelpers;

public class RoslynHelper
{
    public static string GetScheduleClass(string code)
    {
        return @"
                        using System;
                        using System.Linq;
                        using System.Text;
                        using System.Collections.Generic;
                        using BPMS.Core;
                        using System.Web.Mvc;
                        using System.Web;
                        using System.Data.Entity;
                        using BPMS.Core.Models;
                        using Newtonsoft.Json;
                        using System.Globalization;
                        using BPMS.Infrastructure.MainHelpers;
                        using RestSharp;
                        using BPMS.Domain.Common.ViewModels;
                        using BPMS.Infrastructure;
                        using BPMS.Infrastructure.Services;

                        namespace BPMS.Core.Models
                        {
                            public  class MyClass
                            {
                                private static readonly IJobService _jobService = DependencyResolver.Current.GetService<IJobService>();
                                private static readonly IUnitOfWork unitOfWork = DependencyResolver.Current.GetService<IUnitOfWork>();

                                public  void MyMethod()
                                {
                                    " + code + @"
                                }
                            }

                        }";
    }

    public static string GetScheduleCheckClass(string method)
    {
        return @"
            using System;
            using System.Linq;
            using BPMS.Infrastructure;
            using System.Collections.Generic;
            using System.Data.Entity;
            using BPMS.Domain.Entities;
            using BPMS.Infrastructure.Services;
            using Newtonsoft.Json;
            using BPMS.Domain.Common.ViewModels;
            using System.Globalization;
            using BPMS.Infrastructure.MainHelpers;
            using RestSharp;
            using System.Web.Mvc;
            using System.Web;
            using System.Text;
            using BPMS.Infrastructure;
            using BPMS.Infrastructure.Services;

            namespace BPMS.Domain.Entities
            {
                public class myClass
                {
                     private static readonly IJobService _jobService = DependencyResolver.Current.GetService<IJobService>();
                     private static readonly IUnitOfWork unitOfWork = DependencyResolver.Current.GetService<IUnitOfWork>();

                   " + method + @"
                }
            }";
    }
}