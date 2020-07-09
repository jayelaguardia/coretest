using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using coretest.Domain.Models;
using coretest.Resources;


namespace coretest.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<CreateUserResource, User>();

            CreateMap<LoginResource, Auth>();
        }
    }
}
