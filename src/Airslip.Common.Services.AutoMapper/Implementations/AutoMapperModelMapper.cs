using Airslip.Common.Repository.Interfaces;
using AutoMapper;

namespace Airslip.Common.Services.AutoMapper.Implementations
{
    public class AutoMapperModelMapper<TModel> : IModelMapper<TModel>
    {
        private readonly IMapper _mapper;

        public AutoMapperModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDest Create<TDest>(TModel source)
        {
            return _mapper.Map<TModel, TDest>(source);
        }

        public TDest Update<TDest>(TModel source, TDest destination)
        {
            return _mapper.Map(source, destination);
        }

        public TModel Create<TEntity>(TEntity source)
        {
            return _mapper.Map<TEntity, TModel>(source);
        }
    }
}