using AutoMapper;
using coretest.Domain.Models;
using coretest.Resources;

namespace coretest.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<User, UserResource>();
        }
    }
}
